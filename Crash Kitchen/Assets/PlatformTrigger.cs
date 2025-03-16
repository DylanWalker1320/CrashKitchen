using Unity.Netcode;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public GameManager.RoleType platformType;
    private GameManager gmInstance;
    private bool touched = false;

    void Start() {
        gmInstance = GameManager.Instance;
    }

    void OnTriggerEnter(Collider other)
    {
        gmInstance.Log("Player entered platform trigger");

        if (other.CompareTag("Player") && !touched)
        {
            touched = true;
            gmInstance.AssignPlayerToTruck(other.gameObject, platformType);
        }
    }
}
