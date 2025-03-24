using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    public Transform player;
    public Transform destination;

    void Update()
    {
        // Check for space key press and teleport player when pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"<color=green>Teleporting player to {destination.position}</color>");
            player.position = new Vector3(destination.position.x, player.position.y, destination.position.z);
        }
    }
}
