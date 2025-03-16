using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform driverTeleportPoint;
    public Transform cookTeleportPoint;
    public bool debugMode;
    public UnityEvent OnGameStart;
    private string debugLogPrefix = "<color=#FF4400>[GameManager]</color> ";
    public GameObject driver;
    public GameObject cook;
    private GameObject truck;

    public enum RoleType
    {
        Driver,
        Cook
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start() {
        truck = GameObject.FindWithTag("Truck");
    }

    public void AssignPlayerToTruck(GameObject player, RoleType role)
    {
        if (role == RoleType.Driver)
        {
            Log("Assigning player as driver");
            TeleportPlayer(player, driverTeleportPoint);
            ParentPlayerToTruck(player);
            driver = player;
        }
        else if (role == RoleType.Cook)
        {
            Log("Assigning player as cook");
            TeleportPlayer(player, cookTeleportPoint);
            ParentPlayerToTruck(player);
            cook = player;
        }

        CheckStartGame();
    }

    public void TeleportPlayer(GameObject player, Transform teleportPoint)
    {
        TeleportationProvider teleporter = player.GetComponentInChildren<TeleportationProvider>();

        if (teleporter != null)
        {
            Log("Teleporting player to " + teleportPoint.position);
            teleporter.QueueTeleportRequest(new TeleportRequest()
            {
                destinationPosition = teleportPoint.position,
                destinationRotation = teleportPoint.rotation,
                matchOrientation = MatchOrientation.TargetUp
            });
        }
    }
    
    private void CheckStartGame()
    {
        if (driver != null && cook != null)
        {
            Log("Both players are ready, starting game");
            OnGameStart.Invoke();
        }
    }

    private void ParentPlayerToTruck(GameObject player)
    {
        Log($"Parenting player to truck: {player.name}");
        player.transform.parent = truck.transform;
    }

    public void Log(string message)
    {
        if (debugMode)
        {
            Debug.Log(debugLogPrefix + message);
        }
    }
}
