using Unity.Netcode;
using Unity.Netcode.Components;
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
    private NetworkObject networkObject;
    private IXRSelectInteractor interactor;
    private IXRSelectInteractor lastInteractor;
    
    protected override void Awake()
    {
        base.Awake();

        networkObject = GetComponent<NetworkObject>();

        GameObject FoodHolder = GameObject.FindWithTag("FoodHolder");
        if (FoodHolder != null)
        {
            foodContainer = FoodHolder.transform;
            // Make sure FoodHolder has a NetworkObject component
            if (!FoodHolder.TryGetComponent<NetworkObject>(out _))
            {
                var containerNetObj = FoodHolder.AddComponent<NetworkObject>();
                containerNetObj.DontDestroyWithOwner = true;
                
                if (NetworkManager.Singleton.IsServer)
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
        
        if (!canSpawn || foodPrefab == null) return;
        
        lastInteractor = args.interactorObject as IXRSelectInteractor;

        Transform interactorTransform = null;
        if (args.interactorObject is IXRInteractor xrInteractor)
        {
            interactorTransform = (xrInteractor as MonoBehaviour)?.transform;
        }

        // Determine spawn position/rotation
        Vector3 spawnPosition = transform.position;
        // spawnPosition.y += 1f; // Add 3 to the y axis to spawn above the spawner
        Quaternion spawnRotation = transform.rotation;

        // if (interactorTransform != null)
        // {
        //     // Set position to be right in front of the controller/ray
        //     spawnPosition = interactorTransform.position + interactorTransform.forward * 0.2f;
        //     spawnRotation = interactorTransform.rotation;
        // }
        // else
        // {
        //     // Fallback to spawner position
        //     spawnPosition = transform.position;
        //     spawnPosition.y += 0.2f; // Slightly above spawner
        //     spawnRotation = transform.rotation;
        // }

        // Get local client ID for ownership
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        
        if (NetworkManager.Singleton.IsServer)
        {
            // Server can spawn directly
            SpawnFoodServerSide(foodPrefab.name, spawnPosition, spawnRotation, clientId);
        }
        else
        {
            // Clients need to request server to spawn
            SpawnFoodServerRpc(foodPrefab.name, spawnPosition, spawnRotation, clientId);
        }
        
        // Cooldown regardless of whether server/client
        canSpawn = false;
        Invoke(nameof(ResetSpawn), spawnCooldown);
    }

    // Server RPC method called by clients
    [ServerRpc(RequireOwnership = false)]
    private void SpawnFoodServerRpc(string prefabName, Vector3 position, Quaternion rotation, ulong ownerClientId)
    {
        // Only process on server
        if (!NetworkManager.Singleton.IsServer) return;
        
        Debug.Log($"Server received spawn request for {prefabName}");
        SpawnFoodServerSide(prefabName, position, rotation, ownerClientId);
    }

    // Common spawning logic (runs on server only)
    private void SpawnFoodServerSide(string prefabName, Vector3 position, Quaternion rotation, ulong ownerClientId)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("SpawnFoodServerSide should only be called on server!");
            return;
        }
        
        // Find the matching prefab
        NetworkObject prefabNetObj = foodPrefab.GetComponent<NetworkObject>();
        if (prefabNetObj == null)
        {
            Debug.LogError("Food prefab must have a NetworkObject component!");
            return;
        }
        
        // Spawn the food object
        NetworkObject spawnedNetObj = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(
            prefabNetObj,
            NetworkManager.ServerClientId,  // Server spawns initially
            false,                         
            false,                         
            false,                         
            position,                 
            rotation                  
        );
        
        // Transfer ownership to requesting client
        if (spawnedNetObj.OwnerClientId != ownerClientId)
        {
            spawnedNetObj.ChangeOwnership(ownerClientId);
        }
        
        GameObject spawnedFood = spawnedNetObj.gameObject;
        
        // Configure spawned food
        ConfigureSpawnedFood(spawnedFood);
        
        // Parenting logic...
        if (foodContainer != null)
        {
            NetworkObject containerNetObj = foodContainer.GetComponent<NetworkObject>();
            if (containerNetObj != null && containerNetObj.IsSpawned)
            {
                // Store the position we spawned the food at
                Vector3 worldPos = position;
                Quaternion worldRot = rotation;

                // Try both parenting approaches
                bool networkParented = spawnedNetObj.TrySetParent(containerNetObj);
        
                // ALSO set normal parenting as a backup
                spawnedFood.transform.SetParent(foodContainer);
                
                // Force position immediately after parenting
                spawnedFood.transform.position = worldPos;
                spawnedFood.transform.rotation = worldRot;
                
                // And schedule multiple resets through coroutine
                StartCoroutine(ResetPositionAfterParenting(spawnedFood.transform, worldPos, worldRot));
            }
        }

        if (ownerClientId == NetworkManager.Singleton.LocalClientId && lastInteractor != null)
        {
            // Start a coroutine to grab the spawned food after it's fully configured
            StartCoroutine(AutoGrabFoodNextFrame(lastInteractor, spawnedFood));
        }
    }

    // Configure spawned food physics & interactions
    private void ConfigureSpawnedFood(GameObject spawnedFood)
    {
        AdjustFoodScale(spawnedFood);
        
        // Configure rigidbody
        Rigidbody rb = spawnedFood.GetComponent<Rigidbody>();
        if (rb == null) rb = spawnedFood.AddComponent<Rigidbody>();
        
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.constraints = RigidbodyConstraints.None;
        
        // Configure grab interactable
        XRGrabInteractable grabInteractable = spawnedFood.GetComponent<XRGrabInteractable>();
        if (grabInteractable == null) grabInteractable = spawnedFood.AddComponent<XRGrabInteractable>();
        
        // Basic grab settings
        grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        grabInteractable.throwOnDetach = true;
        grabInteractable.throwSmoothingDuration = 0.2f;
        grabInteractable.throwSmoothingCurve = AnimationCurve.Linear(0, 1, 1, 0);
        
        // Critical settings for ray interactor compatibility
        grabInteractable.interactionLayers = InteractionLayerMask.GetMask("Default", "Grab", "Interactable");
        
        // Make sure rigidbody doesn't freeze before user can grab it
        StartCoroutine(DelayedPhysicsActivation(rb));
    }

    // New method to enable auto-grabbing after spawn:
    private IEnumerator AutoGrabFoodNextFrame(IXRSelectInteractor interactor, GameObject spawnedFood)
    {
        if (interactor == null) yield break;
        
        // Wait for everything to initialize
        yield return new WaitForSeconds(0.1f);
        
        // Get the interactor's transform
        Transform interactorTransform = (interactor as MonoBehaviour)?.transform;
        
        // Get the interaction manager
        var interactionManager = FindObjectOfType<XRInteractionManager>();
        if (interactionManager == null) yield break;
        
        // Get the grab component
        XRGrabInteractable grabInteractable = spawnedFood.GetComponent<XRGrabInteractable>();
        if (grabInteractable == null) yield break;
        
        // Move the food object close to the interactor
        spawnedFood.transform.position = interactorTransform.position + 
                                        interactorTransform.forward * 0.1f;
        
        // Release the spawner
        interactionManager.SelectExit(interactor, this);
        
        // Wait a tiny bit for exit to process
        yield return new WaitForSeconds(0.05f);
        
        // Force selection of the food object
        interactionManager.SelectEnter(interactor, grabInteractable);
        
        Debug.Log($"Auto-grabbed food: {spawnedFood.name}");
    }

    private IEnumerator DelayedPhysicsActivation(Rigidbody rb)
    {
        // Briefly make kinematic to prevent falling before player can grab
        rb.isKinematic = true;
        yield return new WaitForSeconds(0.5f);
        rb.isKinematic = false;
    }

    private IEnumerator ResetPositionAfterParenting(Transform objTransform, Vector3 worldPos, Quaternion worldRot)
    {
        // Try multiple times with increasing delays to overcome network sync issues
        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(0.1f * i);  // Progressive delays
            
            // Try to get NetworkTransform if available
            NetworkTransform networkTransform = objTransform.GetComponent<NetworkTransform>();
            if (networkTransform != null && networkTransform.IsOwner)
            {
                // Use NetworkTransform to teleport
                networkTransform.Teleport(worldPos, worldRot, objTransform.localScale);
                Debug.Log($"Reset attempt {i}: {objTransform.name} via NetworkTransform to {worldPos}");
            }
            else
            {
                // Fallback to direct transform setting
                objTransform.position = worldPos;
                objTransform.rotation = worldRot;
                Debug.Log($"Reset attempt {i}: {objTransform.name} directly to {worldPos}");
            }
        }
    }

    // private IEnumerator TransferGrabNextFrame(IXRSelectInteractor interactor, XRGrabInteractable grabInteractable)
    //     {
    //         // Wait one frame for everything to initialize
    //         yield return null;
            
    //         Transform interactorTransform = (interactor as MonoBehaviour)?.transform;
    //         if (interactorTransform != null)
    //         {
    //             grabInteractable.transform.position = interactorTransform.position + 
    //                                                 interactorTransform.forward * 0.05f;
    //         }

    //         // First release the current object
    //         interactionManager.SelectExit(interactor, this);
            
    //         // Wait a tiny bit
    //         yield return new WaitForSeconds(0.05f);
            
    //         // Then select the new food object
    //         interactionManager.SelectEnter(interactor, grabInteractable);
            
    //         Debug.Log("Transferred grab to: " + grabInteractable.name);
    //     }

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
        
        SpawnNewFood(args);
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

