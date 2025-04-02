using Unity.Netcode;
using UnityEngine;

public class Waypoint : NetworkBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // Ensure only the server processes collisions

        if (other.CompareTag("Truck"))
        {
            if (GameManager.instance.currentOrderDone)
            {
                WaypointManager.instance.SetNewWaypointServerRpc();
            }
        }
    }
}
