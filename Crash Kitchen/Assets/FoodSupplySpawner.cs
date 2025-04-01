using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


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
    
    protected override void Awake()
    {
        base.Awake();
        GameObject foodGameObject = GameObject.Find("Food");
        if (foodGameObject != null)
        {
            foodContainer = foodGameObject.transform;
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


            // Instantiate a food object
            GameObject spawnedFood = Instantiate(foodPrefab, spawnPosition, spawnRotation);
            Debug.Log("Spawned: "   + spawnedFood.name);

            AdjustFoodScale(spawnedFood);

            if (foodContainer != null)
            {
                spawnedFood.transform.SetParent(foodContainer);
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
                interactionManager.SelectEnter(interactor, grabInteractable);
            }

            interactionManager.SelectExit(interactor, this);

            canSpawn = false;
            Invoke(nameof(ResetSpawn), spawnCooldown);
        }
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
}

