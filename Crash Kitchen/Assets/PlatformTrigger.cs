using UnityEngine;
using Unity.Netcode;

public class PlatformTrigger : NetworkBehaviour
{
    public GameManager gameManager;

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
            AssignPlayer(netObj.NetworkObjectId);
        }
        else
        {
            AssignPlayerServerRpc(netObj.NetworkObjectId, platformType == PlatformType.Driver);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AssignPlayerServerRpc(ulong playerId, bool isDriver)
    {
        AssignPlayer(playerId);
    }

    private void AssignPlayer(ulong playerId)
    {
        if (platformType == PlatformType.Driver)
        {
            gameManager.player1Id.Value = playerId;
            gameManager.isDriverPlatformEnabled = true;
        }
        else if (platformType == PlatformType.Cook)
        {
            gameManager.player2Id.Value = playerId;
            gameManager.isCookPlatformEnabled = true;
        }

        if (gameManager.isDriverPlatformEnabled && gameManager.isCookPlatformEnabled)
        {
            gameManager.StartCoroutine(gameManager.StartGameWithDelay());
        }
    }
}
