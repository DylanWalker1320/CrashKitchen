using UnityEngine;
using TMPro;  // Import TextMeshPro namespace

public class IncomingOrderGen : MonoBehaviour
{
    public TMP_Text orderText;  // Assign the TMP text object in the Inspector
    public Transform truck;  // Assign the truck GameObject in the Inspector

    private string selectedOrder;
    private Vector3 lastTruckPosition;  // Stores the last position of the truck

    void Start()
    {
        lastTruckPosition = truck.position;  // Save initial position
    }

    void Update()
    {
        // Check if the truck has moved
        if (truck.position != lastTruckPosition)
        {
            GenerateOrder();
            lastTruckPosition = truck.position;  // Update last position
        }
    }

    void GenerateOrder()
    {
        string[] orders = { "Mega Glizzy", "Healthy Burger", "Deluxe Steak" };
        selectedOrder = orders[Random.Range(0, orders.Length)];
        orderText.text = selectedOrder;
    }
}
