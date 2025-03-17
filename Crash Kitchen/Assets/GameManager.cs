using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.Events;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public bool debugMode;
    public UnityEvent OnGameStart;
    private string debugLogPrefix = "<color=#FF4400>[GameManager]</color> ";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Log(string message)
    {
        if (debugMode)
        {
            Debug.Log(debugLogPrefix + message);
        }
    }
}
