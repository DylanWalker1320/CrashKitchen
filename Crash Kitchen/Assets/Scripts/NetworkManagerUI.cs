using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class NetworkManagerUI : MonoBehaviour {
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button hostButton;

    private void Awake(){
        serverButton.onClick.AddListener(() => {
          NetworkManager.Singleton.StartServer();
        });
        
        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });
        
        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });

    }
}
