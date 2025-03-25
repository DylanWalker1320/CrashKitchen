using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Truck")
        {
            WaypointManager.instance.SetNewWaypoint();
        }
    }
}
