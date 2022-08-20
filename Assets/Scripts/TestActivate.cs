using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Testing system to verify speech to text works on Oculus Quest 2
/// </summary>
public class TestActivate : MonoBehaviour
{
    [SerializeField] private SpeechToTextInputField inputField;
    private InputDevice targetDevice;
    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics rightControllerCharacteristics =
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(targetDevice == null)
            return;

        targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue);
        if (primaryButtonValue || Input.GetKey(KeyCode.Space))
        {
            if(!active)
                inputField.Activate();
            active = true;

        }
        else
        {
            if (active)
            {
                inputField.DeActivate();
            }
            active = false;
        }
    }
}
