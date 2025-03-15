using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEngine.Events;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    public static GameObject truck;

    private NetworkVariable<ulong> player1Id = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<ulong> player2Id = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public UnityEvent onGameStart;
    private bool playersTeleported = false;

    public bool debugMode = false;
    private string debugErrorHex = "#FF0000";
    private string debugLogHex = "#FFaa55";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        truck = GameObject.FindGameObjectWithTag("Truck");
        
        if (truck == null)
        {
            if (debugMode) Debug.LogError($"<color={debugErrorHex}>Truck not found!</color>");
        }
        
        truck.GetComponent<NetworkObject>().ChangeOwnership(NetworkManager.ServerClientId);
    }

    public void StartGame()
    {
        if (debugMode) Debug.Log($"<color={debugLogHex}>Starting game...</color>");
        StartGameServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        GameObject player1 = GetPlayerById(player1Id.Value);
        GameObject player2 = GetPlayerById(player2Id.Value);

        if (player1 == null || player2 == null)
        {
            if (debugMode) Debug.LogError($"<color={debugErrorHex}>Players not found!</color>");
            return;
        }

        if (debugMode) Debug.Log($"<color={debugLogHex}>Teleporting {player1.name} to driver position</color>");
        if (debugMode) Debug.Log($"<color={debugLogHex}>Teleporting {player2.name} to cook position</color>");

        // Convert to world space positions before sending them to the clients
        Vector3 player1WorldPos = truck.transform.position + new Vector3(0f, 0.25f, -3.9f);
        Vector3 player2WorldPos = truck.transform.position + new Vector3(0f, 0.25f, 0f);

        // Teleport players using world space positions
        SetPlayerTransformClientRpc(player1.GetComponent<NetworkObject>().NetworkObjectId, player1WorldPos, Quaternion.Euler(0f, 180f, 0f));
        SetPlayerTransformClientRpc(player2.GetComponent<NetworkObject>().NetworkObjectId, player2WorldPos, Quaternion.Euler(0f, 270f, 0f));

        // Parent to truck
        if (debugMode) Debug.Log($"<color={debugLogHex}>Parenting players to truck</color>");
        ParentPlayersToTruck(player1, player2);

        // Freeze Y position
        if (debugMode) Debug.Log($"<color={debugLogHex}>Freezing Y positions</color>");
        //FreezeAllPlayersYPosition();
        onGameStart.Invoke();
    }

    private void FreezeAllPlayersYPosition()
    {
        foreach (var obj in FindObjectsByType<PlayerYLevelFreeze>(FindObjectsSortMode.None))
        {
            if (debugMode) Debug.Log($"<color={debugLogHex}>Freezing Y position for {obj.gameObject.name}</color>");
            obj.Freeze();
        }
    }

    private void ParentPlayersToTruck(GameObject player1, GameObject player2)
    {
        if (player1 != null) player1.transform.SetParent(truck.transform, true);
        if (player2 != null) player2.transform.SetParent(truck.transform, true);
    }

    private GameObject GetPlayerById(ulong networkId)
    {
        foreach (var obj in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            if (obj.NetworkObjectId == networkId)
            {
                return obj.gameObject;
            }
        }
        return null;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerServerRpc(ulong networkId, bool isDriver)
    {
        if (!IsServer) return;

        if (isDriver)
        {
            player1Id.Value = networkId;
        }
        else
        {
            player2Id.Value = networkId;
        }

        if (player1Id.Value != 0 && player2Id.Value != 0)
        {
            StartGame();
        }
    }

    [ClientRpc]
    private void SetPlayerTransformClientRpc(ulong playerId, Vector3 position, Quaternion rotation)
    {
        if (debugMode) Debug.Log($"<color={debugLogHex}>Setting transform for player {playerId}</color>");
        GameObject player = GetPlayerById(playerId);
        if (player != null)
        {
            player.transform.position = position;  // Use world position instead of local
            player.transform.rotation = rotation;
        }
        if (debugMode) Debug.Log($"<color={debugLogHex}>Transform set for player {playerId}</color>");
    }
}
