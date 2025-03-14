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
        Debug.Log($"Teleporting player1: {player1.name} to driver position");
        player1.transform.SetParent(truck.transform);
        player1.transform.localPosition = new Vector3(0f, 0.8f, -3.9f);
        player1.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        Debug.Log($"Teleporting player2: {player2.name} to cook position");
        player2.transform.SetParent(truck.transform);
        player2.transform.localPosition = new Vector3(0f, 0.8f, 0f);
        player2.transform.localRotation = Quaternion.Euler(0f, 270f, 0f);
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
