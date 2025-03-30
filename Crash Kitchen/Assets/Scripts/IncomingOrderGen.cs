using UnityEngine;
using TMPro;

public class IncomingOrderGen : MonoBehaviour
{
    public enum OrderType {HealthyBurger, DeluxeSteak, MegaGlizzy, None}

    public TMP_Text orderText;
    public Transform truck;

    public static IncomingOrderGen instance;

    private IncomingOrderGen.OrderType selectedOrder = OrderType.None;
    private bool orderGenerated = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    public OrderType GenerateOrder()
    {
        OrderType[] orders = {OrderType.MegaGlizzy, OrderType.HealthyBurger, OrderType.DeluxeSteak};
        selectedOrder = orders[Random.Range(0, orders.Length)];
        orderText.text = selectedOrder.ToString();

        return selectedOrder;
    }

    public OrderType GetCurrentOrder()
    {
        Debug.Log("Current Order: " + selectedOrder);
        return selectedOrder;
    }
}