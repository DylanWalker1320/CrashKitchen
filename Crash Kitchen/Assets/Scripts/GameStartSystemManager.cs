// using UnityEngine;
// using Unity.Netcode;

// public class GameStartSystemManager : NetworkBehaviour
// {
//     private GameObject driverStartPlatform;
//     private GameObject cookStartPlatform;
//     private GameObject Truck;

//     private Collider driverCollider;
//     private Collider cookCollider;

//     private bool isDriverOn = false;
//     private bool isCookOn = false;
    
//     // Reference to the CharacterController
//     private CharacterController characterController;
    
//     // Store the initial Y position when teleported
//     private float fixedYPosition = 0f;
//     private bool lockYPosition = false;

//     void Start()
//     {
//         driverStartPlatform = GameObject.FindGameObjectWithTag("DriverPlatform");
//         cookStartPlatform = GameObject.FindGameObjectWithTag("CookPlatform");
//         Truck = GameObject.FindGameObjectWithTag("Truck");

//         Debug.Log("GameStartSystemManager Start");
        
//         driverCollider = driverStartPlatform.GetComponent<Collider>();
//         cookCollider = cookStartPlatform.GetComponent<Collider>();
        
//         // Get the CharacterController
//         characterController = GetComponent<CharacterController>();
//     }

//     void Update()
//     {
//         // Add this code to enforce Y position locking
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
//                 // We use a zero vector because we just want to reset position, not add movement
//                 characterController.enabled = false;
//                 transform.position = fixedPosition;
//                 characterController.enabled = true;
//             }
//         }
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (!IsOwner){
//             return;
//         }

//         if (other.CompareTag("DriverPlatform") && !isDriverOn)
//         {
//             isDriverOn = true;
//             Debug.Log("Player entered DriverStartPlatform");

//             transform.SetParent(Truck.transform);
//             transform.localPosition = new Vector3(0f, 0.8f, -3.9f);
//             transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

//             // Lock the Y position
//             if (characterController != null)
//             {
//                 fixedYPosition = transform.position.y;
//                 lockYPosition = true;
//             }
//         }
//         else if (other.CompareTag("CookPlatform") && !isCookOn)
//         {
//             isCookOn = true;
//             Debug.Log("Player entered CookStartPlatform");

//             transform.SetParent(Truck.transform);
//             transform.localPosition = new Vector3(0f, 0.8f, 0f);
//             transform.localRotation = Quaternion.Euler(0f, 270f, 0f);

//             // // Lock the Y position
//             if (characterController != null)
//             {
//                 fixedYPosition = transform.position.y;
//                 lockYPosition = true;
//             }
//         }
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (!IsOwner) return;

//         if (other.CompareTag("DriverPlatform") && isDriverOn)
//         {
//             isDriverOn = false;
//             Debug.Log("Player exited DriverStartPlatform");

//             // if (IsLocalPlayer)
//             // {
//             //     transform.SetParent(null); // Remove from truck when leaving
//             // }
//         }
//         else if (other.CompareTag("CookPlatform") && isCookOn)
//         {
//             isCookOn = false;
//             Debug.Log("Player exited CookStartPlatform");

//             // if (IsLocalPlayer)
//             // {
//             //     transform.SetParent(null); // Remove from truck when leaving
//             // }
//         }
//     }
// }

using UnityEngine;
using Unity.Netcode;

public class GameStartSystemManager : NetworkBehaviour
{
    // [SerializeField] private GameObject Truck;
    
    // // Network variables to track player states
    // private NetworkVariable<int> testValue = new NetworkVariable<int>(
    //     0,
    //     NetworkVariableReadPermission.Everyone,
    //     NetworkVariableWritePermission.Owner
    // );
    // private NetworkVariable<bool> isDriverReady = new NetworkVariable<bool>(false);
    // private NetworkVariable<bool> isCookReady = new NetworkVariable<bool>(false);
    
    // // Track which client is on which platform
    // private NetworkVariable<ulong> driverClientId = new NetworkVariable<ulong>(ulong.MaxValue);
    // private NetworkVariable<ulong> cookClientId = new NetworkVariable<ulong>(ulong.MaxValue);
    
    // // Track if teleportation has happened
    // private NetworkVariable<bool> hasTeleported = new NetworkVariable<bool>(false);
    
    // // Reference to the CharacterController
    // private CharacterController characterController;
    
    // // Store the initial Y position when teleported
    // private float fixedYPosition = 0f;
    // private bool lockYPosition = false;

    // // Debug keystroke (F1 key)
    // [SerializeField] private KeyCode debugKey = KeyCode.F1;


    // void Start()
    // {
    //     if (Truck == null)
    //     {
    //         Truck = GameObject.FindGameObjectWithTag("Truck");
    //         if (Truck == null)
    //         {
    //             Debug.LogError("Cannot find Truck object with tag 'Truck'");
    //         }
    //     }
        
    //     // Get the CharacterController
    //     characterController = GetComponent<CharacterController>();
        
    //     Debug.Log($"GameStartSystemManager Start for Player {OwnerClientId}");
    // }

    // void Update()
    // {
    //     Debug.Log(OwnerClientId + "; " + testValue.Value);

    //     if(!IsOwner) return;

    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         testValue.Value++;
    //     }

    //     if (Input.GetKeyDown(debugKey))
    //     {
    //         LogDebugInfo();
    //     }

    //     // Check if both players are ready and teleportation hasn't occurred yet
    //     if (IsServer && isDriverReady.Value && isCookReady.Value && !hasTeleported.Value)
    //     {
    //         Debug.Log("Both players are ready. Initiating teleportation...");
            
    //         // Mark that teleportation has occurred to prevent multiple teleports
    //         hasTeleported.Value = true;
            
    //         // Both players are ready, teleport all players
    //         TeleportPlayersClientRpc();
    //     }
        
    //     // Enforce Y position locking for this client
    //     if (IsOwner && lockYPosition && characterController != null)
    //     {
    //         // Get current position
    //         Vector3 currentPosition = transform.position;
            
    //         // If Y position has changed, reset it
    //         if (currentPosition.y != fixedYPosition)
    //         {
    //             // Create a new position with the fixed Y value
    //             Vector3 fixedPosition = new Vector3(currentPosition.x, fixedYPosition, currentPosition.z);
                
    //             // Move the character controller to the fixed position
    //             characterController.enabled = false;
    //             transform.position = fixedPosition;
    //             characterController.enabled = true;
    //         }
    //     }
    // }

    // private void LogDebugInfo()
    // {
    //     Debug.Log("========== GAME START SYSTEM DEBUG INFO ==========");
    //     Debug.Log($"NETWORK: IsOwner={IsOwner}, IsServer={IsServer}, OwnerClientId={OwnerClientId}");
    //     Debug.Log($"STATE: isDriverReady={isDriverReady.Value}, isCookReady={isCookReady.Value}, hasTeleported={hasTeleported.Value}");
    //     Debug.Log($"ASSIGNMENTS: driverClientId={driverClientId.Value}, cookClientId={cookClientId.Value}");
    //     // Debug.Log($"POSITIONING: lockYPosition={lockYPosition}, fixedYPosition={fixedYPosition}");
    //     // Debug.Log($"CURRENT POSITION: {transform.position}, Parent: {(transform.parent ? transform.parent.name : "None")}");
    //     // Debug.Log($"REFERENCES: Truck={Truck != null}, characterController={characterController != null}");
        
    //     // Check if Truck has NetworkObject
    //     if (Truck != null)
    //     {
    //         NetworkObject truckNetObj = Truck.GetComponent<NetworkObject>();
    //         Debug.Log($"TRUCK NETWORK: HasNetworkObject={truckNetObj != null}, " + (truckNetObj != null ? $"IsSpawned={truckNetObj.IsSpawned}" : ""));
    //     }
        
    //     // Check if platforms exist with proper tags
    //     GameObject driverPlatform = GameObject.FindGameObjectWithTag("DriverPlatform");
    //     GameObject cookPlatform = GameObject.FindGameObjectWithTag("CookPlatform");
    //     Debug.Log($"PLATFORMS: DriverPlatform={driverPlatform != null}, CookPlatform={cookPlatform != null}");
        
    //     if (driverPlatform != null)
    //     {
    //         Collider col = driverPlatform.GetComponent<Collider>();
    //         Debug.Log($"DRIVER PLATFORM: HasCollider={col != null}, " + (col != null ? $"IsTrigger={col.isTrigger}" : ""));
    //     }
        
    //     if (cookPlatform != null)
    //     {
    //         Collider col = cookPlatform.GetComponent<Collider>();
    //         Debug.Log($"COOK PLATFORM: HasCollider={col != null}, " + (col != null ? $"IsTrigger={col.isTrigger}" : ""));
    //     }
        
    //     Debug.Log("================================================");
    // }

    // private void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log($"Trigger detected: Player {OwnerClientId} entered {other.gameObject.name} with tag {other.tag}");

    //     // Only process for the local player
    //     if (!IsOwner) return;
        
    //     if (other.CompareTag("DriverPlatform"))
    //     {
    //         Debug.Log($"Player {OwnerClientId} entered DriverStartPlatform");
            
    //         // Communicate to the server this player is on the driver platform
    //         UpdateDriverPlatformStatusServerRpc(true);
    //     }
    //     else if (other.CompareTag("CookPlatform"))
    //     {
    //         Debug.Log($"Player {OwnerClientId} entered CookStartPlatform");
            
    //         // Communicate to the server this player is on the cook platform
    //         UpdateCookPlatformStatusServerRpc(true);
    //     }
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     // Only process for the local player
    //     if (!IsOwner) return;
        
    //     // Only handle exits if teleportation hasn't occurred yet
    //     if (hasTeleported.Value) return;
        
    //     if (other.CompareTag("DriverPlatform"))
    //     {
    //         Debug.Log($"Player {OwnerClientId} exited DriverStartPlatform");
            
    //         // Tell the server this player is no longer on the driver platform
    //         UpdateDriverPlatformStatusServerRpc(false);
    //     }
    //     else if (other.CompareTag("CookPlatform"))
    //     {
    //         Debug.Log($"Player {OwnerClientId} exited CookStartPlatform");
            
    //         // Tell the server this player is no longer on the cook platform
    //         UpdateCookPlatformStatusServerRpc(false);
    //     }
    // }
    
    // [ServerRpc(RequireOwnership = false)]
    // private void UpdateDriverPlatformStatusServerRpc(bool isOnPlatform)
    // {
    //     if (isOnPlatform)
    //     {
    //         isDriverReady.Value = true;
    //         driverClientId.Value = OwnerClientId;
    //         Debug.Log($"Server registered Player {OwnerClientId} as Driver. Driver ready: {isDriverReady.Value}, Cook ready: {isCookReady.Value}");
    //     }
    //     else if (driverClientId.Value == OwnerClientId) // Only reset if this client was the driver
    //     {
    //         isDriverReady.Value = false;
    //         driverClientId.Value = ulong.MaxValue;
    //         Debug.Log($"Server unregistered Player {OwnerClientId} as Driver. Driver ready: {isDriverReady.Value}, Cook ready: {isCookReady.Value}");
    //     }
    // }
    
    // [ServerRpc(RequireOwnership = false)]
    // private void UpdateCookPlatformStatusServerRpc(bool isOnPlatform)
    // {
    //     if (isOnPlatform)
    //     {
    //         isCookReady.Value = true;
    //         cookClientId.Value = OwnerClientId;
    //         Debug.Log($"Server registered Player {OwnerClientId} as Cook. Driver ready: {isDriverReady.Value}, Cook ready: {isCookReady.Value}");
    //     }
    //     else if (cookClientId.Value == OwnerClientId) // Only reset if this client was the cook
    //     {
    //         isCookReady.Value = false;
    //         cookClientId.Value = ulong.MaxValue;
    //         Debug.Log($"Server unregistered Player {OwnerClientId} as Cook. Driver ready: {isDriverReady.Value}, Cook ready: {isCookReady.Value}");
    //     }
    // }
    
    // [ClientRpc]
    // private void TeleportPlayersClientRpc()
    // {
    //     if (!IsOwner) return;
        
    //     // Set this player as a child of the truck
    //     transform.SetParent(Truck.transform);
        
    //     // Teleport based on client ID
    //     if (OwnerClientId == driverClientId.Value)
    //     {
    //         Debug.Log($"Teleporting player {OwnerClientId} to driver position");
    //         transform.localPosition = new Vector3(0f, 0.8f, -3.9f);
    //         transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
    //     }
    //     else if (OwnerClientId == cookClientId.Value)
    //     {
    //         Debug.Log($"Teleporting player {OwnerClientId} to cook position");
    //         transform.localPosition = new Vector3(0f, 0.8f, 0f);
    //         transform.localRotation = Quaternion.Euler(0f, 270f, 0f);
    //     }
    //     else
    //     {
    //         Debug.LogWarning($"Player {OwnerClientId} has no assigned role but is being teleported");
    //         // Fallback teleport position
    //         transform.localPosition = new Vector3(0f, 0.8f, 2f);
    //         transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    //     }
        
    //     // Lock the Y position
    //     if (characterController != null)
    //     {
    //         fixedYPosition = transform.position.y;
    //         lockYPosition = true;
    //         Debug.Log($"Y position locked for player {OwnerClientId} at {fixedYPosition}");
    //     }
    // }
    
    // // Method to reset the game state (can be called from another script or event)
    // public void ResetGameState()
    // {
    //     if (IsServer)
    //     {
    //         isDriverReady.Value = false;
    //         isCookReady.Value = false;
    //         driverClientId.Value = ulong.MaxValue;
    //         cookClientId.Value = ulong.MaxValue;
    //         hasTeleported.Value = false;
    //         Debug.Log("Game state has been reset");
    //     }
    //     else
    //     {
    //         // Client request to server to reset
    //         ResetGameStateServerRpc();
    //     }
    // }
    
    // [ServerRpc(RequireOwnership = false)]
    // private void ResetGameStateServerRpc()
    // {
    //     ResetGameState();
    // }
}