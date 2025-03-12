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

        if (other.CompareTag("DriverPlatform") && !isDriverOn)
        {
            isDriverOn = true;
            Debug.Log("Player entered DriverStartPlatform");

            transform.SetParent(Truck.transform);
            transform.localPosition = new Vector3(0f, 0.8f, -3.9f);
            transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            // Lock the Y position
            if (characterController != null)
            {
                fixedYPosition = transform.position.y;
                lockYPosition = true;
            }
        }
        else if (other.CompareTag("CookPlatform") && !isCookOn)
        {
            isCookOn = true;
            Debug.Log("Player entered CookStartPlatform");

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
        if (!IsOwner) return;

        if (other.CompareTag("DriverPlatform") && isDriverOn)
        {
            isDriverOn = false;
            Debug.Log("Player exited DriverStartPlatform");

            // if (IsLocalPlayer)
            // {
            //     transform.SetParent(null); // Remove from truck when leaving
            // }
        }
        else if (other.CompareTag("CookPlatform") && isCookOn)
        {
            isCookOn = false;
            Debug.Log("Player exited CookStartPlatform");

            // if (IsLocalPlayer)
            // {
            //     transform.SetParent(null); // Remove from truck when leaving
            // }
        }
    }
}

// using UnityEngine;
// using Unity.Netcode;

// public class GameStartSystemManager : NetworkBehaviour
// {
//     [SerializeField] private GameObject Truck;
    
//     // Network variables to track player states
//     private NetworkVariable<bool> isDriverReady = new NetworkVariable<bool>(false);
//     private NetworkVariable<bool> isCookReady = new NetworkVariable<bool>(false);
    
//     // Track local player state
//     private bool isLocalPlayerOnDriverPlatform = false;
//     private bool isLocalPlayerOnCookPlatform = false;
    
//     // Reference to the CharacterController
//     private CharacterController characterController;
    
//     // Store the initial Y position when teleported
//     private float fixedYPosition = 0f;
//     private bool lockYPosition = false;
    
//     // Track which role this player has
//     private bool isDriver = false;
//     private bool isCook = false;

//     void Start()
//     {
//         if (Truck == null)
//         {
//             Truck = GameObject.FindGameObjectWithTag("Truck");
//         }
        
//         // Get the CharacterController
//         characterController = GetComponent<CharacterController>();
        
//         Debug.Log($"GameStartSystemManager Start for Player {OwnerClientId}");
//     }

//     void Update()
//     {
//         // Check if both players are ready
//         if (IsServer && isDriverReady.Value && isCookReady.Value)
//         {
//             // Both players are ready, teleport all players
//             TeleportPlayersServerRpc();
//         }
        
//         // Enforce Y position locking for this client
//         if (IsOwner && lockYPosition && characterController != null)
//         {
//             // Get current position
//             Vector3 currentPosition = transform.position;
            
//             // If Y position has changed, reset it
//             if (currentPosition.y != fixedYPosition)
//             {
//                 // Create a new position with the fixed Y value
//                 Vector3 fixedPosition = new Vector3(currentPosition.x, fixedYPosition, currentPosition.z);
                
//                 // Move the character controller to the fixed position
//                 characterController.enabled = false;
//                 transform.position = fixedPosition;
//                 characterController.enabled = true;
//             }
//         }
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         // Only process for the local player
//         if (!IsOwner) return;
        
//         if (other.CompareTag("DriverPlatform"))
//         {
//             Debug.Log($"Player {OwnerClientId} entered DriverStartPlatform");
//             isLocalPlayerOnDriverPlatform = true;
            
//             // Communicate to the server this player is on the driver platform
//             UpdatePlayerReadyStatusServerRpc(true, false);
//         }
//         else if (other.CompareTag("CookPlatform"))
//         {
//             Debug.Log($"Player {OwnerClientId} entered CookStartPlatform");
//             isLocalPlayerOnCookPlatform = true;
            
//             // Communicate to the server this player is on the cook platform
//             UpdatePlayerReadyStatusServerRpc(false, true);
//         }
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         // Only process for the local player
//         if (!IsOwner) return;
        
//         if (other.CompareTag("DriverPlatform"))
//         {
//             Debug.Log($"Player {OwnerClientId} exited DriverStartPlatform");
//             isLocalPlayerOnDriverPlatform = false;
            
//             // Tell the server this player is no longer on the driver platform
//             UpdatePlayerReadyStatusServerRpc(false, false);
//         }
//         else if (other.CompareTag("CookPlatform"))
//         {
//             Debug.Log($"Player {OwnerClientId} exited CookStartPlatform");
//             isLocalPlayerOnCookPlatform = false;
            
//             // Tell the server this player is no longer on the cook platform
//             UpdatePlayerReadyStatusServerRpc(false, false);
//         }
//     }
    
//     [ServerRpc(RequireOwnership = false)]
//     private void UpdatePlayerReadyStatusServerRpc(bool isDriverPlatform, bool isCookPlatform)
//     {
//         // This is called on the server to update player states
//         if (isDriverPlatform && !isDriverReady.Value)
//         {
//             // This player is the driver
//             isDriverReady.Value = true;
            
//             // Track which client is the driver (for teleporting later)
//             AssignRoleToPlayerClientRpc(true, false, new ClientRpcParams
//             {
//                 Send = new ClientRpcSendParams
//                 {
//                     TargetClientIds = new ulong[] { OwnerClientId }
//                 }
//             });
//         }
//         else if (isCookPlatform && !isCookReady.Value)
//         {
//             // This player is the cook
//             isCookReady.Value = true;
            
//             // Track which client is the cook (for teleporting later)
//             AssignRoleToPlayerClientRpc(false, true, new ClientRpcParams
//             {
//                 Send = new ClientRpcSendParams
//                 {
//                     TargetClientIds = new ulong[] { OwnerClientId }
//                 }
//             });
//         }
//         else if (!isDriverPlatform && !isCookPlatform)
//         {
//             // Reset status if player leaves either platform
//             if (isDriver)
//             {
//                 isDriverReady.Value = false;
//             }
//             else if (isCook)
//             {
//                 isCookReady.Value = false;
//             }
//         }
        
//         Debug.Log($"Updated status: Driver: {isDriverReady.Value}, Cook: {isCookReady.Value}");
//     }
    
//     [ClientRpc]
//     private void AssignRoleToPlayerClientRpc(bool isDriverRole, bool isCookRole, ClientRpcParams clientRpcParams = default)
//     {
//         isDriver = isDriverRole;
//         isCook = isCookRole;
        
//         Debug.Log($"Role assigned to player {OwnerClientId}: Driver={isDriver}, Cook={isCook}");
//     }
    
//     [ServerRpc]
//     private void TeleportPlayersServerRpc()
//     {
//         // Tell all clients to teleport based on their roles
//         TeleportPlayersClientRpc();
        
//         // Reset the ready flags to prevent multiple teleports
//         isDriverReady.Value = false;
//         isCookReady.Value = false;
//     }
    
//     [ClientRpc]
//     private void TeleportPlayersClientRpc()
//     {
//         if (!IsOwner) return;
        
//         // Set this player as a child of the truck
//         transform.SetParent(Truck.transform);
        
//         // Teleport based on role
//         if (isDriver)
//         {
//             Debug.Log($"Teleporting player {OwnerClientId} to driver position");
//             transform.localPosition = new Vector3(0f, 0.8f, -3.9f);
//             transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
//         }
//         else if (isCook)
//         {
//             Debug.Log($"Teleporting player {OwnerClientId} to cook position");
//             transform.localPosition = new Vector3(0f, 0.8f, 0f);
//             transform.localRotation = Quaternion.Euler(0f, 270f, 0f);
//         }
        
//         // Lock the Y position
//         if (characterController != null)
//         {
//             fixedYPosition = transform.position.y;
//             lockYPosition = true;
//         }
//     }
// }