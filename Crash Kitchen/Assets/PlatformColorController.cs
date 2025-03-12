using UnityEngine;
using Unity.Netcode;

public class PlatformColorController : NetworkBehaviour
{
    // Color to change to when player stands on platform
    [SerializeField] private Color activatedColor = Color.blue;
    
    // Default colors based on platform type
    [SerializeField] private Color driverPlatformColor = new Color(0.5f, 0.5f, 0.5f); // #808080 (gray)
    [SerializeField] private Color cookPlatformColor = new Color(0.31f, 0.77f, 0.25f); // #4FC43F (green)
    
    // Reference to the renderer component
    private Renderer platformRenderer;
    
    // Store the original material to avoid shared material issues
    private Material platformMaterial;
    
    // Network variable to track if the platform is activated
    private NetworkVariable<bool> isActivated = new NetworkVariable<bool>(false);
    
    // Cache the default color based on platform type
    private Color defaultColor;
    
    void Awake()
    {
        // Get the renderer component
        platformRenderer = GetComponent<Renderer>();
        
        if (platformRenderer != null)
        {
            // Create a unique instance of the material to avoid shared material issues
            platformMaterial = new Material(platformRenderer.material);
            platformRenderer.material = platformMaterial;
            
            // Determine the default color based on the platform tag
            if (CompareTag("DriverPlatform"))
            {
                defaultColor = driverPlatformColor;
                Debug.Log($"Platform {gameObject.name} identified as Driver Platform");
            }
            else if (CompareTag("CookPlatform"))
            {
                defaultColor = cookPlatformColor;
                Debug.Log($"Platform {gameObject.name} identified as Cook Platform");
            }
            else
            {
                defaultColor = Color.white; // Fallback for untagged platforms
                Debug.LogWarning($"Platform {gameObject.name} has no recognized tag, using white as default");
            }
            
            // Set initial color
            platformMaterial.color = defaultColor;
            
            Debug.Log($"Platform {gameObject.name} initialized with material: {platformMaterial.name}");
        }
        else
        {
            Debug.LogError($"No Renderer component found on platform {gameObject.name}!");
        }
    }
    
    void Start()
    {
        // Subscribe to the network variable change event
        isActivated.OnValueChanged += OnActivationChanged;
        
        // Make sure trigger is enabled
        Collider platformCollider = GetComponent<Collider>();
        if (platformCollider != null && !platformCollider.isTrigger)
        {
            Debug.LogWarning($"Enabling trigger on {gameObject.name} collider");
            platformCollider.isTrigger = true;
        }
        
        Debug.Log($"PlatformColorController started on {gameObject.name}");
    }
    
    void OnDestroy()
    {
        // Unsubscribe from the event when object is destroyed
        isActivated.OnValueChanged -= OnActivationChanged;
    }
    
    private void OnActivationChanged(bool previousValue, bool newValue)
    {
        // Update the platform color based on the activation state
        if (platformRenderer != null && platformMaterial != null)
        {
            Color targetColor = newValue ? activatedColor : defaultColor;
            platformMaterial.color = targetColor;
            // Debug.Log($"Platform {gameObject.name} color changed to {targetColor}, activated: {newValue}");
        }
    }
    
    // Called when a player enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log($"COLOR CHANGER: Trigger entered by {other.gameObject.name} with tag {other.gameObject.tag}");
        
        // Check if it's a player (using either tag or layer)
        if (other.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Debug.Log($"COLOR CHANGER: Player detected on platform {gameObject.name}");
            
            // Local color change for immediate visual feedback
            if (platformRenderer != null && platformMaterial != null)
            {
                platformMaterial.color = activatedColor;
            }
            
            // Update the network variable
            if (IsServer)
            {
                isActivated.Value = true;
                // Debug.Log($"COLOR CHANGER: Server set platform {gameObject.name} activated");
            }
            else
            {
                SetActivationServerRpc(true);
                // Debug.Log($"COLOR CHANGER: Client requested platform {gameObject.name} activation");
            }
        }
    }
    
    // Called when a player exits the trigger
    private void OnTriggerExit(Collider other)
    {
        // Debug.Log($"COLOR CHANGER: Trigger exited by {other.gameObject.name} with tag {other.gameObject.tag}");
        
        // Check if it's a player (using either tag or layer)
        if (other.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Debug.Log($"COLOR CHANGER: Player left platform {gameObject.name}");
            
            // Local color change for immediate visual feedback
            if (platformRenderer != null && platformMaterial != null)
            {
                platformMaterial.color = defaultColor; // Using platform-specific default color
            }
            
            // Update the network variable
            if (IsServer)
            {
                isActivated.Value = false;
                // Debug.Log($"COLOR CHANGER: Server set platform {gameObject.name} deactivated");
            }
            else
            {
                SetActivationServerRpc(false);
                // Debug.Log($"COLOR CHANGER: Client requested platform {gameObject.name} deactivation");
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetActivationServerRpc(bool activated)
    {
        // Update the network variable which will trigger the OnValueChanged event
        isActivated.Value = activated;
        // Debug.Log($"ServerRpc called, setting platform {gameObject.name} activated: {activated}");
    }
}
















































// Player 0 entered CookStartPlatform
// UnityEngine.Debug:Log (object)
// GameStartSystemManager:OnTriggerEnter (UnityEngine.Collider) (at Assets/Scripts/GameStartSystemManager.cs:218)

// Server registered Player 0 as Cook. Driver ready: False, Cook ready: True

// COLOR CHANGER: Trigger exited by Player with tag Player
// UnityEngine.Debug:Log (object)
// PlatformColorController:OnTriggerExit (UnityEngine.Collider) (at Assets/PlatformColorController.cs:130)

// COLOR CHANGER: Player left platform CookStartPlatform
// UnityEngine.Debug:Log (object)
// PlatformColorController:OnTriggerExit (UnityEngine.Collider) (at Assets/PlatformColorController.cs:135)

// Platform CookStartPlatform color changed to RGBA(0.310, 0.770, 0.250, 1.000), activated: False
// UnityEngine.Debug:Log (object)
// PlatformColorController:OnActivationChanged (bool,bool) (at Assets/PlatformColorController.cs:93)
// Unity.Netcode.NetworkVariable`1<bool>:set_Value (bool) (at ./Library/PackageCache/com.unity.netcode.gameobjects/Runtime/NetworkVariable/NetworkVariable.cs:129)
// PlatformColorController:OnTriggerExit (UnityEngine.Collider) (at Assets/PlatformColorController.cs:146)

// COLOR CHANGER: Server set platform CookStartPlatform deactivated
// UnityEngine.Debug:Log (object)
// PlatformColorController:OnTriggerExit (UnityEngine.Collider) (at Assets/PlatformColorController.cs:147)

// Player 0 exited CookStartPlatform
// UnityEngine.Debug:Log (object)
// GameStartSystemManager:OnTriggerExit (UnityEngine.Collider) (at Assets/Scripts/GameStartSystemManager.cs:242)

// Server unregistered Player 0 as Cook. Driver ready: False, Cook ready: False
// UnityEngine.Debug:Log (object)
// GameStartSystemManager:UpdateCookPlatformStatusServerRpc (bool) (at Assets/Scripts/GameStartSystemManager.cs:279)
// GameStartSystemManager:__rpc_handler_2359625232 (Unity.Netcode.NetworkBehaviour,Unity.Netcode.FastBufferReader,Unity.Netcode.__RpcParams)
// Unity.Netcode.RpcMessageHelpers:Handle (Unity.Netcode.NetworkContext&,Unity.Netcode.RpcMetadata&,Unity.Netcode.FastBufferReader&,Unity.Netcode.__RpcParams&) (at ./Library/PackageCache/com.unity.netcode.gameobjects/Runtime/Messaging/Messages/RpcMessages.cs:75)
// Unity.Netcode.ServerRpcMessage:Handle (Unity.Netcode.NetworkContext&) (at ./Library/PackageCache/com.unity.netcode.gameobjects/Runtime/Messaging/Messages/RpcMessages.cs:132)
// Unity.Netcode.NetworkBehaviour:__endSendServerRpc (Unity.Netcode.FastBufferWriter&,uint,Unity.Netcode.ServerRpcParams,Unity.Netcode.RpcDelivery) (at ./Library/PackageCache/com.unity.netcode.gameobjects/Runtime/Core/NetworkBehaviour.cs:124)
// GameStartSystemManager:UpdateCookPlatformStatusServerRpc (bool) (at Assets/Scripts/GameStartSystemManager.cs:269)
// GameStartSystemManager:OnTriggerExit (UnityEngine.Collider) (at Assets/Scripts/GameStartSystemManager.cs:245)

// <color=#33FF64>[XRMultiplayer]</color> <color=#EC0CFA>[Lobby Manager]</color> Sending Heartbeat Ping for Lobby GgR72qf2yFDAMyuCedgA3A
// UnityEngine.Debug:Log (object)
// XRMultiplayer.Utils:Log (string,int) (at Assets/VRMPAssets/Scripts/Helpers/Utils.cs:27)
// XRMultiplayer.LobbyManager/<LobbyHeartbeatCoroutine>d__28:MoveNext () (at Assets/VRMPAssets/Scripts/Network/NetworkManagers/LobbyManager.cs:320)
// UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr)

// COLOR CHANGER: Trigger entered by Player with tag Player
// UnityEngine.Debug:Log (object)
// PlatformColorController:OnTriggerEnter (UnityEngine.Collider) (at Assets/PlatformColorController.cs:100)

// COLOR CHANGER: Player detected on platform CookStartPlatform
// UnityEngine.Debug:Log (object)
// PlatformColorController:OnTriggerEnter (UnityEngine.Collider) (at Assets/PlatformColorController.cs:105)

// Platform CookStartPlatform color changed to RGBA(0.273, 0.503, 0.985, 1.000), activated: True
// UnityEngine.Debug:Log (object)
// PlatformColorController:OnActivationChanged (bool,bool) (at Assets/PlatformColorController.cs:93)
// Unity.Netcode.NetworkVariable`1<bool>:set_Value (bool) (at ./Library/PackageCache/com.unity.netcode.gameobjects/Runtime/NetworkVariable/NetworkVariable.cs:129)
// PlatformColorController:OnTriggerEnter (UnityEngine.Collider) (at Assets/PlatformColorController.cs:116)

// COLOR CHANGER: Server set platform CookStartPlatform activated
// UnityEngine.Debug:Log (object)
// PlatformColorController:OnTriggerEnter (UnityEngine.Collider) (at Assets/PlatformColorController.cs:117)

// Player 0 entered CookStartPlatform
// UnityEngine.Debug:Log (object)
// GameStartSystemManager:OnTriggerEnter (UnityEngine.Collider) (at Assets/Scripts/GameStartSystemManager.cs:218)

// Server registered Player 0 as Cook. Driver ready: False, Cook ready: True
// UnityEngine.Debug:Log (object)
// GameStartSystemManager:UpdateCookPlatformStatusServerRpc (bool) (at Assets/Scripts/GameStartSystemManager.cs:273)
// GameStartSystemManager:__rpc_handler_2359625232 (Unity.Netcode.NetworkBehaviour,Unity.Netcode.FastBufferReader,Unity.Netcode.__RpcParams)
// Unity.Netcode.RpcMessageHelpers:Handle (Unity.Netcode.NetworkContext&,Unity.Netcode.RpcMetadata&,Unity.Netcode.FastBufferReader&,Unity.Netcode.__RpcParams&) (at ./Library/PackageCache/com.unity.netcode.gameobjects/Runtime/Messaging/Messages/RpcMessages.cs:75)
// Unity.Netcode.ServerRpcMessage:Handle (Unity.Netcode.NetworkContext&) (at ./Library/PackageCache/com.unity.netcode.gameobjects/Runtime/Messaging/Messages/RpcMessages.cs:132)
// Unity.Netcode.NetworkBehaviour:__endSendServerRpc (Unity.Netcode.FastBufferWriter&,uint,Unity.Netcode.ServerRpcParams,Unity.Netcode.RpcDelivery) (at ./Library/PackageCache/com.unity.netcode.gameobjects/Runtime/Core/NetworkBehaviour.cs:124)
// GameStartSystemManager:UpdateCookPlatformStatusServerRpc (bool) (at Assets/Scripts/GameStartSystemManager.cs:269)
// GameStartSystemManager:OnTriggerEnter (UnityEngine.Collider) (at Assets/Scripts/GameStartSystemManager.cs:221)

// Platform DriverStartPlatform color changed to RGBA(0.500, 0.500, 0.500, 1.000), activated: False
// UnityEngine.Debug:Log (object)
// PlatformColorController:OnActivationChanged (bool,bool) (at Assets/PlatformColorController.cs:93)
// Unity.Netcode.NetworkVariable`1<bool>:set_Value (bool) (at ./Library/PackageCache/com.unity.netcode.gameobjects/Runtime/NetworkVariable/NetworkVariable.cs:129)
// PlatformColorController:SetActivationServerRpc (bool) (at Assets/PlatformColorController.cs:161)
// PlatformColorController:__rpc_handler_3451231245 (Unity.Netcode.NetworkBehaviour,Unity.Netcode.FastBufferReader,Unity.Netcode.__RpcParams)
// Unity.Netcode.RpcMessageHelpers:Handle (Unity.Netcode.NetworkContext&,Unity.Netcode.RpcMetadata&,Unity.Netcode.FastBufferReader&,Unity.Netcode.__RpcParams&) (at ./Library/PackageCache/com.unity.netcode.gameobjects/Runtime/Messaging/Messages/RpcMessages.cs:75)
// Unity.Netcode.ServerRpcMessage:Handle (Unity.Netcode.NetworkContext&) (at ./Library/PackageCache/com.unity.netcode.gameobjects/Runtime/Messaging/Messages/RpcMessages.cs:132)
// Unity.Netcode.NetworkMessageManager:ReceiveMessage<Unity.Netcode.ServerRpcMessage> (Unity.Netcode.FastBufferReader,Unity.Netcode.NetworkContext&,Unity.Netcode.NetworkMessageManager) (at ./Library/PackageCache/com.unity.netcode.gameobjects/Runtime/Messaging/NetworkMessageManager.cs:563)
// Unity.Netcode.NetworkMessageManager:HandleMessage (Unity.Netcode.NetworkMessageHeader&,Unity.Netcode.FastBufferReader,ulong,single,int) (at ./Library/PackageCache/com.unity.netcode.gameobjects/Runtime/Messaging/NetworkMessageManager.cs:422)
// Unity.Netcode.NetworkMessageManager:ProcessIncomingMessageQueue () (at ./Library/PackageCache/com.unity.netcode.gameobjects/Runtime/Messaging/NetworkMessageManager.cs:448)
// Unity.Netcode.NetworkManager:NetworkUpdate (Unity.Netcode.NetworkUpdateStage) (at ./Library/PackageCache/com.unity.netcode.gameobjects/Runtime/Core/NetworkManager.cs:332)
// Unity.Netcode.NetworkUpdateLoop:RunNetworkUpdateStage (Unity.Netcode.NetworkUpdateStage) (at ./Library/PackageCache/com.unity.netcode.gameobjects/Runtime/Core/NetworkUpdateLoop.cs:191)
// Unity.Netcode.NetworkUpdateLoop/NetworkEarlyUpdate/<>c:<CreateLoopSystem>b__0_0 () (at ./Library/PackageCache/com.unity.netcode.gameobjects/Runtime/Core/NetworkUpdateLoop.cs:214)

// ServerRpc called, setting platform DriverStartPlatform activated: False
