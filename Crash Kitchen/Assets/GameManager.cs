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
    public GameObject[] outlinePrefabs;
    private string debugLogPrefix = "<color=#FF4400>[GameManager]</color> ";
    private IncomingOrderGen.OrderType currentOrder;
    public Dictionary<IncomingOrderGen.OrderType, GameObject> outlineDict = new Dictionary<IncomingOrderGen.OrderType, GameObject>();
    public GameObject outlinePos;
    public bool currentOrderDone;

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
        outlineDict.Add(IncomingOrderGen.OrderType.HealthyBurger, outlinePrefabs[0]);
        outlineDict.Add(IncomingOrderGen.OrderType.DeluxeSteak, outlinePrefabs[1]);
        outlineDict.Add(IncomingOrderGen.OrderType.MegaGlizzy, outlinePrefabs[2]);

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
        if (outlineDict.TryGetValue(currentOrder, out GameObject outlineObj))
        {
            currentOrderDone = false;
            GameObject spawnedObj = Instantiate(outlineObj, outlinePos.transform.position, outlinePos.transform.rotation);
            spawnedObj.transform.position = outlinePos.transform.position;

            // Parent the outline to the outlinePos object
            spawnedObj.transform.SetParent(outlinePos.transform);
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
