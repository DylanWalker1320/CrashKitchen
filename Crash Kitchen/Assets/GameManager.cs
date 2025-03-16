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
        
        // truck.GetComponent<NetworkObject>().ChangeOwnership(NetworkManager.ServerClientId);
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
        } else {
            if (debugMode) Debug.Log($"<color={debugLogHex}>Players found!... they are {player1.name} and {player2.name}</color>");
        }

        // Parent to truck
        if (debugMode) Debug.Log($"<color={debugLogHex}>Parenting players to truck</color>");
        ParentPlayersToTruck(player1, player2);

        // Define local positions with more distinct separation
        Vector3 player1LocalPos = new Vector3(0f, 0.25f, -5f);
        Vector3 player2LocalPos = new Vector3(0f, 0.25f, 0f);

        // Define relative rotations (-z is forward)
        Quaternion playerRotation = Quaternion.Euler(0f, 180f, 0f); // Rotate 180 degrees about Y axis

        // Ensure positions are set after parenting is complete
        StartCoroutine(DelayedPositionSet(player1, player2, player1LocalPos, player2LocalPos, playerRotation));
    }

    private IEnumerator DelayedPositionSet(GameObject player1, GameObject player2, Vector3 pos1, Vector3 pos2, Quaternion rotation)
    {
        // Wait for 5 seconds before setting positions
        yield return new WaitForSeconds(5f);
        if (debugMode) Debug.Log($"<color={debugLogHex}>Setting player positions</color>");

        // Set positions directly on server first
        player1.transform.localPosition = pos1;
        player1.transform.localRotation = rotation;
        player2.transform.localPosition = pos2;
        player2.transform.localRotation = rotation;

        // Then synchronize to clients
        SetPlayerTransformClientRpc(player1Id.Value, pos1, rotation);
        SetPlayerTransformClientRpc(player2Id.Value, pos2, rotation);
        
        // Invoke game start event
        if (debugMode) Debug.Log($"<color={debugLogHex}>Invoking the start event</color>");
        onGameStart.Invoke();
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
            if (obj.CompareTag("ShadowPlayer")) continue; // Ignore shadow players
            
            if ((obj.NetworkObjectId == networkId && obj.IsSpawned) || 
                (obj.CompareTag("Player") && obj.GetComponentInChildren<Camera>() != null))
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
        GameObject player = GetPlayerById(playerId);
        
        if (player != null)
        {
            // Double-check that we have the right player
            NetworkObject netObj = player.GetComponent<NetworkObject>();
            if (netObj && netObj.NetworkObjectId == playerId)
            {
                if (debugMode) Debug.Log($"<color={debugLogHex}>Teleporting player {player.name} to {position}</color>");
                player.transform.localPosition = position;
                player.transform.localRotation = rotation;
            }
        }
        else
        {
            if (debugMode) Debug.LogError($"<color={debugErrorHex}>Failed to find player with ID {playerId} for teleport</color>");
        }
    }
}
