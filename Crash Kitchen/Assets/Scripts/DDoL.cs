using UnityEngine;
using Unity.Netcode;

public class DDoL : MonoBehaviour
{
    private void Awake()
    {
        if (Object.FindObjectsByType<NetworkManager>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
