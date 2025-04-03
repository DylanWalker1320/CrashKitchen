using UnityEngine;

public class NetworkManagerSingleton : MonoBehaviour
{
    private static NetworkManagerSingleton instance;
    public static NetworkManagerSingleton Instance
    {
        get {return instance;}
    }
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
