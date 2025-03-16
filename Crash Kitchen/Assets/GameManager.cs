using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.Events;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public Transform driverTeleportPoint;
    public Transform cookTeleportPoint;
    public bool debugMode;
    public UnityEvent OnGameStart;
    private string debugLogPrefix = "<color=#FF4400>[GameManager]</color> ";
    public NetworkObject driver;
    public NetworkObject cook;
    private GameObject truck;

    public enum RoleType
    {
        Driver,
        Cook
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start() {
        truck = GameObject.FindWithTag("Truck");
    }

    // This method is called from PlatformTrigger
    public void AssignPlayerToTruck(NetworkObject playerNetObj, RoleType role)
    {
        // Only the server should process this logic
        if (IsServer)
        {
            ulong clientId = playerNetObj.OwnerClientId;
            
            if (role == RoleType.Driver)
            {
                Log("Assigning player as driver");
                TeleportPlayerServerRpc(clientId, driverTeleportPoint.position, driverTeleportPoint.rotation);
                ParentPlayerToTruckServerRpc(playerNetObj.NetworkObjectId);
                driver = playerNetObj;
            }
            else if (role == RoleType.Cook)
            {
                Log("Assigning player as cook");
                TeleportPlayerServerRpc(clientId, cookTeleportPoint.position, cookTeleportPoint.rotation);
                ParentPlayerToTruckServerRpc(playerNetObj.NetworkObjectId);
                cook = playerNetObj;
            }

            CheckStartGame();
        }
        else if (role == RoleType.Cook)
        {
            // If called on client, forward to server
            AssignPlayerToTruckServerRpc(playerNetObj.NetworkObjectId, role);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void AssignPlayerToTruckServerRpc(ulong networkObjectId, RoleType role)
    {
        NetworkObject playerNetObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        AssignPlayerToTruck(playerNetObj, role);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TeleportPlayerServerRpc(ulong clientId, Vector3 position, Quaternion rotation)
    {
        // Tell the specific client to teleport
        TeleportPlayerClientRpc(position, rotation, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        });
    }
    
    [ClientRpc]
    private void TeleportPlayerClientRpc(Vector3 position, Quaternion rotation, ClientRpcParams clientRpcParams = default)
    {
        // This runs on the client that needs to teleport
        if (NetworkManager.Singleton.LocalClientId == clientRpcParams.Send.TargetClientIds[0])
        {
            GameObject player = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;
            
            TeleportationProvider teleporter = player.GetComponentInChildren<TeleportationProvider>();
            if (teleporter != null)
            {
                Log("Teleporting player to " + position);
                teleporter.QueueTeleportRequest(new TeleportRequest()
                {
                    destinationPosition = position,
                    destinationRotation = rotation,
                    matchOrientation = MatchOrientation.TargetUp
                });
            }
            else
            {
                Log($"No TeleportationProvider found, forcing position update.");
                player.transform.position = position;
                player.transform.rotation = rotation;
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ParentPlayerToTruckServerRpc(ulong networkObjectId)
    {
        // Get the NetworkObject with the given ID
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject playerNetObj))
        {
            // Parent the player to the truck on the server
            Log($"Parenting player to truck: {playerNetObj.name}");
            
            // Tell all clients about this parent change (including the server)
            ParentPlayerToTruckClientRpc(networkObjectId);
        }
    }
    
    [ClientRpc]
    private void ParentPlayerToTruckClientRpc(ulong networkObjectId)
    {
        // Get the NetworkObject with the given ID
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject playerNetObj))
        {
            // Apply parenting on all clients
            playerNetObj.transform.parent = truck.transform;
        }
    }
    
    private void CheckStartGame()
    {
        if (driver != null && cook != null)
        {
            Log("Both players are ready, starting game");
            OnGameStart.Invoke();
        }
    }

    private void ParentPlayerToTruck(GameObject player)
    {
        Log($"Parenting player to truck: {player.name}");
        player.transform.parent = truck.transform;
    }

    public void Log(string message)
    {
        if (debugMode)
        {
            Debug.Log(debugLogPrefix + message);
        }
    }
}