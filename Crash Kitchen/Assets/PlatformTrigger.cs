using Unity.Netcode;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public PlatformType platformType;
    private GameManager gmInstance;

    void Start() {
        gmInstance = GameManager.Instance;
    }

    void OnTriggerEnter(Collider other)
    {
        gmInstance.Log("Player entered platform trigger");

        if (other.CompareTag("Player"))
        {
            Transform teleportPosition = platformType == PlatformType.Driver ? gmInstance.driverTeleportPoint : gmInstance.cookTeleportPoint;

            gmInstance.TeleportPlayer(other.gameObject, teleportPosition);
        }
    }

    public enum PlatformType
    {
        Driver,
        Cook
    }
}
