using UnityEngine;
using Unity.Netcode;

public class FoodSpawnButton : NetworkBehaviour
{
    public GameObject foodPrefab; // The prefab of the food item to spawn
    public Transform spawnPoint; // The point where the food will be spawned

    // This method is called when the button is clicked
    public void OnButtonClick()
    {
        if (!NetworkManager.Singleton)
        {
            Debug.LogWarning("No NetworkManager found!");
            return;
        }

        // If we're the server/host, spawn directly
        if (NetworkManager.Singleton.IsServer)
        {
            SpawnFood(NetworkManager.Singleton.LocalClientId);
        }
        // If we're a client, request the server to spawn
        else if (NetworkManager.Singleton.IsClient)
        {
            SpawnFoodServerRpc();
        }
        else
        {
            Debug.LogWarning("Not connected to the network, can't spawn food");
        }
    }

    // Server RPC - will be executed on the server when called from a client
    [ServerRpc(RequireOwnership = false)]
    private void SpawnFoodServerRpc(ServerRpcParams serverRpcParams = default)
    {
        // Extract the client ID of the sender
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        
        // Call the common spawn method with the client ID
        SpawnFood(clientId);
    }

    // Common method for spawning food that runs on the server
    private void SpawnFood(ulong ownerClientId)
    {
        Debug.Log($"Spawning food requested by client: {ownerClientId}");

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
            Debug.LogError("Failed to spawn food! Ensure the prefab has a NetworkObject component and is registered with the NetworkManager.");
            return;
        }
        
        networkObject.ChangeOwnership(ownerClientId);

        // Parent the spawned object to the spawn point
        networkObject.transform.SetParent(spawnPoint);

        // Reset the position and rotation of the spawned object
        networkObject.transform.localPosition = Vector3.zero;
        networkObject.transform.localRotation = Quaternion.identity;


        Rigidbody rb = networkObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.constraints = RigidbodyConstraints.None;
    }
}
