using Unity.Netcode;
using UnityEngine;

public class PlatformTrigger : NetworkBehaviour
{
    public GameManager.RoleType platformType;
    private GameManager gmInstance;
    
    // Replace boolean with NetworkVariable
    public NetworkVariable<bool> touched = new NetworkVariable<bool>(false);

    void Start() {
        gmInstance = GameManager.Instance;
    }

    void OnTriggerEnter(Collider other)
    {
        gmInstance.Log("Player entered platform trigger");

        if (other.CompareTag("Player") && !touched.Value)
        {
            if (IsServer)
            {
                // Server can directly modify the NetworkVariable
                touched.Value = true;
                gmInstance.AssignPlayerToTruck(other.gameObject, platformType);
            }
            else
            {
                // Clients need to request the server to handle this
                SetTouchedServerRpc(other.GetComponent<NetworkObject>());
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetTouchedServerRpc(NetworkObjectReference playerRef)
    {
        // Double check to prevent race conditions
        if (touched.Value)
            return;
            
        touched.Value = true;
        
        if (playerRef.TryGet(out NetworkObject playerNetObj))
        {
            gmInstance.AssignPlayerToTruck(playerNetObj.gameObject, platformType);
        }
    }
}
