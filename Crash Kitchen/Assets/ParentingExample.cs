using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class ParentingExample : NetworkBehaviour
{
    // References to the objects
    private GameObject parentObj;
    private GameObject childObj;
    private NetworkObject parentNetObj;
    private NetworkObject childNetObj;

    void Start()
    {
        // Find the objects and cache them
        parentObj = GameObject.Find("Parent");
        childObj = GameObject.Find("Child");

        // Get network object components
        if (parentObj != null)
            parentNetObj = parentObj.GetComponent<NetworkObject>();
            
        if (childObj != null)
            childNetObj = childObj.GetComponent<NetworkObject>();

        // Log errors if NetworkObjects are missing
        if (parentObj != null && parentNetObj == null)
            Debug.LogError("Parent object is missing NetworkObject component");
            
        if (childObj != null && childNetObj == null)
            Debug.LogError("Child object is missing NetworkObject component");
    }

    void Update()
    {
        // Press P to parent the child to the parent
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (childObj == null || parentObj == null)
            {
                Debug.LogError("Child or Parent object not found!");
                return;
            }

            if (childNetObj == null || parentNetObj == null)
            {
                Debug.LogError("NetworkObject components missing!");
                return;
            }

            // Check if both objects are spawned
            if (!childNetObj.IsSpawned || !parentNetObj.IsSpawned)
            {
                Debug.LogError($"Objects must be spawned before parenting! Child:${childNetObj.IsSpawned} Parent:${parentNetObj.IsSpawned}");
                return;
            } else {
                childNetObj.Spawn();
                parentNetObj.Spawn();
            }

            // If we're the server, parent directly
            if (NetworkManager.Singleton.IsServer)
            {
                ParentObjects(childNetObj, parentNetObj);
            }
            // If we're a client, request the server to parent via RPC
            else
            {
                ParentObjectsServerRpc(childNetObj.NetworkObjectId, parentNetObj.NetworkObjectId);
            }
        }

        // Press U to unparent (reset to world space)
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (childObj != null && childNetObj != null && childNetObj.IsSpawned)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    UnparentObject(childNetObj);
                }
                else
                {
                    UnparentObjectServerRpc(childNetObj.NetworkObjectId);
                }
            }
        }
    }

    // Local helper method for parenting on the server
    private void ParentObjects(NetworkObject childNetObj, NetworkObject parentNetObj)
    {
        // Try the NetworkTransform disabling technique
        NetworkTransform childNetTransform = childNetObj.GetComponent<NetworkTransform>();
        bool hadNetTransform = false;
        bool wasEnabled = false;
        
        if (childNetTransform != null)
        {
            hadNetTransform = true;
            wasEnabled = childNetTransform.enabled;
            childNetTransform.enabled = false;
            Debug.Log("Temporarily disabled NetworkTransform for parenting");
        }
        
        // Try parenting
        bool success = childNetObj.TrySetParent(parentNetObj);
        Debug.Log($"Parenting result: {success}");
        
        // Re-enable NetworkTransform if needed
        if (hadNetTransform && wasEnabled)
        {
            childNetTransform.enabled = true;
            Debug.Log("Re-enabled NetworkTransform");
        }
    }

    // Local helper method for unparenting on the server
    private void UnparentObject(NetworkObject childNetObj)
    {
        NetworkTransform childNetTransform = childNetObj.GetComponent<NetworkTransform>();
        bool hadNetTransform = false;
        bool wasEnabled = false;
        
        if (childNetTransform != null)
        {
            hadNetTransform = true;
            wasEnabled = childNetTransform.enabled;
            childNetTransform.enabled = false;
        }
        
        bool success = childNetObj.TrySetParent((NetworkObject)null);
        Debug.Log($"Unparenting result: {success}");
        
        if (hadNetTransform && wasEnabled)
        {
            childNetTransform.enabled = true;
        }
    }

    // Server RPC for parenting objects
    [ServerRpc(RequireOwnership = false)]
    private void ParentObjectsServerRpc(ulong childObjectId, ulong parentObjectId)
    {
        // Validate we're on the server
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("ParentObjectsServerRpc called on client!");
            return;
        }

        Debug.Log($"Server received request to parent {childObjectId} to {parentObjectId}");
        
        // Find the NetworkObjects by their IDs
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(childObjectId, out NetworkObject child) &&
            NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(parentObjectId, out NetworkObject parent))
        {
            ParentObjects(child, parent);
        }
        else
        {
            Debug.LogError("Child or Parent NetworkObject not found in SpawnManager!");
        }
    }

    // Server RPC for unparenting objects
    [ServerRpc(RequireOwnership = false)]
    private void UnparentObjectServerRpc(ulong childObjectId)
    {
        // Validate we're on the server
        if (!NetworkManager.Singleton.IsServer)
            return;
            
        Debug.Log($"Server received request to unparent {childObjectId}");
        
        // Find the NetworkObject by ID
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(childObjectId, out NetworkObject child))
        {
            UnparentObject(child);
        }
        else
        {
            Debug.LogError("Child NetworkObject not found in SpawnManager!");
        }
    }
}