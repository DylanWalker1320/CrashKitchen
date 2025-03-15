using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class CarControl : NetworkBehaviour
{
    [Header("Car Properties")]
    public float motorTorque = 2000f;
    public float brakeTorque = 2000f;
    public float maxSpeed = 20f;
    public float steeringRange = 30f;
    public float steeringRangeAtMaxSpeed = 10f;
    public float centreOfGravityOffset = -1f;
    public float accelThreshold = 0.05f;
    public XRKnob knob;
    public XRLever lever;
    public InputActionReference driveAction;

    private WheelControl[] wheels;
    private Rigidbody rigidBody;

    private float vInput;
    private float hInput;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        // Adjust center of mass to improve stability and prevent rolling
        Vector3 centerOfMass = rigidBody.centerOfMass;
        centerOfMass.y += centreOfGravityOffset;
        rigidBody.centerOfMass = centerOfMass;

        // Get all wheel components attached to the car
        wheels = GetComponentsInChildren<WheelControl>();

        if (!knob) knob = GetComponentInChildren<XRKnob>();
        if (!lever) lever = GetComponentInChildren<XRLever>();

        // Set the knob's value to default
        knob.value = 0;
    }

    // FixedUpdate is called at a fixed time interval 
    void FixedUpdate()
    {
        if (IsOwner)
        {
            // Read player input (client-side)
            vInput = driveAction.action.ReadValue<float>();
            hInput = knob.value * 2 - 1;

            // Send input to the server
            SendCarInputServerRpc(vInput, hInput);
        }

        if (!IsServer) return;

        // Apply movement
        ApplyCarPhysics(vInput, hInput);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendCarInputServerRpc(float vInput, float hInput)
    {
        // Apply input immediately on the server
        ApplyCarPhysics(vInput, hInput);
    }

    private void ApplyCarPhysics(float vInput, float hInput)
    {
        // Calculate current speed along the car's forward axis
        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.velocity);
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(forwardSpeed)); // Normalized speed factor

        // Reduce motor torque and steering at high speeds for better handling
        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        // Get the direction of the car (forward = -1, reverse = 1)
        float direction = lever.value ? -1 : 1;

        foreach (var wheel in wheels)
        {
            // Apply steering to wheels that support steering
            if (wheel.steerable)
            {
                wheel.WheelCollider.steerAngle = hInput * currentSteerRange;
            }

            if (vInput > accelThreshold)
            {
                // Apply torque to motorized wheels
                if (wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = vInput * currentMotorTorque * direction;
                }
                // Release brakes while accelerating
                wheel.WheelCollider.brakeTorque = 0f;
            }
            else
            {
                // Apply brakes when input is zero (slowing down)
                wheel.WheelCollider.motorTorque = 0f;
                wheel.WheelCollider.brakeTorque = brakeTorque;
            }
        }
    }
}
