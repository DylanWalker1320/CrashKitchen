using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class Car : MonoBehaviour
{

    public XRKnob knob;
    public float speed = 5;

    void Start()
    {
        // Get the knob component from the car
        knob = GetComponentInChildren<XRKnob>();

        // Set the knob's value to default
        knob.value = 0;
    }

    void Update()
    {
        // Move constantly forward at a speed of 5
        transform.position += transform.forward * speed * Time.deltaTime;

        // Rotate the car based on the knob's value
        transform.Rotate(Vector3.up, knob.value * 360 * Time.deltaTime);
    }
}
