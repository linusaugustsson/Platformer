using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using UnityEngine.InputSystem;
using UnityEngine.Animations;


public enum PlayerSpriteOrientation {
    South = 0,
    West = 1,
    North = 2,
    East = 3,
};

public enum PlayerSpriteState {
    Idle = 0,
    Walking = 1,
    JumpStart = 2,
    Jump = 3,
    GotSpoida = 4,
    GotGlasses = 5,
    GotDash = 6
};


[RequireComponent(typeof(Inputs))]
public class Player : MonoBehaviour
{
    public CharacterController characterController;
    public Cinemachine.CinemachineVirtualCamera virtualCamera;
    public Camera mainCamera;
    public PostProcessScript postProcess;

    public Cinemachine.CinemachineVirtualCamera spiderBossCamera;


    [HideInInspector]
    public Inputs inputs;

    public bool hinderMovement;

    public bool jumpUnlocked = false;
    public bool dashUnlocked = false;
    public bool glassesUnlocked = false;

    public CheckPoint currentCheckPoint;

    public int numberOfKeys;

    public bool crazyTaxiMovement;

    public GlobalData globalData;

    public PlayerSpriteOrientation spriteOrientation;
    public PlayerSpriteState spriteState;

    private float deathFloorY = -512f;

    private bool isSlowed = false;
    private float slowAmount = 0.5f;
    public float maxSlowTime = 2.0f;
    private float currentSlowTime = 0.0f;

    public PlayerInput playerInput;

    //public ParticleSystem dashParticles;
    public ParticleSystemRenderer dashParticles;

    public InputMessages inputMessages;

    public GameManager gameManager;

    public enum GlassesState {
        off,
        on
    }

    public GlassesState currentGlassesState = GlassesState.off;

    public LayerMask glassesOffMask;
    public LayerMask glassesOnMask;

    public Animator playerAnimator;

    public AudioSource musicSourceA;
    public AudioSource musicSourceB;
    public AudioSource generalSoundEffectSource;
    public AudioSource playerSoundEffectSource;


    private void Awake() {
        inputs = GetComponent<Inputs>();
        globalData.player = this;
    }

    // Update is called once per frame
    void Update()
    {
        HandleCharacterInput();

        if(currentCheckPoint != null && characterController.transform.position.y < deathFloorY)
        {
            currentCheckPoint.TriggerCheckPoint();
        }
    }


    private void HandleCharacterInput() {
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        if(hinderMovement)
        {
            characterInputs.moveAxisForward = 0f;
            characterInputs.moveAxisRight = 0f;

            characterInputs.jumpDown = false;
        }
        else
        {
            if(crazyTaxiMovement)
            {
                characterInputs.moveAxisForward = 0f;
                characterInputs.moveAxisRight = inputs.moveVal.x;
            }
            else
            {
                characterInputs.moveAxisForward = inputs.moveVal.y;
                characterInputs.moveAxisRight = inputs.moveVal.x;
            }

            if(isSlowed)
            {
                characterInputs.moveAxisForward *= slowAmount;
                characterInputs.moveAxisRight *= slowAmount;
                currentSlowTime += Time.deltaTime;

                characterController.jumpHeightModifier = -(slowAmount * 0.05f);

                if(currentSlowTime >= maxSlowTime)
                {
                    isSlowed = false;
                    currentSlowTime = 0.0f;
                    slowAmount = 1.0f;
                    characterController.jumpHeightModifier = 0f;
                }
            }
        }

        if(playerInput.currentControlScheme == "Gamepad")
        {
            if(characterInputs.moveAxisForward > 0.35f)
            {
                characterInputs.moveAxisForward = 1f;
            }
            else if(characterInputs.moveAxisForward < -0.35f)
            {
                characterInputs.moveAxisForward = -1f;
            }
            else
            {
                characterInputs.moveAxisForward = 0f;
            }


            if(characterInputs.moveAxisRight > 0.35f)
            {
                characterInputs.moveAxisRight = 1f;
            }
            else if(characterInputs.moveAxisRight < -0.35f)
            {
                characterInputs.moveAxisRight = -1f;
            }
            else
            {
                characterInputs.moveAxisRight = 0f;
            }
        }

        SetOrientation(characterInputs);

        if((characterInputs.moveAxisForward != 0.0f || characterInputs.moveAxisRight != 0.0) && characterController.motor.GroundingStatus.IsStableOnGround == true) {
            spriteState = PlayerSpriteState.Walking;
        }


        if(characterInputs.moveAxisRight == 0.0f && characterInputs.moveAxisForward == 0.0 && characterController.motor.GroundingStatus.IsStableOnGround == true && crazyTaxiMovement == false) {
            spriteState = PlayerSpriteState.Idle;
        } else if(crazyTaxiMovement == true) {
            spriteState = PlayerSpriteState.Walking;
        }

        if(characterController.motor.GroundingStatus.IsStableOnGround == false && characterController._jumpedThisFrame == false) {
            spriteState = PlayerSpriteState.Jump;
        }

        if(characterController._jumpedThisFrame == true) {
            spriteState = PlayerSpriteState.JumpStart;
        }




        playerAnimator.SetInteger("state", (int)spriteState);
        playerAnimator.SetInteger("orientation", (int)spriteOrientation);


        // Apply inputs to character
        characterController.SetInputs(ref characterInputs);
    }

    public void OnCheckpointTriggered(Vector3 position)
    {
        characterController.motor.SetPosition(position, true);
        characterController.motor.BaseVelocity = new Vector3(0f, 0f, 0f);
        SetGlassesState(Player.GlassesState.off);
    }

    public void OnLevelLoaded(Vector3 position)
    {
        // TODO!!!!!
        /* . . . . . . 
         * 
         * . . . . . .
         * 
         * . . . . . . */
    }

    public void SetDeathFloorValue(float value)
    {
        deathFloorY = value;
    }

    public void ToggleGlassesState() {
        if (currentGlassesState == GlassesState.off) {
            currentGlassesState = GlassesState.on;
            mainCamera.cullingMask = glassesOnMask;
            postProcess.GlassesOn();
            playerAnimator.SetBool("has_glasses", true);
            globalData.audioList.PlaySoundEffect(playerSoundEffectSource, "player_glasses_on");
        } else {
            currentGlassesState = GlassesState.off;
            mainCamera.cullingMask = glassesOffMask;
            postProcess.GlassesOff();
            playerAnimator.SetBool("has_glasses", false);
            globalData.audioList.PlaySoundEffect(playerSoundEffectSource, "player_glasses_off");
        }
        postProcess.ChangedGlasses();

    }

    public void SetGlassesState(GlassesState newState)
    {
        if(currentGlassesState != newState)
        {
            ToggleGlassesState();
        }
    }

    public void PlayerHurt(int _damage) {



    }

    public void PlayerHurt(int _damage, Vector3 _fromPosition) {


    }


    public void PlayerSlow(float _slowAmount, float _slowTime) {
        isSlowed = true;
        currentSlowTime = 0.0f;

        maxSlowTime = _slowTime;
        slowAmount = _slowAmount;

    }

    public Texture dashWest;
    public Texture dashEast;
    public Texture dashNorth;
    public Texture dashSouth;
    public void SetOrientation(PlayerCharacterInputs _playerCharacterInputs) {
        if(crazyTaxiMovement)
        {
            spriteOrientation = PlayerSpriteOrientation.North;
            dashParticles.material.mainTexture = dashNorth;
        }
        else
        {
            if(_playerCharacterInputs.moveAxisForward > 0.2f)
            {
                spriteOrientation = PlayerSpriteOrientation.North;
                dashParticles.material.mainTexture = dashNorth;
            }
            else if(_playerCharacterInputs.moveAxisForward < -0.2f)
            {
                spriteOrientation = PlayerSpriteOrientation.South;
                dashParticles.material.mainTexture = dashSouth;
            }

            if(_playerCharacterInputs.moveAxisRight > 0.2f)
            {
                spriteOrientation = PlayerSpriteOrientation.East;
                dashParticles.material.mainTexture = dashEast;
            }
            else if(_playerCharacterInputs.moveAxisRight < -0.2f)
            {
                spriteOrientation = PlayerSpriteOrientation.West;
                dashParticles.material.mainTexture = dashWest;
            }
        }
    }

}
