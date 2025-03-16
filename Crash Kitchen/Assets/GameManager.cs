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
    
    // Use NetworkVariable to synchronize the player references
    public NetworkVariable<NetworkObjectReference> networkDriver = new NetworkVariable<NetworkObjectReference>();
    public NetworkVariable<NetworkObjectReference> networkCook = new NetworkVariable<NetworkObjectReference>();
    
    // Keep local references for convenience
    public GameObject driver;
    public GameObject cook;
    
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
        
        // Subscribe to NetworkVariable changes
        networkDriver.OnValueChanged += OnDriverChanged;
        networkCook.OnValueChanged += OnCookChanged;
    }
    
    private void OnDriverChanged(NetworkObjectReference previousValue, NetworkObjectReference newValue)
    {
        if (newValue.TryGet(out NetworkObject networkObject))
        {
            driver = networkObject.gameObject;
            Log("Driver reference updated on client");
        }
        CheckStartGame();
    }
    
    private void OnCookChanged(NetworkObjectReference previousValue, NetworkObjectReference newValue)
    {
        if (newValue.TryGet(out NetworkObject networkObject))
        {
            cook = networkObject.gameObject;
            Log("Cook reference updated on client");
        }
        CheckStartGame();
    }

    public void AssignPlayerToTruck(GameObject player, RoleType role)
    {
        // Call the server RPC to handle assignment
        AssignPlayerToTruckServerRpc(player.GetComponent<NetworkObject>(), (int)role);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void AssignPlayerToTruckServerRpc(NetworkObjectReference playerRef, int roleInt)
    {
        RoleType role = (RoleType)roleInt;
        
        if (!playerRef.TryGet(out NetworkObject playerNetObj))
            return;
            
        GameObject player = playerNetObj.gameObject;
        
        // Check if roles are already assigned to prevent duplicate assignments
        if (role == RoleType.Driver)
        {
            // Check if driver role is already taken by another player
            if (networkDriver.Value.TryGet(out NetworkObject existingDriver) && 
                existingDriver.NetworkObjectId != playerNetObj.NetworkObjectId)
            {
                Log($"Driver role is already taken by player {existingDriver.NetworkObjectId}");
                return;
            }
            
            Log("Assigning player as driver");
            TeleportPlayerServerRpc(playerRef, driverTeleportPoint.position, driverTeleportPoint.rotation);
            ParentPlayerToTruckServerRpc(playerRef);
            networkDriver.Value = playerRef;
        }
        else if (role == RoleType.Cook)
        {
            // Check if cook role is already taken by another player
            if (networkCook.Value.TryGet(out NetworkObject existingCook) && 
                existingCook.NetworkObjectId != playerNetObj.NetworkObjectId)
            {
                Log($"Cook role is already taken by player {existingCook.NetworkObjectId}");
                return;
            }
            
            Log("Assigning player as cook");
            TeleportPlayerServerRpc(playerRef, cookTeleportPoint.position, cookTeleportPoint.rotation);
            ParentPlayerToTruckServerRpc(playerRef);
            networkCook.Value = playerRef;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TeleportPlayerServerRpc(NetworkObjectReference playerRef, Vector3 position, Quaternion rotation)
    {
        if (!playerRef.TryGet(out NetworkObject playerNetObj))
            return;
            
        GameObject player = playerNetObj.gameObject;
        TeleportPlayerClientRpc(playerRef, position, rotation);
    }
    
    [ClientRpc]
    private void TeleportPlayerClientRpc(NetworkObjectReference playerRef, Vector3 position, Quaternion rotation)
    {
        if (!playerRef.TryGet(out NetworkObject playerNetObj))
            return;
            
        GameObject player = playerNetObj.gameObject;
        
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
    }
    
    private void CheckStartGame()
    {
        if (driver != null && cook != null && IsServer)
        {
            Log("Both players are ready, starting game");
            StartGameClientRpc();
        }
    }
    
    [ClientRpc]
    private void StartGameClientRpc()
    {
        OnGameStart.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    private void  ParentPlayerToTruckServerRpc(NetworkObjectReference playerRef)
    {
        if (!playerRef.TryGet(out NetworkObject playerNetObj))
            return;
            
        GameObject player = playerNetObj.gameObject;
        
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
