using UnityEngine;

public class ColliderListener : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"<color=#FF0000>[Collision Listener]</color> Collision detected with {collision.gameObject.name}");
    }
}
