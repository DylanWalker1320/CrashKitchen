using Unity.Netcode;
using UnityEngine;

public class PlatformTrigger : NetworkBehaviour
{
    public GameManager.RoleType platformType;
    private GameManager gmInstance;

    private void Start()
    {
        gmInstance = GameManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // Only the server assigns roles

        if (other.CompareTag("Player") && other.TryGetComponent(out NetworkObject netObj))
        {
            gmInstance.Log($"Player {netObj.OwnerClientId} entered platform");
            gmInstance.AssignPlayerToTruck(other.gameObject, platformType);

            // Ensure the trigger gets destroyed across all clients
            DestroyPlatformClientRpc();
        }
    }

    [ClientRpc]
    private void DestroyPlatformClientRpc()
    {
        Destroy(gameObject);
    }
}
