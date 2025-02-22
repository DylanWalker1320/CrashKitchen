using UnityEngine;

public class GameStartSystemManager : MonoBehaviour
{
    public GameObject driverStartPlatform;
    public GameObject cookStartPlatform;
    public GameObject Car;

    private Collider driverCollider;
    private Collider cookCollider;

    private bool isDriverOn = false;
    private bool isCookOn = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        driverCollider = driverStartPlatform.GetComponent<Collider>();
        cookCollider = cookStartPlatform.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == driverCollider)
        {
            isDriverOn = true;
            Debug.Log("Player entered DriverStartPlatform");

            // Set the player as a child of the car
            transform.SetParent(Car.transform);
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localRotation = Quaternion.identity;
        }
        else if (other == cookCollider)
        {
            isCookOn = true;
            Debug.Log("Player entered CookStartPlatform");
        }
    }

    private void OnTriggerExit(Collider other)
    {
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