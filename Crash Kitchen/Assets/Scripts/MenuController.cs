using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuController : MonoBehaviour
{
    public void StartBtn(){
        SceneManager.LoadScene("DrivingDemo");
    }

}