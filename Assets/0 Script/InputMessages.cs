using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMessages : MonoBehaviour
{

    public GameObject movementHint;
    public GameObject jumpHint;
    public GameObject glassesHint;
    public GameObject dashHint;


    public GameObject movementKeyboard;
    public GameObject movementGamepad;

    public GameObject jumpKeyboard;
    public GameObject jumpGamepad;

    public GameObject glassesKeyboard;
    public GameObject glassesGamepad;

    public GameObject dashKeyboard;
    public GameObject dashGamepad;





    public void ShowMovement() {
        if(movementHint != null) {
            movementHint.SetActive(true);
        }
    }



    public void ShowJump() {
        if(jumpHint != null) {
            jumpHint.SetActive(true);
        }

    }

    public void ShowGlasses() {
        if(glassesHint != null) {
            glassesHint.SetActive(true);
        }

    }


    public void ShowDash() {
        if(dashHint != null) {
            dashHint.SetActive(true);
        }

    }


    public void UpdateControllerScheme(string _scheme) {
        if(movementHint != null) {
            if(_scheme == "KeyboardMouse") {
                movementKeyboard.SetActive(true);
                movementGamepad.SetActive(false);
            } else {
                movementKeyboard.SetActive(false);
                movementGamepad.SetActive(true);
            }
        }

        if (jumpHint != null) {
            if (_scheme == "KeyboardMouse") {
                jumpKeyboard.SetActive(true);
                jumpGamepad.SetActive(false);
            } else {
                jumpKeyboard.SetActive(false);
                jumpGamepad.SetActive(true);
            }
        }

        if (glassesHint != null) {
            if (_scheme == "KeyboardMouse") {
                glassesKeyboard.SetActive(true);
                glassesGamepad.SetActive(false);
            } else {
                glassesKeyboard.SetActive(false);
                glassesGamepad.SetActive(true);
            }
        }

        if (dashHint != null) {
            if (_scheme == "KeyboardMouse") {
                dashKeyboard.SetActive(true);
                dashGamepad.SetActive(false);
            } else {
                dashKeyboard.SetActive(false);
                dashGamepad.SetActive(true);
            }
        }

    }


    public void HideMessages() {
        movementHint.SetActive(false);
        jumpHint.SetActive(false);
        glassesHint.SetActive(false);
        dashHint.SetActive(false);
    }



}
