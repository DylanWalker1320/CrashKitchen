using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class SceneSwitcher : NetworkBehaviour
{

    public UnityEditor.SceneAsset gameScene;
    public UnityEditor.SceneAsset startScene;
    
    private string gameSceneName;
    private string startSceneName;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SwitchScenes(startSceneName);
        }
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (gameScene != null)
        {
            gameSceneName = gameScene.name;
        }
        if (startScene != null)
        {
            startSceneName = startScene.name;
        }
    }
    #endif

    private void SwitchScenes(string scene)
    {
        if (IsServer && !string.IsNullOrEmpty(scene))
        {
            var status = NetworkManager.SceneManager.LoadScene(scene, LoadSceneMode.Single);
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogError($"Failed to load {scene} " +
                        $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }
    }

    public void StartGame()
    {
        SwitchScenes(gameSceneName);
    }
}