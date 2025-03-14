using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{

    public enum PlatformType
    {
        Driver,
        Cook
    } 

    public PlatformType platformType;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Platform type touched!: {platformType}, Player: {other.name}");
        if (other.tag == "Player")
        {
            Debug.Log("Player entered platform trigger type: " + platformType);
            if (platformType == PlatformType.Driver)
            {
                GameManager.isDriverPlatformEnabled = true;
                GameManager.player1 = other.gameObject;
            }
            else if (platformType == PlatformType.Cook)
            {
                GameManager.isCookPlatformEnabled = true;
                GameManager.player2 = other.gameObject;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"Platform type exited!: {platformType}, Player: {other.name}");
        if (other.tag == "Player")
        {
            Debug.Log("Player exited platform trigger type " + platformType);
            if (platformType == PlatformType.Driver)
            {
                GameManager.isDriverPlatformEnabled = false;
                GameManager.player1 = null;
            }
            else if (platformType == PlatformType.Cook)
            {
                GameManager.isCookPlatformEnabled = false;
                GameManager.player2 = null;
            }
        }
    }
}
