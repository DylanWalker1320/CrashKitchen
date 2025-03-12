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
            Debug.Log($"Platform {gameObject.name} color changed to {targetColor}, activated: {newValue}");
        }
    }
    
    // Called when a player enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by {other.gameObject.name} with tag {other.gameObject.tag}");
        
        // Check if it's a player (using either tag or layer)
        if (other.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log($"Player detected on platform {gameObject.name}");
            
            // Local color change for immediate visual feedback
            if (platformRenderer != null && platformMaterial != null)
            {
                platformMaterial.color = activatedColor;
            }
            
            // Update the network variable
            if (IsServer)
            {
                isActivated.Value = true;
                Debug.Log($"Server set platform {gameObject.name} activated");
            }
            else
            {
                SetActivationServerRpc(true);
                Debug.Log($"Client requested platform {gameObject.name} activation");
            }
        }
    }
    
    // Called when a player exits the trigger
    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"Trigger exited by {other.gameObject.name} with tag {other.gameObject.tag}");
        
        // Check if it's a player (using either tag or layer)
        if (other.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log($"Player left platform {gameObject.name}");
            
            // Local color change for immediate visual feedback
            if (platformRenderer != null && platformMaterial != null)
            {
                platformMaterial.color = defaultColor; // Using platform-specific default color
            }
            
            // Update the network variable
            if (IsServer)
            {
                isActivated.Value = false;
                Debug.Log($"Server set platform {gameObject.name} deactivated");
            }
            else
            {
                SetActivationServerRpc(false);
                Debug.Log($"Client requested platform {gameObject.name} deactivation");
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetActivationServerRpc(bool activated)
    {
        // Update the network variable which will trigger the OnValueChanged event
        isActivated.Value = activated;
        Debug.Log($"ServerRpc called, setting platform {gameObject.name} activated: {activated}");
    }
}