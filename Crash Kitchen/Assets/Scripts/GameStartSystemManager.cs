using UnityEngine;
using Unity.Netcode;

public class GameStartSystemManager : NetworkBehaviour
{
    private GameObject driverStartPlatform;
    private GameObject cookStartPlatform;
    private GameObject Truck;

    private Collider driverCollider;
    private Collider cookCollider;

    private bool isDriverOn = false;
    private bool isCookOn = false;
    
    // Reference to the CharacterController
    private CharacterController characterController;
    
    // Store the initial Y position when teleported
    private float fixedYPosition = 0f;
    private bool lockYPosition = false;

    void Start()
    {
        driverStartPlatform = GameObject.FindGameObjectWithTag("DriverPlatform");
        cookStartPlatform = GameObject.FindGameObjectWithTag("CookPlatform");
        Truck = GameObject.FindGameObjectWithTag("Truck");

        Debug.Log("GameStartSystemManager Start");
        
        driverCollider = driverStartPlatform.GetComponent<Collider>();
        cookCollider = cookStartPlatform.GetComponent<Collider>();
        
        // Get the CharacterController
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Add this code to enforce Y position locking
        if (IsOwner && lockYPosition && characterController != null)
        {
            // Get current position
            Vector3 currentPosition = transform.position;
            
            // If Y position has changed, reset it
            if (currentPosition.y != fixedYPosition)
            {
                // Create a new position with the fixed Y value
                Vector3 fixedPosition = new Vector3(currentPosition.x, fixedYPosition, currentPosition.z);
                
                // Move the character controller to the fixed position
                // We use a zero vector because we just want to reset position, not add movement
                characterController.enabled = false;
                transform.position = fixedPosition;
                characterController.enabled = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner){
            return;
        }

        if (other == driverCollider)
        {
            isDriverOn = true;
            Debug.Log("Player entered DriverStartPlatform");

            // Set the player as a child of the Truck
            transform.SetParent(Truck.transform);
            transform.localPosition = new Vector3(0f, 0.8f, -3.9f);
            transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            
            // // Lock the Y position
            if (characterController != null)
            {
                fixedYPosition = transform.position.y;
                lockYPosition = true;
            }
        }
        else if (other == cookCollider)
        {
            isCookOn = true;
            Debug.Log("Player entered CookStartPlatform");

            // Set the player as a child of the Truck
            transform.SetParent(Truck.transform);
            transform.localPosition = new Vector3(0f, 0.8f, 0f);
            transform.localRotation = Quaternion.Euler(0f, 270f, 0f);
            
            // // Lock the Y position
            if (characterController != null)
            {
                fixedYPosition = transform.position.y;
                lockYPosition = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner)
            return;

        if (other == driverCollider)
        {
            isDriverOn = false;
            Debug.Log("Player exited DriverStartPlatform");
        }
        else if (other == cookCollider)
        {
            isCookOn = false;
            Debug.Log("Player exited CookStartPlatform");
        }
    }
}