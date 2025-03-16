using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public Transform driverTeleportPoint;
    public Transform cookTeleportPoint;
    public bool debugMode;
    private string debugLogPrefix = "<color=#FF4400>[GameManager]</color> ";

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

    public void Log(string message)
    {
        if (debugMode)
        {
            Debug.Log(debugLogPrefix + message);
        }
    }
}
