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
    [SerializeField] private GameObject startingPlayerPrefab;

    private async void Start(){
        await UnityServices.InitializeAsync();
        
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In | " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }


    private void Awake()
    {
        hostButton.onClick.AddListener(() => HandleHostButton());
        clientButton.onClick.AddListener(() => HandleClientButton());
    }

    private async void HandleHostButton()
    {
        string joinCode = await StartHostWithRelay();
        Debug.Log("Host Started with Join Code: " + joinCode);

        // Update the TMP Text UI
        if (joinCodeText != null)
            joinCodeText.text = "Join Code: " + joinCode;
    }

    private async void HandleClientButton()
    {
        Debug.Log("Client Button Clicked");

        if (joinCodeInput == null)
        {
            Debug.LogError("Join Code Input Field is missing!");
            return;
        }

        string enteredCode = joinCodeInput.text.Trim();
        Debug.Log("Entered Join Code: " + enteredCode);

        // Validate join code format
        if (!System.Text.RegularExpressions.Regex.IsMatch(enteredCode, "^[6789BCDFGHJKLMNPQRTWbcdfghjklmnpqrtw]{6,12}$"))
        {
            Debug.LogError($"Invalid Join Code: {enteredCode}. Must be 6-12 characters long and contain only '6789BCDFGHJKLMNPQRTW'.");
            return;
        }

        Debug.Log("Client trying to start with Join Code: " + enteredCode);
        await StartClientWithRelay(enteredCode);
        
    }


    public async Task<string> StartHostWithRelay()
    {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Join Code: " + joinCode);
            var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();

            if (joinCodeText != null){
                joinCodeText.text = "Join Code: " + joinCode;
                return joinCode;
            } else {
                return null;
            }
        } catch (RelayServiceException e){
            Debug.Log("Error Starting host with Relay: " + e);
            return null;
        }
    }

    public async Task StartClientWithRelay(string joinCode)
    {
        try {
            Debug.Log("Join Code: " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            // Setup Relay
            var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();

        } catch (RelayServiceException e){
            Debug.Log(e);
        }
    }
}