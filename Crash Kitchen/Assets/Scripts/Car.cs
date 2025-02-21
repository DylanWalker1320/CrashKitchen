using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.InputSystem;

public class Car : MonoBehaviour
{
    public XRKnob knob;
    
    public XRLever lever;
    public float maxSpeed;
    public float maxAccel;
    public float speedDecayRate;
    public float acceleration = 0;
    public float speed = 0;
    public int direction;

    public InputActionReference driveAction;


    void Start()
    {
        // Get the knob component from the car
        knob = GetComponentInChildren<XRKnob>();

        // Set the knob's value to default
        knob.value = 0;
    }

    void Update()
    {
        // Rotate the car based on the knob's value
        transform.Rotate(Vector3.up, knob.value * 360 * Time.deltaTime);

        acceleration = driveAction.action.ReadValue<float>();

        // Get the direction of the car
        direction = lever.value ? 1 : -1;
        
        if (acceleration > 0)
        {
            // Increase the speed of the car
            speed += acceleration * maxAccel * Time.deltaTime;
            speed = Mathf.Min(speed, maxSpeed);
        }
        else
        {
            // Decrease the speed of the car
            speed -= maxAccel * Time.deltaTime * speedDecayRate;
            speed = Mathf.Max(speed, 0);
        }

        // Move the car forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime * direction);
    }
}
