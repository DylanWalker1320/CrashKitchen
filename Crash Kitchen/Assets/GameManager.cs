using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.Events;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public bool debugMode;
    public UnityEvent OnGameStart;
    public GameObject[] outlinePrefabs;
    private string debugLogPrefix = "<color=#FF4400>[GameManager]</color> ";
    private IncomingOrderGen.OrderType currentOrder;
    public GameObject outlinePos;
    public Dictionary<IncomingOrderGen.OrderType, GameObject> outlineDict = new Dictionary<IncomingOrderGen.OrderType, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start() 
    {
        InitializeWaypoints();
        GenerateOrder();
        SpawnFood();
    }

    void NewOrder() 
    {
        GenerateOrder();
        SpawnFood();
        // Waypoints are handled individually
    }

    void InitDictionary() 
    {
        outlineDict.Add(IncomingOrderGen.OrderType.HealthyBurger, outlinePrefabs[0]);
        outlineDict.Add(IncomingOrderGen.OrderType.DeluxeSteak, outlinePrefabs[1]);
        outlineDict.Add(IncomingOrderGen.OrderType.MegaGlizzy, outlinePrefabs[2]);
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
        Instantiate(outlineDict[currentOrder], outlinePos.transform.position, outlinePos.transform.rotation);
    }

    public void Log(string message)
    {
        if (debugMode)
        {
            Debug.Log(debugLogPrefix + message);
        }
    }
}
