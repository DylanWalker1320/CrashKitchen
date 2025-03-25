using UnityEngine;

public class WaypointManager : MonoBehaviour
{

    public static WaypointManager instance;
    public Transform[] waypoints;
    public GameObject activeWaypoint;

    public GameObject waypointPrefab;
    public int waypointsCleared = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else { 
            Destroy(this); 
        }
    }

    void Start()
    {
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("Waypoint");
        waypoints = new Transform[waypointObjects.Length];

        for (int i = 0; i < waypointObjects.Length; i++)
        {
            waypoints[i] = waypointObjects[i].transform;
        }

        if (waypoints.Length > 0)
        {
            SetNewWaypoint();
        }
    }

    public void SetNewWaypoint()
    {
        // Clean up the old waypoint first
        if (activeWaypoint != null) {
            Destroy(activeWaypoint);
            waypointsCleared++;
        }

        // Get a random waypoint from the list
        Transform waypointPos = waypoints[Random.Range(0, waypoints.Length)];

        activeWaypoint = Instantiate(waypointPrefab, waypointPos.position, waypointPos.rotation);
    }

}
