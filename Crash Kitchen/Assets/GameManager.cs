using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.Events;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
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
    }

    // public override void OnNetworkSpawn()
    // {
    //     base.OnNetworkSpawn();
        
    //     NewOrder();
    // }

    public void NewOrder() 
    {
        //if (!IsServer) return; // Ensure only the server runs this logic

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

        return;
        // if (!IsServer) return;

        // if (outlineDict.TryGetValue(currentOrder, out GameObject outlineObj))
        // {
        //     currentOrderDone = false;

        //     // Get the NetworkObject component from the prefab
        //     NetworkObject prefabNetObj = outlineObj.GetComponent<NetworkObject>();
            
        //     if (prefabNetObj != null)
        //     {
        //         NetworkObject spawnedNetObj = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(
        //             prefabNetObj,                      // The prefab to spawn
        //             NetworkManager.ServerClientId,     // Server owns this object
        //             false,                            // Don't destroy with scene
        //             false,                            // Not a player object
        //             false,                            // Don't force override
        //             outlinePos.transform.position,    // Position
        //             outlinePos.transform.rotation     // Rotation
        //         );
                
        //         spawnedNetObj.transform.SetParent(outlinePos.transform);
        //     }
        //     else
        //     {
        //         Debug.LogError("Outline prefab does not have a NetworkObject component!");
        //     }
        // }
        // else
        // {
        //     Debug.LogError("No prefab found for order type: " + currentOrder);
        // }
    }


    public void Log(string message)
    {
        if (debugMode)
        {
            Debug.Log(debugLogPrefix + message);
        }
    }
}
