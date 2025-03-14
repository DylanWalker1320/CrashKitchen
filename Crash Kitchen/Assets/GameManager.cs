using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool isDriverPlatformEnabled = false;
    public static bool isCookPlatformEnabled = false;

    public static GameObject player1; // Driver
    public static GameObject player2; // Cook
    public static GameManager instance;
    public static GameObject truck;

    public void Start()
    {
        Debug.Log("GameManager Start()");
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        // Get Truck
        if (truck == null)
        {
            truck = GameObject.FindGameObjectWithTag("Truck");
            if (truck == null)
            {
                Debug.LogError("Cannot find Truck object with tag 'Truck'");
            }
        }
    }

    private void StartGame(){
        // Player 1
        Debug.Log($"Teleporting player1: {player1.name} to driver position");
        player1.transform.SetParent(truck.transform);
        player1.transform.localPosition = new Vector3(0f, 0.8f, -3.9f);
        player1.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        // Player 2
        Debug.Log($"Teleporting player2: {player2.name} to cook position");
        player2.transform.SetParent(truck.transform);
        player2.transform.localPosition = new Vector3(0f, 0.8f, 0f);
        player2.transform.localRotation = Quaternion.Euler(0f, 270f, 0f);
    }

    public void Update()
    {
        Debug.Log($"Driver: {isDriverPlatformEnabled}, Cook: {isCookPlatformEnabled}");
        if (isDriverPlatformEnabled && isCookPlatformEnabled)
        {
            StartGame();
        }
    }
}
