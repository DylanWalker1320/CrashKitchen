using UnityEngine;

public class WheelControl : MonoBehaviour
{
    public Transform wheelModel;

    [HideInInspector] public WheelCollider WheelCollider;

    // Create properties for the CarControl script
    // (You should enable/disable these via the 
    // Editor Inspector window)
    public bool steerable;
    public bool motorized;

    Vector3 position;
    Quaternion rotation;

    // Start is called before the first frame update
    private void Start()
    {
        WheelCollider = GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        WheelCollider.GetWorldPose(out position, out rotation);

        // Apply a correction to rotate around X-axis instead of Z-axis
        Quaternion correction = Quaternion.Euler(0, 0, 90); // Adjust as needed

        wheelModel.transform.position = position;
        wheelModel.transform.rotation = rotation * correction;
    }


}
