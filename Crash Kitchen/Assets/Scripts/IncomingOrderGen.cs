using UnityEngine;
using TMPro;

public class IncomingOrderGen : MonoBehaviour
{
    public enum OrderType {MegaGlizzy, HealthyBurger, DeluxeSteak, None}

    public TMP_Text orderText;
    public Transform truck;

    private OrderType selectedOrder = OrderType.None;
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
            GetCurrentOrder();
        }
    }

    void GenerateOrder()
    {
        OrderType[] orders = {OrderType.MegaGlizzy, OrderType.HealthyBurger, OrderType.DeluxeSteak};
        selectedOrder = orders[Random.Range(0, orders.Length)];
        orderText.text = selectedOrder.ToString();
    }

    public void ResetOrder()
    {
        Debug.Log("Order reset!");
        orderGenerated = false;
    }

    public OrderType GetCurrentOrder()
    {
        Debug.Log("Current Order: " + selectedOrder);
        return selectedOrder;
    }
}