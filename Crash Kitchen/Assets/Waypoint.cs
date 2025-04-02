using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Truck")
        {
            if(GameManager.instance.currentOrderDone)
            {
                WaypointManager.instance.SetNewWaypoint();
            }
        }
    }
}
