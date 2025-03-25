using UnityEngine;
using UnityEngine.SceneManagement;

public class GameReset : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
    }
    void ResetGame()
    {
        Debug.Log("Game reset triggered - loading GameStart scene");
        SceneManager.LoadScene(0); // Game start scene
    }
}