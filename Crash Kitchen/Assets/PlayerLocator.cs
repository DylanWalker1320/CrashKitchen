using UnityEngine;

public class PlayerLocator : MonoBehaviour
{
    public GameManager.RoleType roleType;
    void Start()
    {
        GameManager.Instance.AssignPlayerToTruck(gameObject, roleType);
    }
}
