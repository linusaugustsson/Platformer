using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;


public enum CharacterState
{
    Default,
}

public enum OrientationMethod
{
    towardsCamera,
    towardsMovement,
}

public struct PlayerCharacterInputs
{
    public float moveAxisForward;
    public float moveAxisRight;
    public Quaternion cameraRotation;
    public bool jumpDown;

}

public enum BonusOrientationMethod
{
    none,
    towardsGravity,
    towardsGroundSlopeAndGravity,
}

public class CharacterController : MonoBehaviour, ICharacterController
{
    public KinematicCharacterMotor motor;

    [Header("Stable Movement")]
    public float maxStableMoveSpeed = 10f;
    public float stableMovementSharpness = 15f;
    public float orientationSharpness = 10f;
    public OrientationMethod orientationMethod = OrientationMethod.towardsCamera;

    [Header("Air Movement")]
    public float maxAirMoveSpeed = 15f;
    public float airAccelerationSpeed = 15f;
    public float drag = 0.1f;

    [Header("Jumping")]
    public bool allowJumpingWhenSliding = false;
    public float jumpUpSpeed = 10f;
    public float jumpScalableForwardSpeed = 10f;
    public float jumpPreGroundingGraceTime = 0f;
    public float jumpPostGroundingGraceTime = 0f;


    [Header("Misc")]
    public List<Collider> ignoredColliders = new List<Collider>();
    public BonusOrientationMethod bonusOrientationMethod = BonusOrientationMethod.none;
    public float bonusOrientationSharpness = 10f;
    public Vector3 gravity = new Vector3(0, -30f, 0);
    public Vector3 normalGravity = new Vector3(0, -30f, 0);
    [HideInInspector]
    public Vector3 glideGravity = new Vector3(0, -3f, 0);
    public Transform meshRoot;
    public Transform cameraFollowPoint;

    public CharacterState currentCharacterState { get; private set; }

    private Collider[] _probedColliders = new Collider[8];
    private RaycastHit[] _probedHits = new RaycastHit[8];
    private Vector3 _moveInputVector;
    private Vector3 _lookInputVector;
    private bool _jumpRequested = false;
    private bool _jumpConsumed = false;
    public bool _jumpedThisFrame = false;
    private float _timeSinceJumpRequested = Mathf.Infinity;
    private float _timeSinceLastAbleToJump = 0f;
    private Vector3 _internalVelocityAdd = Vector3.zero;



    //private bool _dashConsumed = false;


    [HideInInspector]
    public bool isGliding = false;
    private bool glideConsumed = false;
    [HideInInspector]
    public float maxGlideTime = 3.0f;
    private float currentGlideTime = 0.0f;


    
    private Vector3 defaultGravity = new Vector3();

    //public Vector3 dashGravity = new Vector3();

    [Header("Dashing")]
    public bool allowAirDashing;
    public bool isDashing = false;
    public bool dashConsumed = false;
    public float maxDashTime = 0.1f;
    public float currentDashTime = 1.0f;
    public float dashCooldown = 1.0f;
    private float dashCooldownTimer = 0.0f;
    
    public ParticleSystem dashParticles;

    [HideInInspector]
    public float speedModifier = 0.0f;
    [HideInInspector]
    public float weightModifier = 0.0f;
    [HideInInspector]
    public float jumpHeightModifier = 0.0f;
    [HideInInspector]
    public float longJumpModifier = 0.0f;
    

    private Vector3 lastInnerNormal = Vector3.zero;
    private Vector3 lastOuterNormal = Vector3.zero;


    public float dashSpeed = 30.0f;


    // Start is called before the first frame update
    void Start()
    {
        motor.CharacterController = this;
        defaultGravity = gravity;
    }

    

    // Update is called once per frame
    void Update()
    {
        

        if(isGliding == true) {
            gravity = glideGravity;
            currentGlideTime += Time.deltaTime;
            if(currentGlideTime >= maxGlideTime) {
                isGliding = false;
                //glideConsumed = true;
            }
        } else {
            gravity = new Vector3(0, -30.0f, 0);
            currentGlideTime = 0.0f;
        }

        dashCooldownTimer += Time.deltaTime;
        
        if (isDashing == true) {
            //gravity = Vector3.zero;
            currentDashTime += Time.deltaTime;
            dashParticles.Play();

            if (currentDashTime >= maxDashTime) {
                isDashing = false;
                currentGlideTime = 0.0f;
                gravity = defaultGravity;
                //dashConsumed = false;
                dashEndedThisFrame = true;
                dashParticles.Stop();

            }
        }



    }

    public void SetInputs(ref PlayerCharacterInputs inputs)
    {
        // Clamp input
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.moveAxisRight, 0f, inputs.moveAxisForward), 1f);

        // Calculate camera direction and rotation on the character plane
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.forward, motor.CharacterUp).normalized;
        if(cameraPlanarDirection.sqrMagnitude == 0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.up, motor.CharacterUp).normalized;
        }
        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, motor.CharacterUp);

        switch(currentCharacterState)
        {
            case CharacterState.Default:
            {
                // Move and look inputs
                _moveInputVector = /*cameraPlanarRotation * */ moveInputVector;

                switch(orientationMethod)
                {
                    case OrientationMethod.towardsCamera:
                        _lookInputVector = cameraPlanarDirection;
                        break;
                    case OrientationMethod.towardsMovement:
                        _lookInputVector = _moveInputVector.normalized;
                        break;
                }

                    // Jumping input
                    if (inputs.jumpDown) {
                        _timeSinceJumpRequested = 0f;
                        _jumpRequested = true;
                    }


                    break;
            }
        }
    }

    public void Jump()
    {
        switch(currentCharacterState)
        {
            case CharacterState.Default:
            {

                _timeSinceJumpRequested = 0f;
                _jumpRequested = true;

                if(!allowAirDashing)
                {
                    dashConsumed = false;
                }


                break;
            }
        }
    }

    private Vector3 savedVelocity = new Vector3();

    public void Dash() {
        
        if(dashCooldownTimer >= dashCooldown) {
            if (dashConsumed == false && (allowAirDashing || motor.GroundingStatus.IsStableOnGround)) {
                savedVelocity = motor.BaseVelocity;
                currentDashTime = 0.0f;
                isDashing = true;
                dashConsumed = true;
                dashCooldownTimer = 0.0f;

                Player player = gameObject.GetComponentInParent<Player>();

                if(player != null)
                {
                    player.globalData.audioList.PlaySoundEffect(player.playerSoundEffectSource, "player_dash");
                }
            }
        }

        


    }




    public void Glide() {

        if (motor.GroundingStatus.FoundAnyGround == false && glideConsumed == false) {
            glideConsumed = true;
            isGliding = true;
        }

    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        switch(currentCharacterState)
        {
            case CharacterState.Default:
            {
                // Handle jump-related values
                {
                    // Handle jumping pre-ground grace period
                    if(_jumpRequested && _timeSinceJumpRequested > jumpPreGroundingGraceTime)
                    {
                        _jumpRequested = false;
                    }

                    if(allowJumpingWhenSliding ? motor.GroundingStatus.FoundAnyGround : motor.GroundingStatus.IsStableOnGround)
                    {
                        // If we're on a ground surface, reset jumping values
                        if(!_jumpedThisFrame)
                        {
                            _jumpConsumed = false;
                            isGliding = false;
                            glideConsumed = false;
                            currentGlideTime = 0.0f;
                        }
                    _timeSinceLastAbleToJump = 0f;
                    }
                    else
                    {
                        // Keep track of time since we were last able to jump (for grace period)
                        _timeSinceLastAbleToJump += deltaTime;
                    }
                }

                break;
            }
        }
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {

    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        if(ignoredColliders.Count == 0)
        {
            return true;
        }

        if(ignoredColliders.Contains(coll))
        {
            return false;
        }

        return true;
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {

    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        glideConsumed = false;
        currentGlideTime = 0.0f;
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {

    }

    public void PostGroundingUpdate(float deltaTime)
    {
        // Handle landing and leaving ground
        if(motor.GroundingStatus.IsStableOnGround && !motor.LastGroundingStatus.IsStableOnGround)
        {
            OnLanded();
        }
        else if(!motor.GroundingStatus.IsStableOnGround && motor.LastGroundingStatus.IsStableOnGround)
        {
            OnLeaveStableGround();
        }
    }

    protected void OnLanded()
    {
        dashCooldownTimer = dashCooldown + 0.1f;

        if(!allowAirDashing)
        {
            dashConsumed = false;
        }
    }

    protected void OnLeaveStableGround()
    {
        dashCooldownTimer = dashCooldown + 0.1f;

        if(!allowAirDashing)
        {
            dashConsumed = false;
        }
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {

    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        switch(currentCharacterState)
        {
            case CharacterState.Default:
            {
                if(_lookInputVector.sqrMagnitude > 0f && orientationSharpness > 0f)
                {
                    // Smoothly interpolate from current to target look direction
                    Vector3 smoothedLookInputDirection = Vector3.Slerp(motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-orientationSharpness * deltaTime)).normalized;

                    // Set the current rotation (which will be used by the KinematicCharacterMotor)
                    currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, motor.CharacterUp);
                }

                Vector3 currentUp = (currentRotation * Vector3.up);
                if(bonusOrientationMethod == BonusOrientationMethod.towardsGravity)
                {
                    // Rotate from current up to invert gravity
                    Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -gravity.normalized, 1 - Mathf.Exp(-bonusOrientationSharpness * deltaTime));
                    currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                }
                else if(bonusOrientationMethod == BonusOrientationMethod.towardsGroundSlopeAndGravity)
                {
                    if(motor.GroundingStatus.IsStableOnGround)
                    {
                        Vector3 initialCharacterBottomHemiCenter = motor.TransientPosition + (currentUp * motor.Capsule.radius);

                        Vector3 smoothedGroundNormal = Vector3.Slerp(motor.CharacterUp, motor.GroundingStatus.GroundNormal, 1 - Mathf.Exp(-bonusOrientationSharpness * deltaTime));
                        currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * currentRotation;

                        // Move the position to create a rotation around the bottom hemi center instead of around the pivot
                        motor.SetTransientPosition(initialCharacterBottomHemiCenter + (currentRotation * Vector3.down * motor.Capsule.radius));
                    }
                    else
                    {
                        Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -gravity.normalized, 1 - Mathf.Exp(-bonusOrientationSharpness * deltaTime));
                        currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                    }
                }
                else
                {
                    Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-bonusOrientationSharpness * deltaTime));
                    currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                }
                break;
            }
        }
    }

    private bool dashEndedThisFrame = false;

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        switch(currentCharacterState)
        {
            case CharacterState.Default:
            {
                    // Ground movement
                    if(dashEndedThisFrame == true) {
                        savedVelocity.y = 0.0f;
#if false
                    currentVelocity = savedVelocity;
#else
                    float t = 0.675f;
                    currentVelocity.x = Mathf.Lerp(currentVelocity.x, savedVelocity.x, t);
                    currentVelocity.y = Mathf.Lerp(currentVelocity.y, savedVelocity.y, t * 0.4f);
                    currentVelocity.z = Mathf.Lerp(currentVelocity.z, savedVelocity.z, t);
#endif
                    dashEndedThisFrame = false;
            }
                    

                if (isDashing == true) {
                        if (_moveInputVector.sqrMagnitude > 0f) {
                            Vector3 addedVelocity = _moveInputVector * airAccelerationSpeed * deltaTime * (longJumpModifier + 1.0f) * dashSpeed;

                            Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);

                            // Limit air velocity from inputs
                            if (currentVelocityOnInputsPlane.magnitude < 100) {
                                // clamp addedVel to make total vel not exceed max vel on inputs plane
                                Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, dashSpeed);
                                addedVelocity = newTotal - currentVelocityOnInputsPlane;
                            } else {
                                // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                                if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f) {
                                    addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                                }
                            }

                            // Prevent air-climbing sloped walls
                            if (motor.GroundingStatus.FoundAnyGround) {
                                if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f) {
                                    Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal), motor.CharacterUp).normalized;
                                    addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                                }
                            }

                            // Apply added velocity
                            currentVelocity += addedVelocity;
                        }

                        // Gravity
                        currentVelocity += gravity * deltaTime;

                    currentVelocity.y += (-gravity.y) * deltaTime * 3f;

                        // Drag
                        currentVelocity *= (1f / (1f + (drag * deltaTime)));
                    } else 
                if(motor.GroundingStatus.IsStableOnGround)
                {
                    dashConsumed = false;
                    
                    float currentVelocityMagnitude = currentVelocity.magnitude;

                    Vector3 effectiveGroundNormal = motor.GroundingStatus.GroundNormal;
                    if(currentVelocityMagnitude > 0f && motor.GroundingStatus.SnappingPrevented)
                    {
                        // Take the normal from where we're coming from
                        Vector3 groundPointToCharacter = motor.TransientPosition - motor.GroundingStatus.GroundPoint;
                        if(Vector3.Dot(currentVelocity, groundPointToCharacter) >= 0f)
                        {
                            effectiveGroundNormal = motor.GroundingStatus.OuterGroundNormal;
                        }
                        else
                        {
                            effectiveGroundNormal = motor.GroundingStatus.InnerGroundNormal;
                        }
                    }

                    // Reorient velocity on slope
                    currentVelocity = motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

                    // Calculate target velocity
                    Vector3 inputRight = Vector3.Cross(_moveInputVector, motor.CharacterUp);
                    Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                    Vector3 targetMovementVelocity = reorientedInput * maxStableMoveSpeed * (1.0f + speedModifier);

                    // Smooth movement Velocity
                    currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-stableMovementSharpness * deltaTime));

                    if(currentVelocity.x == 0f || currentVelocity.z == 0f)
                    {

                    }
                    else
                    {

                    }
                }
                // Air movement
                else {
                    // Add move input
                    if(_moveInputVector.sqrMagnitude > 0f)
                    {
                        Vector3 addedVelocity = _moveInputVector * airAccelerationSpeed * deltaTime * (longJumpModifier + 1.0f);

                        Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);

                        // Limit air velocity from inputs
                        if(currentVelocityOnInputsPlane.magnitude < maxAirMoveSpeed)
                        {
                            // clamp addedVel to make total vel not exceed max vel on inputs plane
                            Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, maxAirMoveSpeed);
                            addedVelocity = newTotal - currentVelocityOnInputsPlane;
                        }
                        else
                        {
                            // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                            if(Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                            {
                                addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                            }
                        }

                        // Prevent air-climbing sloped walls
                        if(motor.GroundingStatus.FoundAnyGround)
                        {
                            if(Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
                            {
                                Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal), motor.CharacterUp).normalized;
                                addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                            }
                        }

                        // Apply added velocity
                        currentVelocity += addedVelocity;

                        if(isDashing == true)
                        {
                            currentVelocity.y *= 4f;
                        }
                    }

                    // Gravity
                    currentVelocity += gravity * deltaTime;

                    // Drag
                    currentVelocity *= (1f / (1f + (drag * deltaTime)));
                }

                if(currentVelocity.y < -8f && !motor.GroundingStatus.FoundAnyGround)
                {

                }

                // Handle jumping
                _jumpedThisFrame = false;
                _timeSinceJumpRequested += deltaTime;
                if (_jumpRequested) {
                    /*
                    if (_doubleJumpConsumed == false && inputManager.hasDoubleJump == true && motor.GroundingStatus.FoundAnyGround == false) {
                        Vector3 jumpDirection = motor.CharacterUp;

                        currentVelocity += (jumpDirection * jumpUpSpeed * (1.0f + jumpHeightModifier)) - Vector3.Project(currentVelocity, motor.CharacterUp);
                        currentVelocity += (_moveInputVector * jumpScalableForwardSpeed * (1.0f + longJumpModifier));
                        _jumpRequested = false;
                        //_doubleJumpConsumed = true;
                        _jumpedThisFrame = true;

                        //visualPlayer.SetPrimaryAnimation(AnimationType.JUMP_SQUAT);
                        //visualPlayer.SetPrimaryFallbackAnimation(AnimationType.JUMP);
                    } else
                    // See if we actually are allowed to jump
                    */if (!_jumpConsumed && ((allowJumpingWhenSliding ? motor.GroundingStatus.FoundAnyGround : motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= jumpPostGroundingGraceTime)) {
                        // Calculate jump direction before ungrounding
                        Vector3 jumpDirection = motor.CharacterUp;
                        if (motor.GroundingStatus.FoundAnyGround && !motor.GroundingStatus.IsStableOnGround) {
                            jumpDirection = motor.GroundingStatus.GroundNormal;
                        }

                        // Makes the character skip ground probing/snapping on its next update. 
                        // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                        motor.ForceUnground();

                        // Add to the return velocity and reset jump state
                        currentVelocity += (jumpDirection * jumpUpSpeed * (1.0f + jumpHeightModifier)) - Vector3.Project(currentVelocity, motor.CharacterUp);
                        currentVelocity += (_moveInputVector * jumpScalableForwardSpeed * (1.0f + longJumpModifier));
                        _jumpRequested = false;
                        _jumpConsumed = true;
                        _jumpedThisFrame = true;


                        Player player = gameObject.GetComponentInParent<Player>();

                        if(player != null)
                        {
                            player.globalData.audioList.PlaySoundEffect(player.playerSoundEffectSource, "player_jump");
                        }


                        //visualPlayer.SetPrimaryAnimation(AnimationType.JUMP_SQUAT);
                        //visualPlayer.SetPrimaryFallbackAnimation(AnimationType.JUMP);
                    }

                    if (_jumpedThisFrame) {
                        //visualPlayer.globalData.audioList.PlaySoundEffect(transform, "jump");

                    }
                }

                /*
                if(_dashRequested) {
                        currentVelocity.y = 0.0f;
                        currentVelocity +=
                    }
                */

                /*
                    _dasedThisFrame = false;
                    _timeSinceDashRequested += Time.deltaTime;
                if(_dashRequested) {

                }
                */

                // Take into account additive velocity
                if (_internalVelocityAdd.sqrMagnitude > 0f) {
                    currentVelocity += _internalVelocityAdd;
                    _internalVelocityAdd = Vector3.zero;
                }
                break;
            }
        }
    }


}
