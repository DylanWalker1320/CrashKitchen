using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay; 

public class NetworkRelayManager : MonoBehaviour
{
    [SerializeField] private Button clientButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private TMP_InputField joinCodeInput;

    private void Awake()
    {
        hostButton.onClick.AddListener(() => HandleHostButton());
        clientButton.onClick.AddListener(() => HandleClientButton());
    }

    private async void HandleHostButton()
    {
        string joinCode = await StartHostWithRelay();
        Debug.Log("Host Started with Join Code: " + joinCode);

        // ✅ Update the TMP Text UI
        if (joinCodeText != null)
            joinCodeText.text = "Join Code: " + joinCode;
    }

    private async void HandleClientButton()
    {
        string enteredCode = joinCodeInput.text.Trim(); // ✅ Get the text from the input field
        if (string.IsNullOrEmpty(enteredCode))
        {
            Debug.LogWarning("No Join Code entered!");
            return;
        }

        bool success = await StartClientWithRelay(enteredCode);
        Debug.Log(success ? "Client Connected Successfully" : "Failed to Connect");

    }

    public async Task<string> StartHostWithRelay(int maxConnections = 2)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

        var relayServerData = new RelayServerData(
            allocation.RelayServer.IpV4, 
            (ushort)allocation.RelayServer.Port, 
            allocation.AllocationIdBytes, 
            allocation.ConnectionData, 
            allocation.ConnectionData, 
            allocation.Key, 
            true
        );

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log($"Relay Join Code: {joinCode}");

        // ✅ Update the TMP Text UI when the join code is generated
        if (joinCodeText != null)
            joinCodeText.text = "Join Code: " + joinCode;

        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    public async Task<bool> StartClientWithRelay(string joinCode)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);

        var relayServerData = new RelayServerData(
            joinAllocation.RelayServer.IpV4, 
            (ushort)joinAllocation.RelayServer.Port, 
            joinAllocation.AllocationIdBytes, 
            joinAllocation.ConnectionData, 
            joinAllocation.HostConnectionData, 
            joinAllocation.Key, 
            true
        );

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }
}
