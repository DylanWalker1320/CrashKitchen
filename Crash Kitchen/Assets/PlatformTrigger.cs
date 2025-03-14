using UnityEngine;
using Unity.Netcode;

public class PlatformTrigger : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindFirstObjectByType<GameManager>();
    }

    public enum PlatformType
    {
        Driver,
        Cook
    }

    public PlatformType platformType;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Platform type touched!: {platformType}, Player: {other.name}");
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player entered platform trigger type: {platformType}");

            if (platformType == PlatformType.Driver)
            {
                if (gameManager.IsServer)
                {
                    gameManager.isDriverPlatformEnabled.Value = true;
                    GameManager.player1 = other.gameObject;
                }
                else
                {
                    gameManager.SetDriverPlatformEnabled(true);
                }
            }
            else if (platformType == PlatformType.Cook)
            {
                if (gameManager.IsServer)
                {
                    gameManager.isCookPlatformEnabled.Value = true;
                    GameManager.player2 = other.gameObject;
                }
                else
                {
                    gameManager.SetCookPlatformEnabled(true);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"Platform type exited!: {platformType}, Player: {other.name}");
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player exited platform trigger type: {platformType}");

            if (platformType == PlatformType.Driver)
            {
                if (gameManager.IsServer)
                {
                    gameManager.isDriverPlatformEnabled.Value = false;
                    GameManager.player1 = null;
                }
                else
                {
                    gameManager.SetDriverPlatformEnabled(false);
                }
            }
            else if (platformType == PlatformType.Cook)
            {
                if (gameManager.IsServer)
                {
                    gameManager.isCookPlatformEnabled.Value = false;
                    GameManager.player2 = null;
                }
                else
                {
                    gameManager.SetCookPlatformEnabled(false);
                }
            }
        }
    }
}
