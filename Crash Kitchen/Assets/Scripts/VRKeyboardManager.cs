using UnityEngine;
using TMPro;

public class VRKeyboardManager : MonoBehaviour
{
    public TMP_InputField inputField;
    private TouchScreenKeyboard keyboard;

    public void OpenKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    private void Update()
    {
        Debug.Log("LOOK HERE:" + keyboard + inputField);
        if (keyboard != null && keyboard.active)
        {
            inputField.text = keyboard.text;
        }
    }
}
