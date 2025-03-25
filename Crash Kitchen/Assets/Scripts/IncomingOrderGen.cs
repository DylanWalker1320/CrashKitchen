using UnityEngine;
using TMPro;

public class IncomingOrderGen : MonoBehaviour
{
    public TMP_Text orderText;  
    public Transform truck;  

    private string selectedOrder;
    private Vector3 lastTruckPosition; 
    private bool orderGenerated = false;  

    void Start()
    {
        lastTruckPosition = truck.position; 
    }

    void Update()
    {

        if (!orderGenerated && truck.position != lastTruckPosition)
        {
            GenerateOrder();
            orderGenerated = true;  
        }
    }

    void GenerateOrder()
    {
        string[] orders = { "Mega Glizzy", "Healthy Burger", "Deluxe Steak" };
        selectedOrder = orders[Random.Range(0, orders.Length)];
        orderText.text = selectedOrder;
    }
}