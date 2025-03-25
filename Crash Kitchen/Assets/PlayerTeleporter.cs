using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    public Transform player;
    public Transform drivingStation;
    public Transform cookingStation;
    private bool atCookingStation = false;

    void Update()
    {
        // Check for space key press and teleport player when pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (atCookingStation)
            {
                player.position = new Vector3(drivingStation.position.x, player.position.y, drivingStation.position.z);
                atCookingStation = false;
            }
            else
            {
                player.position =  new Vector3(cookingStation.position.x, player.position.y, cookingStation.position.z);
                atCookingStation = true;
            }
        }
    }
}
