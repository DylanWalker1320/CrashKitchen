using Unity.Netcode;
using UnityEngine;

public class PlatformTrigger : NetworkBehaviour
{
    public GameManager.RoleType platformType;
    private GameManager gmInstance;
    private bool touched = false;

    void Start() {
        gmInstance = GameManager.Instance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // Only process this on the server
        
        // Make sure it's a player and has a NetworkObject
        if (other.CompareTag("Player"))
        {
            NetworkObject playerNetObj = other.GetComponentInParent<NetworkObject>();
            
            if (playerNetObj != null)
            {
                gmInstance.Log("Player entered platform trigger");
                gmInstance.AssignPlayerToTruck(playerNetObj, platformType);
                
                // Remove the platform when triggered
                DestroyPlatformClientRpc();
            }
        }
    }

    [ClientRpc]
    private void DestroyPlatformClientRpc()
    {
        Destroy(gameObject);
    }
}
