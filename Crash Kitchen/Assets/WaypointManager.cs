using Unity.Netcode;
using UnityEngine;

public class WaypointManager : NetworkBehaviour
{
    public static WaypointManager instance;
    public Transform[] waypoints;
    public GameObject waypointPrefab;
    public NetworkObject activeWaypoint;
    public int waypointsCleared = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Init()
    {
        if (!IsServer) return; // Only the server should handle waypoint setup

        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("Waypoint");
        waypoints = new Transform[waypointObjects.Length];

        for (int i = 0; i < waypointObjects.Length; i++)
        {
            waypoints[i] = waypointObjects[i].transform;
        }

        if (waypoints.Length > 0)
        {
            SetNewWaypointServerRpc();
        }
    }

    [ServerRpc]
    public void SetNewWaypointServerRpc()
    {
        if (activeWaypoint != null)
        {
            activeWaypoint.Despawn();
            waypointsCleared++;
            GameManager.instance.NewOrder();
        }

        Transform waypointPos = waypoints[Random.Range(0, waypoints.Length)];
        GameObject newWaypoint = Instantiate(waypointPrefab, waypointPos.position, waypointPos.rotation);
        activeWaypoint = newWaypoint.GetComponent<NetworkObject>();
        activeWaypoint.Spawn();
    }
}
