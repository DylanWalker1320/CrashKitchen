using UnityEngine;
using Unity.Netcode;

public class PlayerYLevelFreeze : NetworkBehaviour
{
    public bool lockYPosition = false;
    private CharacterController characterController;
    private float fixedYPosition = 0f;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        fixedYPosition = transform.position.y;
    }

    void Update()
    {
        if (IsOwner && lockYPosition)
        {
            // Get current position
            Vector3 currentPosition = transform.position;
            
            // If Y position has changed, reset it
            if (currentPosition.y != fixedYPosition)
            {
                // Create a new position with the fixed Y value
                Vector3 fixedPosition = new Vector3(currentPosition.x, fixedYPosition, currentPosition.z);
                
                // Move the character controller to the fixed position
                // We use a zero vector because we just want to reset position, not add movement
                characterController.enabled = false;
                transform.position = fixedPosition;
                characterController.enabled = true;
            }
        }
    }
}
