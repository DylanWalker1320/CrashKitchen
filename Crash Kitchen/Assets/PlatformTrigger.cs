using UnityEngine;
using Unity.Netcode;

public class PlatformTrigger : NetworkBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public enum PlatformType
    {
        Driver,
        Cook
    }

    public PlatformType platformType;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        NetworkObject netObj = other.GetComponent<NetworkObject>();
        if (netObj == null || !netObj.IsSpawned) return;

        Debug.Log($"Platform type touched!: {platformType}, Player: {other.name}");

        if (IsServer)
        {
            gameManager.SetPlayerServerRpc(netObj.NetworkObjectId, platformType == PlatformType.Driver);
        }
        else
        {
            AssignPlayerServerRpc(netObj.NetworkObjectId, platformType == PlatformType.Driver);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AssignPlayerServerRpc(ulong playerId, bool isDriver)
    {
        gameManager.SetPlayerServerRpc(playerId, isDriver);
    }
}
