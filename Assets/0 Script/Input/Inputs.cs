using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inputs : MonoBehaviour
{

    private CharacterController characterController;

    private Player player;


    public Vector2 moveVal = new Vector2();

    public GameManager gameManager;


    private void Start() {
        player = GetComponent<Player>();
        characterController = GetComponentInChildren<CharacterController>();
    }

    public void OnRestart()
    {
#if false
        if(player.currentCheckPoint != null)
        {
            player.currentCheckPoint.TriggerCheckPoint();
        }
#endif
    }

    public void OnMovement(InputValue _value) {
        moveVal = _value.Get<Vector2>();
    }

    public void OnInteraction() {

    }


    public void OnAttack() {

    }


    public void OnDash() {
        if(player.dashUnlocked == true) {
            characterController.Dash();
        }
    }


    public void OnPause() {

        if(gameManager != null) {
            gameManager.Pause();
        }


        // TODO: Fix pause menu to quit
        //Application.Quit();
    }

    public void OnUnpause() {
        if (gameManager != null) {
            gameManager.Continue();
        }
    }

    public void OnJump() {
        if(player.jumpUnlocked == true) {
            characterController.Jump();
        }
        
    }


    public void OnGlasses() {
        if(player.glassesUnlocked == true) {
            player.ToggleGlassesState();
        }
    }

}
