using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    public static GameObject truck;

    public NetworkVariable<ulong> player1Id = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<ulong> player2Id = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public bool isDriverPlatformEnabled;
    public bool isCookPlatformEnabled;

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

    // This runs for 5 seconds.
    public IEnumerator StartGameWithDelay()
    {
        float timeout = 5f;
        float elapsedTime = 0f;

        while ((player1Id.Value == 0 || player2Id.Value == 0) && elapsedTime < timeout)
        {
            Debug.Log("Waiting for players to be assigned...");
            yield return new WaitForSeconds(0.5f);
            elapsedTime += 0.5f;
        }

        if (player1Id.Value == 0 || player2Id.Value == 0)
        {
            Debug.LogError("Players could not be assigned in time!");
            yield break;
        }

        StartGame();
    }

    private void StartGame()
    {
        if (!IsServer) return;

        GameObject player1 = GetPlayerById(player1Id.Value);
        GameObject player2 = GetPlayerById(player2Id.Value);

        if (player1 == null || player2 == null)
        {
            Debug.LogError("One or both players could not be found!");
            return;
        }

        Debug.Log($"Teleporting {player1.name} to driver position");
        Debug.Log($"Teleporting {player2.name} to cook position");

        player1.transform.SetParent(truck.transform, true);
        player2.transform.SetParent(truck.transform, true);

        SetPlayerTransformServerRpc(player1.GetComponent<NetworkObject>().NetworkObjectId, new Vector3(0f, 0.8f, -3.9f), Quaternion.Euler(0f, 180f, 0f));
        SetPlayerTransformServerRpc(player2.GetComponent<NetworkObject>().NetworkObjectId, new Vector3(0f, 0.8f, 0f), Quaternion.Euler(0f, 270f, 0f));
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
        if (isDriver)
        {
            player1Id.Value = networkId;
        }
        else
        {
            player2Id.Value = networkId;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerTransformServerRpc(ulong playerId, Vector3 position, Quaternion rotation)
    {
        GameObject player = GetPlayerById(playerId);
        if (player != null)
        {
            player.transform.localPosition = position;
            player.transform.localRotation = rotation;
        }
    }
}
