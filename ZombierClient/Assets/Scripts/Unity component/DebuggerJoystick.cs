using UnityEngine;
using UnityEngine.UI;

public class DebuggerJoystick : MonoBehaviour
{
    public FloatingJoystick FloatingJoystick;
    public Text valueText;

    private void Update()
    {
        valueText.text = "Current Value: " + FloatingJoystick.Direction;
    }
}