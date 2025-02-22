using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;


public class ProjectSceneManager : NetworkBehaviour
{      
  /// INFO: You can remove the #if UNITY_EDITOR code segment and make SceneName public,
  /// but this code assures if the scene name changes you won't have to remember to
  /// manually update it.
#if UNITY_EDITOR
  public UnityEditor.SceneAsset SceneAsset;
  private void OnValidate()
  {
      if (SceneAsset != null)
      {
          m_SceneName = SceneAsset.name;
      }
  }
#endif
  [SerializeField]
  private string m_SceneName;

  public override void OnNetworkSpawn()
  {
      if (IsServer && !string.IsNullOrEmpty(m_SceneName))
      {
          var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Additive);
          if (status != SceneEventProgressStatus.Started)
          {
              Debug.LogWarning($"Failed to load {m_SceneName} " + $"with a {nameof(SceneEventProgressStatus)}: {status}");
          }
      }
  }
}