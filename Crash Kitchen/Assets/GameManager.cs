using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEngine.Events;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    public static GameObject truck;

    public NetworkVariable<ulong> player1Id = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<ulong> player2Id = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public UnityEvent onGameStart;
    private bool playersTeleported = false;

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
            Debug.LogError("Cannot find Truck object with tag 'Truck'");
        }
    }

    public void StartGame()
    {
        Debug.Log("Both players are assigned. Starting game...");
        StartGameServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        GameObject player1 = GetPlayerById(player1Id.Value);
        GameObject player2 = GetPlayerById(player2Id.Value);

        if (player1 == null || player2 == null)
        {
            Debug.LogError("One or both players could not be found!");
            return;
        }

        Debug.Log($"Teleporting {player1.name} to driver position");
        Debug.Log($"Teleporting {player2.name} to cook position");

        // Convert to world space positions before sending them to the clients
        Vector3 player1WorldPos = truck.transform.position + new Vector3(0f, 0.25f, -3.9f);
        Vector3 player2WorldPos = truck.transform.position + new Vector3(0f, 0.25f, 0f);

        // Teleport players using world space positions
        SetPlayerTransformClientRpc(player1.GetComponent<NetworkObject>().NetworkObjectId, player1WorldPos, Quaternion.Euler(0f, 180f, 0f));
        SetPlayerTransformClientRpc(player2.GetComponent<NetworkObject>().NetworkObjectId, player2WorldPos, Quaternion.Euler(0f, 270f, 0f));

        // Parent to truck
        Debug.Log("Parenting players to truck");
        ParentPlayersToTruck(player1, player2);

        // Freeze Y position
        Debug.Log("Freezing Y position for players");
        FreezeAllPlayersYPosition();
        onGameStart.Invoke();
    }

    private void FreezeAllPlayersYPosition()
    {
        foreach (var obj in FindObjectsByType<PlayerYLevelFreeze>(FindObjectsSortMode.None))
        {
            Debug.Log($"Freezing Y position for {obj.gameObject.name}");
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
        Debug.Log($"Setting transform for player with ID: {playerId}");
        GameObject player = GetPlayerById(playerId);
        if (player != null)
        {
            player.transform.position = position;  // Use world position instead of local
            player.transform.rotation = rotation;
        }
        Debug.Log($"Player {player.name} has been teleported to {position}");
    }

    public void Print() {
        Debug.Log("Invoked");
    }
}
