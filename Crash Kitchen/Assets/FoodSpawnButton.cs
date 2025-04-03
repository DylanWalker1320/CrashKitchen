using UnityEngine;
using Unity.Netcode;

public class FoodSpawnButton : NetworkBehaviour
{
    public GameObject foodPrefab; // The prefab of the food item to spawn
    public GameObject spawnPoint; // The point where the food will be spawned

    public void OnButtonClick()
    {
        if (!NetworkManager.Singleton)
        {
            Debug.LogError("NetworkManager is not initialized. Cannot spawn food.");
            return;
        }

        // If we're the server/host, spawn directly
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Spawning food on server for client ID: " + NetworkManager.Singleton.LocalClientId);
            SpawnFood(NetworkManager.Singleton.LocalClientId);
        }
        // If we're a client, request the server to spawn
        else if (NetworkManager.Singleton.IsClient)
        {
            Debug.Log("Requesting server to spawn food for client ID: " + NetworkManager.Singleton.LocalClientId);
            SpawnFoodServerRpc();
        }
        else
        {
            Debug.LogError("Not a server or client. Cannot spawn food.");
            return;
        }
    }

    // Server RPC - will be executed on the server when called from a client
    [ServerRpc(RequireOwnership = false)]
    private void SpawnFoodServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("SpawnFoodServerRpc called by client ID: " + serverRpcParams.Receive.SenderClientId);

        // Extract the client ID of the sender
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        
        // Call the common spawn method with the client ID
        SpawnFood(clientId);
    }

    // Common method for spawning food that runs on the server
    private void SpawnFood(ulong ownerClientId)
    {
        Debug.Log("Spawning food for client ID: " + ownerClientId);

        NetworkObject prefabNetObj = foodPrefab.GetComponent<NetworkObject>();
        
        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(
            prefabNetObj,
            ownerClientId, // The client ID of the owner
            false, // Don't destroy with scene
            false, // Not a player object
            false // Don't force override
        );

        if (networkObject == null)
        {
            Debug.LogError("Failed to spawn food. NetworkObject is null.");
            return;
        }
        
        networkObject.ChangeOwnership(ownerClientId);

        // Parent the spawned object to the spawn point
        if (networkObject.TrySetParent(spawnPoint)) { // Set parent, and disable world position staying
            Debug.Log("Spawned food at " + spawnPoint.transform.position + " for client ID: " + ownerClientId);
        }
        else
        {
            Debug.LogError("Failed to set parent for spawned food. It may not be in the same scene as the spawn point.");
        }

        // Reset the position and rotation of the spawned object
        networkObject.transform.localPosition = new Vector3(5, 5, 5);
        networkObject.transform.localRotation = Quaternion.identity;


        Rigidbody rb = networkObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.constraints = RigidbodyConstraints.None;
    }
}
