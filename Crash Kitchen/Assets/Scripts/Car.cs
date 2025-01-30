using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.InputSystem; // Import this for InputAction usage

public class Car : MonoBehaviour
{
    public XRKnob knob;
    public XRLever lever;
    public float speed;
    public InputActionReference gripAction; // Reference to the XRI Left/Grip action
    private int direction;

    void Start()
    {
        // Get the knob component from the car
        knob = GetComponentInChildren<XRKnob>();

        // Set the knob's value to default
        knob.value = 0;

        // Get the direction of the car
        direction = lever.value ? 1 : -1;

        // Enable the grip action
        if (gripAction != null && gripAction.action != null)
        {
            gripAction.action.Enable();
        }
    }

    void Update()
    {
        // Adjust speed based on the grip input
        if (gripAction != null && gripAction.action != null)
        {
            float gripValue = gripAction.action.ReadValue<float>();
            speed = gripValue * 10f; // Scale grip value to a usable speed range
        }

        // Move the car forward or backward based on speed and direction
        transform.position += transform.forward * speed * Time.deltaTime * direction;

        // Rotate the car based on the knob's value
        transform.Rotate(Vector3.up, knob.value * 360 * Time.deltaTime);

        Debug.Log("Lever value: " + lever.value + ", Grip value: " + speed);
    }

    private void OnDisable()
    {
        // Disable the grip action when the script is disabled
        if (gripAction != null && gripAction.action != null)
        {
            gripAction.action.Disable();
        }
    }
}
