using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    public static GameObject truck;

    public NetworkVariable<bool> isDriverPlatformEnabled = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> isCookPlatformEnabled = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public static GameObject player1; // Driver
    public static GameObject player2; // Cook

    public void Start()
    {
        Debug.Log("GameManager Start()");

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        if (truck == null)
        {
            truck = GameObject.FindGameObjectWithTag("Truck");
            if (truck == null)
            {
                Debug.LogError("Cannot find Truck object with tag 'Truck'");
            }
        }
    }

    private void StartGame()
    {
        if (!IsServer) return; // Only the server controls teleportation

        Debug.Log($"Teleporting players to positions...");

        // Ensure both players exist
        if (player1 == null || player2 == null)
        {
            Debug.LogError("Players are not assigned properly.");
            return;
        }

        // Assign as children of the truck (ensure they are networked)
        player1.transform.SetParent(truck.transform, true);
        player2.transform.SetParent(truck.transform, true);

        // Move players using an RPC so all clients update correctly
        SetPlayerTransformServerRpc(player1.GetComponent<NetworkObject>().NetworkObjectId, new Vector3(0f, 0.8f, -3.9f), Quaternion.Euler(0f, 180f, 0f));
        SetPlayerTransformServerRpc(player2.GetComponent<NetworkObject>().NetworkObjectId, new Vector3(0f, 0.8f, 0f), Quaternion.Euler(0f, 270f, 0f));
    }

    // ServerRpc to move players safely
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerTransformServerRpc(ulong playerId, Vector3 position, Quaternion rotation)
    {
        NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerId];
        if (playerObject != null)
        {
            playerObject.transform.localPosition = position;
            playerObject.transform.localRotation = rotation;
        }
    }


    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        isDriverPlatformEnabled.OnValueChanged += OnPlatformStateChanged;
        isCookPlatformEnabled.OnValueChanged += OnPlatformStateChanged;
    }

    private void OnPlatformStateChanged(bool previousValue, bool newValue)
    {
        Debug.Log($"Driver: {isDriverPlatformEnabled.Value}, Cook: {isCookPlatformEnabled.Value}");

        if (isDriverPlatformEnabled.Value && isCookPlatformEnabled.Value)
        {
            StartGame();
        }
    }

    public void SetDriverPlatformEnabled(bool value)
    {
        if (IsServer)
        {
            isDriverPlatformEnabled.Value = value;
        }
        else
        {
            SetDriverPlatformEnabledServerRpc(value);
        }
    }

    public void SetCookPlatformEnabled(bool value)
    {
        if (IsServer)
        {
            isCookPlatformEnabled.Value = value;
        }
        else
        {
            SetCookPlatformEnabledServerRpc(value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetDriverPlatformEnabledServerRpc(bool value)
    {
        isDriverPlatformEnabled.Value = value;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetCookPlatformEnabledServerRpc(bool value)
    {
        isCookPlatformEnabled.Value = value;
    }
}
