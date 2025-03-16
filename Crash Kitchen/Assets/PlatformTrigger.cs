using Unity.Netcode;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public GameManager.RoleType platformType;
    private GameManager gmInstance;

    void Start() {
        gmInstance = GameManager.Instance;
    }

    void OnTriggerEnter(Collider other)
    {
        gmInstance.Log("Player entered platform trigger");

        if (other.CompareTag("Player"))
        {
            gmInstance.AssignPlayerToTruck(other.gameObject, platformType);

            Destroy(gameObject);
        }
    }
}
