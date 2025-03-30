using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.Events;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    public bool debugMode;
    public UnityEvent OnGameStart;
    public GameObject[] outlines;
    private string debugLogPrefix = "<color=#FF4400>[GameManager]</color> ";
    private IncomingOrderGen.OrderType currentOrder;
    public Dictionary<IncomingOrderGen.OrderType, GameObject> outlineDict = new Dictionary<IncomingOrderGen.OrderType, GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start() 
    {
        InitializeWaypoints();
        InitDictionary();

        
    }

    private void InitDictionary()
    {
        outlineDict.Add(IncomingOrderGen.OrderType.HealthyBurger, outlines[0]);
        outlineDict.Add(IncomingOrderGen.OrderType.DeluxeSteak, outlines[1]);
        outlineDict.Add(IncomingOrderGen.OrderType.MegaGlizzy, outlines[2]);

        NewOrder();
    }

    public void NewOrder() 
    {
        GenerateOrder();
        SpawnFood();
        // Waypoints are handled individually
    }

    private void InitializeWaypoints()
    {
        WaypointManager.instance.Init();
    }

    private void GenerateOrder()
    {
        currentOrder = IncomingOrderGen.instance.GenerateOrder();
    }

    private void SpawnFood()
    {
        Log($"Current dictionary size: {outlineDict.Count}, dictionary: {outlineDict}");
        if (outlineDict.TryGetValue(currentOrder, out GameObject outlineObj))
        {
            outlineObj.SetActive(true);

            // Disable all others
            foreach (var outline in outlineDict.Values)
            {
                if (outline != outlineObj)
                {
                    outline.SetActive(false);
                }
            }
        }
        else
        {
            Debug.LogError("No prefab found for order type: " + currentOrder);
        }
    }

    public void Log(string message)
    {
        if (debugMode)
        {
            Debug.Log(debugLogPrefix + message);
        }
    }
}
