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
    private NetworkVariable<ulong> driverId = new NetworkVariable<ulong>(ulong.MaxValue, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<ulong> cookId = new NetworkVariable<ulong>(ulong.MaxValue, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private GameObject truck;

    public enum RoleType
    {
        Driver,
        Cook
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        truck = GameObject.FindWithTag("Truck");
    }

    public void AssignPlayerToTruck(GameObject player, RoleType role)
    {
        if (!IsServer) return; // Ensure only the server assigns roles

        ulong playerId = player.GetComponent<NetworkObject>().OwnerClientId;
        Transform targetPoint = role == RoleType.Driver ? driverTeleportPoint : cookTeleportPoint;

        if (role == RoleType.Driver)
        {
            driverId.Value = playerId;
        }
        else
        {
            cookId.Value = playerId;
        }

        Log($"Assigning player {playerId} as {role}");
        TeleportPlayerClientRpc(playerId, targetPoint.position, targetPoint.rotation);

        if (driverId.Value != ulong.MaxValue && cookId.Value != ulong.MaxValue)
        {
            Log("Both players assigned. Starting game...");
            OnGameStart.Invoke();
        }
    }

    [ClientRpc]
    private void TeleportPlayerClientRpc(ulong playerId, Vector3 position, Quaternion rotation)
    {
        if (NetworkManager.Singleton.LocalClientId == playerId)
        {
            GameObject player = NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject.gameObject;
            TeleportationProvider teleporter = player.GetComponentInChildren<TeleportationProvider>();

            if (teleporter != null)
            {
                Log($"Teleporting {player.name} to {position}");
                teleporter.QueueTeleportRequest(new TeleportRequest
                {
                    destinationPosition = position,
                    destinationRotation = rotation,
                    matchOrientation = MatchOrientation.TargetUp
                });
            }

            // Parent to truck only after teleport
            player.transform.parent = truck.transform;
        }
    }

    public void Log(string message)
    {
        if (debugMode) Debug.Log(debugLogPrefix + message);
    }
}
