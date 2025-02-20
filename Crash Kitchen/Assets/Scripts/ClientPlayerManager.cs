using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class ClientPlayerManager : NetworkBehaviour
{


    private void Awake(){

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("Player Spawned!");
        if (IsOwner)
        {
            Debug.Log("Owner Player Spawned");
            // Enable the player input
            // GetComponent<PlayerInput>().enabled = true;
        }

        if (IsClient)
        {
            Debug.Log("Client Player Spawned");
            // Enable the player input
            // GetComponent<PlayerInput>().enabled = true;
        }
    }
}
