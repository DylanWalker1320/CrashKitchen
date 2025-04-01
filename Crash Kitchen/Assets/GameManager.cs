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
        if (!IsServer) return; // Ensure only the server runs this logic

        StartCoroutine(WaitForOrderThenSpawn());
    }

    private System.Collections.IEnumerator WaitForOrderThenSpawn()

    {
        GenerateOrder();

        // Wait until currentOrder is set properly
        while (currentOrder == IncomingOrderGen.OrderType.None) 
        {
            yield return null; // Wait for the next frame
        }

        SpawnFood();
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
        if (!IsServer) return; // Ensure only the server spawns objects

        if (outlineDict.TryGetValue(currentOrder, out GameObject outlineObj))
        {
            GameObject spawnedObj = Instantiate(outlineObj, outlinePos.transform.position, outlinePos.transform.rotation);
            spawnedObj.transform.position = outlinePos.transform.position;

            // Parent the outline to the outlinePos object
            spawnedObj.transform.SetParent(outlinePos.transform);

            // Network-Spawn the food so it's visible across all clients
            NetworkObject netObj = spawnedObj.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                netObj.Spawn();
            }
            else
            {
                Debug.LogError("Spawned object does not have a NetworkObject component!");
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
