using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using System.Collections;

public class WheelReset : MonoBehaviour
{
    public XRKnob knob;
    public float resetDuration = 0.5f;  // Fixed duration for reset

    public void BeginReset()
    {
        StartCoroutine(ResetWheel());
    }

    private IEnumerator ResetWheel()
    {
        float startValue = knob.value;
        float startTime = Time.time;
        
        // Reset over a fixed duration
        while (Time.time - startTime < resetDuration)
        {
            float t = (Time.time - startTime) / resetDuration;
            knob.value = Mathf.Lerp(startValue, 0.5f, t);
            yield return null;
        }
        
        // Ensure we reach exactly 0.5
        knob.value = 0.5f;
    }
}
