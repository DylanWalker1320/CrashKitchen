using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections;

public class FoodSupplySpawner : XRGrabInteractable
{
    [Header("Food Supply Settings")]
    [Tooltip("Prefab for the food object to spawn")]
    public GameObject foodPrefab;
    [Tooltip("Spawn point for the food (could be a child transform)")]
    public Transform spawnPoint;

    [Tooltip("Spawn the food at the center of this object")]
    public bool spawnAtCenter = true;

    [Tooltip("Scale multiplier for spawned food")]
    public float scaleMultiplier = 0.3f;

    [Tooltip("Cooldown between spawns in seconds")]
    public float spawnCooldown = 1.0f;
    private bool canSpawn = true;
    private Transform foodContainer;

    // Store original position, rotation, and parent for this game object
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;
    
    protected override void Awake()
    {
        base.Awake();

        GameObject FoodHolder = GameObject.FindWithTag("FoodHolder");
        if (FoodHolder != null)
        {
            foodContainer = FoodHolder.transform;
            // Make sure FoodHolder has a NetworkObject component
            if (!FoodHolder.TryGetComponent<NetworkObject>(out _))
            {
                NetworkObject containerNetObj = FoodHolder.AddComponent<NetworkObject>();
                containerNetObj.DontDestroyWithOwner = true;
                containerNetObj.Spawn();
            }
        }
        else
        {
            Debug.LogWarning("No 'Food' GameObject found in scene. Spawned food will not be parented.");
        }
    }

    public void SpawnNewFood(SelectEnterEventArgs args)
    {        
        Debug.Log($"Food supply grabbed {args}");
        
        if (canSpawn && foodPrefab != null)
        {
            Debug.Log("Spawning food");

            Vector3 spawnPosition;
            Quaternion spawnRotation;
            
            if (spawnAtCenter || spawnPoint == null)
            {
                spawnPosition = transform.position;
                spawnRotation = transform.rotation;
            }
            else
            {
                spawnPosition = spawnPoint.position;
                spawnRotation = spawnPoint.rotation;
            }

            NetworkObject prefabNetObj = foodPrefab.GetComponent<NetworkObject>();
            if (prefabNetObj == null)
            {
                Debug.LogError("Food prefab must have a NetworkObject component!");
                return;
            }

            Debug.Log("Spawning food prefab: " + prefabNetObj.name);

            NetworkObject spawnedNetObj = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(
                prefabNetObj,                  // The prefab to spawn
                NetworkManager.ServerClientId, // Server owns this object
                false,                         // Don't destroy with scene
                false,                         // Not a player object  
                false,                         // Don't force override
                spawnPosition,                 // Position
                spawnRotation                  // Rotation
            );

            Debug.Log("Spawned food prefab: " + spawnedNetObj.name);
            GameObject spawnedFood = spawnedNetObj.gameObject;
            Debug.Log("Spawned: "   + spawnedFood.name);

            AdjustFoodScale(spawnedFood);

            if (args.interactorObject is IXRInteractor xrInteractor)
            {
                Transform interactorTransform = (xrInteractor as MonoBehaviour)?.transform;
                if (interactorTransform != null)
                {
                    // Set position to be right in front of the controller
                    spawnedFood.transform.position = interactorTransform.position + 
                                                    interactorTransform.forward * 0.1f;
                    spawnedFood.transform.rotation = interactorTransform.rotation;
                }
            }

            // Set the rigidbody settings for the spawned food object
            Rigidbody foodRigidbody = spawnedFood.GetComponent<Rigidbody>();
            if (foodRigidbody != null)
            {
                Debug.Log("Found Rigidbody on spawned food prefab: " + foodRigidbody.name);
                foodRigidbody.isKinematic = false;
                foodRigidbody.useGravity = true;
                foodRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                foodRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                foodRigidbody.constraints = RigidbodyConstraints.None;
            }
            else
            {
                Debug.LogError("Spawned food prefab does not have a Rigidbody component!");
            }

            if (foodContainer != null)
            {
                Debug.Log("Attempting to parent spawned food to FoodHolder");
                NetworkObject containerNetObj = foodContainer.GetComponent<NetworkObject>();
                if (containerNetObj != null && containerNetObj.IsSpawned)
                {
                    // Store the world position before parenting
                    Vector3 worldPos = spawnedFood.transform.position;
                    Quaternion worldRot = spawnedFood.transform.rotation;

                    // Use Netcode's proper parenting API
                    bool success = spawnedNetObj.TrySetParent(containerNetObj);
                    Debug.Log($"Parenting result: {success}");
                    
                    if (success) {
                        // Don't reset to zero - keep the object at the original spawn position relative to parent
                        // Calculate relative position from parent to spawner
                        // Vector3 relativePosition = containerNetObj.transform.InverseTransformPoint(transform.position);
                        // spawnedNetObj.transform.localPosition = relativePosition;

                        spawnedFood.transform.position = worldPos;
                        spawnedFood.transform.rotation = worldRot;
                    }
                }
                else
                {
                    Debug.LogError("FoodHolder NetworkObject not found or not spawned!");
                }
            }


            // Ensure the new food object has an XRGrabInteractable component
            XRGrabInteractable grabInteractable = spawnedFood.GetComponent<XRGrabInteractable>();
            if (grabInteractable == null)
            {
                grabInteractable = spawnedFood.AddComponent<XRGrabInteractable>();
            }

            grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking; 
            grabInteractable.throwOnDetach = true;
            grabInteractable.throwSmoothingDuration = 0.2f;
            grabInteractable.throwSmoothingCurve = AnimationCurve.Linear(0, 1, 1, 0);

            // Transfer the interaction to the newly spawned object
            IXRSelectInteractor interactor = args.interactorObject;
            if (interactor != null)
            {
                if (spawnedNetObj != null)
                {
                    ulong clientId = NetworkManager.Singleton.LocalClientId;
                    spawnedNetObj.ChangeOwnership(clientId);
                }

                StartCoroutine(TransferGrabNextFrame(interactor, grabInteractable));
                // // First release the current object
                // interactionManager.SelectExit(interactor, this);
                
                // // Then select the new food object
                // interactionManager.SelectEnter(interactor, grabInteractable);
            }

            canSpawn = false;
            Invoke(nameof(ResetSpawn), spawnCooldown);
        }
    }

    private IEnumerator TransferGrabNextFrame(IXRSelectInteractor interactor, XRGrabInteractable grabInteractable)
        {
            // Wait one frame for everything to initialize
            yield return null;
            
            Transform interactorTransform = (interactor as MonoBehaviour)?.transform;
            if (interactorTransform != null)
            {
                grabInteractable.transform.position = interactorTransform.position + 
                                                    interactorTransform.forward * 0.05f;
            }

            // First release the current object
            interactionManager.SelectExit(interactor, this);
            
            // Wait a tiny bit
            yield return new WaitForSeconds(0.05f);
            
            // Then select the new food object
            interactionManager.SelectEnter(interactor, grabInteractable);
            
            Debug.Log("Transferred grab to: " + grabInteractable.name);
        }

    private void AdjustFoodScale(GameObject spawnedFood)
    {
        if (spawnedFood.name == "Bun(Clone)")
        {
            scaleMultiplier = 1f;
        }
        else if (spawnedFood.name == "Steak(Clone)")
        {
            scaleMultiplier = 0.0024f;
        }
        else if (spawnedFood.name == "Lettuce(Clone)")
        {
            scaleMultiplier = 0.08f;
        }
        else if (spawnedFood.name == "Patty(Clone)")
        {
            scaleMultiplier = 0.08f;
        }
        else if (spawnedFood.name == "Hotdog Bun(Clone)")
        {
            scaleMultiplier = 0.03f;
        }
        else if (spawnedFood.name == "Hotdog(Clone)")
        {
            scaleMultiplier = 0.03f;
        }

        spawnedFood.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
    }


    private void ResetSpawn()
    {
        canSpawn = true;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        
        // Store position at moment of grabbing
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;
        
        Debug.Log("Stored position when grabbed");
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        
        // Find the kitchen container by tag
        GameObject kitchenContainer = GameObject.FindWithTag("InteriorKitchenObjects");
        if (kitchenContainer != null)
        {
            // First parent to kitchen objects
            transform.SetParent(kitchenContainer.transform);
            
            // Then restore the position/rotation
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            
            Debug.Log($"Returned {gameObject.name} to kitchen container at original position");
        }
        else
        {
            // Fallback if kitchen objects container isn't found
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            if (originalParent != null)
            {
                transform.SetParent(originalParent);
            }
            Debug.Log($"Could not find InteriorKitchenObjects, using original parent instead");
        }
    }
}

