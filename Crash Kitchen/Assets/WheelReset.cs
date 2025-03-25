using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using System.Collections;

public class WheelReset : MonoBehaviour
{
    public XRKnob knob;
    public float lerpRate = 5f;

    public void BeginReset()
    {
        Debug.Log("Resetting wheel");
        StartCoroutine(ResetWheel());
    }

    private IEnumerator ResetWheel()
    {
        // Lerp to the original position
        while (knob.value > 0.01f)
        {
            knob.value = Mathf.Lerp(knob.value, 0, Time.deltaTime * lerpRate);
            yield return null;
        }
    }
}
