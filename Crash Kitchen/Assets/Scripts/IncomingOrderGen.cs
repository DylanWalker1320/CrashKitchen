using UnityEngine;
using TMPro;  // Import TextMeshPro namespace

public class IncomingOrderGen : MonoBehaviour
{
    public TMP_Text orderText;  // Assign the TMP text object in the Inspector
    private string selectedOrder;

    void Start()
    {
        GenerateOrder();
    }

    void GenerateOrder()
    {
        string[] orders = { "Mega Glizzy", "Healthy Burger", "Deluxe Steak" };
        selectedOrder = orders[Random.Range(0, orders.Length)];
        orderText.text = selectedOrder;
    }
}
