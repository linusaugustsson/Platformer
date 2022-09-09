using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceDetect : MonoBehaviour
{

    public PlayerInput playerInput;
    /*
    public enum CurrentDevice {
        KeyboardMouse,
        Gamepad
    }

    public CurrentDevice currentDevice = CurrentDevice.KeyboardMouse;
    */

    private string current = "";

    public InputMessages inputMessages;

    private void Awake() {
        
    }

    private void Update() {
        if(current != playerInput.currentControlScheme) {
            /*
            if (playerInput.currentControlScheme == "KeyboardMouse") {
                currentDevice = CurrentDevice.KeyboardMouse;
            } else {
                currentDevice = CurrentDevice.Gamepad;
            }
            */
            current = playerInput.currentControlScheme;
            ChangedControlSceheme();
        }

        
    }


    public void ChangedControlSceheme() {
        if(inputMessages != null) {
            inputMessages.UpdateControllerScheme(playerInput.currentControlScheme);
        }

    }

}
