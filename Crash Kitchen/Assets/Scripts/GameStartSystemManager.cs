using UnityEngine;
using Unity.Netcode;

public class GameStartSystemManager : NetworkBehaviour
{
    private GameObject driverStartPlatform;
    private GameObject cookStartPlatform;
    private GameObject Truck;

    private Collider driverCollider;
    private Collider cookCollider;

    private bool isDriverOn = false;
    private bool isCookOn = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if (!NetworkManager.Singleton.IsListening)
        // {
        //     Debug.LogWarning("NetworkManager is not listening. Start a server or host before reparenting.");
        //     return;
        // }

        // NEED to start a server or host before reparenting otherwise u get teleported to 0 0 0 platform

        driverStartPlatform = GameObject.FindGameObjectWithTag("DriverPlatform");
        cookStartPlatform = GameObject.FindGameObjectWithTag("CookPlatform");
        Truck = GameObject.FindGameObjectWithTag("Truck");

        Debug.Log("GameStartSystemManager Start");
        // if (!IsOwner)
        //     return;
        
        // Teleport the player to the start platform
        // Debug.Log("Teleporting player to start platform");
        // transform.position = new Vector3(-460f, 1.5f, 65f);
        driverCollider = driverStartPlatform.GetComponent<Collider>();
        cookCollider = cookStartPlatform.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // if (!IsOwner)
        //     return;
        
        if (other == driverCollider)
        {
            isDriverOn = true;
            Debug.Log("Player entered DriverStartPlatform");

            // Set the player as a child of the Truck
            transform.SetParent(Truck.transform);
            transform.localPosition = new Vector3(0f, 1f, -3.9f);
            transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if (other == cookCollider)
        {
            isCookOn = true;
            Debug.Log("Player entered CookStartPlatform");

            // Set the player as a child of the Truck
            transform.SetParent(Truck.transform);
            transform.localPosition = new Vector3(0f, 0f, 0f);
            transform.localRotation = Quaternion.Euler(0f, 270f, 0f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // if (!IsOwner)
        //     return;

        if (other == driverCollider)
        {
            isDriverOn = false;
            Debug.Log("Player exited DriverStartPlatform");
        }
        else if (other == cookCollider)
        {
            isCookOn = false;
            Debug.Log("Player exited CookStartPlatform");
        }
    }
}