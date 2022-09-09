using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;


// NOTE(Patrik): All enum entries should be numbered here. Otherwise it can mess up numbers in the editor.
[System.Serializable]
public enum BoundEventCollidingType
{
    Any = 0,
    Player = 1,
    EyeTimer = 2,
    EyeAlwaysOn = 3,
    EyeAny = 4,
};

[System.Serializable]
public struct BoundEventToggle {
    public enum EventType {
        OpenDoor = 0,
        SetCurrentCheckPoint = 1,
        TriggerCheckPoint = 2,
        UnlockDoorIfPlayerHasKey = 3,
        RecordCheckPoint = 4,
        GoToNextLevel = 5,
    };

    public EventType type;
    public bool applyToChildren;
    public GameObject gameObject;
};

[System.Serializable]
public struct BoundEventBool {
    public enum EventType {
        SetActive = 0,
        HinderPlayerMovement = 1,
        SetDoorOpen = 2,
        SetVisibleOrShadow = 3,
        SetDoorLocked = 4,
        SetDoorFrozen = 5,
        EnableCrazyTaxiMovement = 6,
    };

    public EventType type;
    public bool applyToChildren;
    public GameObject gameObject;
    public bool value;
};

[System.Serializable]
public struct BoundEventInt {
    public enum EventType {
        SetKeyCount = 0,
        AddKeys = 1,
    };

    public EventType type;
    public bool applyToChildren;
    public GameObject gameObject;
    public int value;
};

[System.Serializable]
public struct BoundEventFloat {
    public enum EventType {
        SetDeathFloorValue = 0,
        SetPlayerCapsuleRadius = 1,
    };

    public EventType type;
    public bool applyToChildren;
    public GameObject gameObject;
    public float value;
};

[System.Serializable]
public struct BoundEventString {
    public enum EventType {
        PlaySound = 0,
        // LoadLevel = 1,
        PlayMusic = 2,
        PlayMusicCrossFadeFromPrevious = 3,
    };

    public EventType type;
    public bool applyToChildren;
    public GameObject gameObject;
    public string value;
};

[System.Serializable]
public struct BoundEventVector3 {
    public enum EventType {
        SetPosition = 0,
        SetPositionRelative = 1,
        SetCameraFollowOffset = 2,
        SetCameraRotation = 3,
    };

    public EventType type;
    public bool applyToChildren;
    public GameObject gameObject;
    public Vector3 value;
};

public enum BoundEventTrigger {
    Enter = 0,
    Exit = 1,
    EnterAndExit = 2,
    Continious = 3,
};

public class BoundEvent : MonoBehaviour
{
    public BoundEventTrigger trigger;

    [Space(8)]
    public GameObject collidingObject;
    public BoundEventCollidingType collidingType;

    [Space(8)]
    public BoundEventToggle[] toggleEvents;
    public BoundEventBool[] boolEvents;
    public BoundEventInt[] intEvents;
    public BoundEventFloat[] floatEvents;
    public BoundEventString[] stringEvents;
    public BoundEventVector3[] vector3Events;

    [Space(8)]
    public GlobalData globalData;


    public bool IsRequiredType(GameObject gameObject)
    {
        switch(collidingType)
        {
            case BoundEventCollidingType.Player:
            {
                Player player = gameObject.GetComponentInParent<Player>();

                if(player != null)
                {
                    return true;
                }
            }
            break;

            case BoundEventCollidingType.EyeAny:
            case BoundEventCollidingType.EyeTimer:
            case BoundEventCollidingType.EyeAlwaysOn:
            {
                EyeTimer timer = gameObject.GetComponentInParent<EyeTimer>();

                if(collidingType != BoundEventCollidingType.EyeAlwaysOn)
                {
                    if(timer != null && timer.visualBeam.activeSelf)
                    {
                        return true;
                    }
                }

                if(collidingType != BoundEventCollidingType.EyeTimer)
                {
                    if(timer == null)
                    {
                        if(gameObject.TryGetComponent(out EyeBeam beam))
                        {
                            return beam.visualBeam.gameObject.activeSelf;
                        }
                        else
                        {
                            if(gameObject.transform.parent != null)
                            {
                                if(gameObject.transform.parent.GetComponentInChildren<EyeBeam>() != null)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            break;

            default:
            {
                if(collidingObject == null)
                {
                    return true;
                }
                else if(collidingObject == gameObject)
                {
                    return true;
                }
            }
            break;
        }

        return false;
    }

    public void RunEventToggle(BoundEventToggle.EventType type, GameObject gameObject)
    {
        switch(type)
        {
            case BoundEventToggle.EventType.OpenDoor:
            {
                if(gameObject != null && gameObject.TryGetComponent(out Door door))
                {
                    door.ToggleDoorOpen();
                }
            }
            break;

            case BoundEventToggle.EventType.SetCurrentCheckPoint:
            {
                if(gameObject != null && gameObject.TryGetComponent(out CheckPoint checkPoint))
                {
                    checkPoint.SetThisAsCurrentCheckPoint();
                }
            }
            break;

            case BoundEventToggle.EventType.TriggerCheckPoint:
            {
                if(globalData.player.currentCheckPoint != null)
                {
                    globalData.player.currentCheckPoint.TriggerCheckPoint();
                }
            }
            break;

            case BoundEventToggle.EventType.UnlockDoorIfPlayerHasKey:
            {
                if(globalData.player.numberOfKeys > 0 && gameObject != null && gameObject.TryGetComponent(out Door door))
                {
                    if(door.isLocked)
                    {
                        door.SetLocked(false);
                        globalData.player.numberOfKeys -= 1;
                    }
                }
            }
            break;

            case BoundEventToggle.EventType.RecordCheckPoint:
            {
                if(gameObject != null && gameObject.TryGetComponent(out CheckPoint checkPoint))
                {
                    checkPoint.RecordCheckPoint();
                }
            }
            break;

            case BoundEventToggle.EventType.GoToNextLevel:
            {
                globalData.player.gameManager.levelTransition.GoToNextLevel();
            }
            break;
        }
    }

    public void RunEventBool(BoundEventBool.EventType type, GameObject gameObject, bool value)
    {
        switch(type)
        {
            case BoundEventBool.EventType.HinderPlayerMovement:
            {
                if(gameObject != null && gameObject.TryGetComponent(out Player player))
                {
                    player.hinderMovement = value;
                }
            }
            break;

            case BoundEventBool.EventType.SetActive:
            {
                if(gameObject != null)
                {
                    gameObject.SetActive(value);
                }
            }
            break;

            case BoundEventBool.EventType.SetDoorOpen:
            {
                if(gameObject != null && gameObject.TryGetComponent(out Door door))
                {
                    door.SetDoorOpen(value);
                }
            }
            break;

            case BoundEventBool.EventType.SetVisibleOrShadow:
            {
                if(gameObject != null)
                {
                    if(gameObject.TryGetComponent(out MeshRenderer renderer))
                    {
                        if(value)
                        {
                            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                        }
                        else
                        {
                            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                        }
                    }
                    else
                    {
                        for(int it_index = 0; it_index < gameObject.transform.childCount; it_index += 1)
                        {
                            RunEventBool(type, gameObject.transform.GetChild(it_index).gameObject, value);
                        }
                    }
                }
            }
            break;

            case BoundEventBool.EventType.SetDoorLocked:
            {
                if(gameObject != null && gameObject.TryGetComponent(out Door door))
                {
                    door.SetLocked(value);
                }
            }
            break;

            case BoundEventBool.EventType.SetDoorFrozen:
            {
                if(gameObject != null && gameObject.TryGetComponent(out Door door))
                {
                    door.SetFreeze(value);
                }
            }
            break;

            case BoundEventBool.EventType.EnableCrazyTaxiMovement:
            {
                globalData.player.crazyTaxiMovement = value;
            }
            break;
        }
    }

    public void RunEventInt(BoundEventInt.EventType type, GameObject gameObject, int value)
    {
        switch(type)
        {
            case BoundEventInt.EventType.SetKeyCount:
            {
                globalData.player.numberOfKeys = value;
            }
            break;

            case BoundEventInt.EventType.AddKeys:
            {
                globalData.player.numberOfKeys += value;
            }
            break;
        }
    }

    public void RunEventFloat(BoundEventFloat.EventType type, GameObject gameObject, float value)
    {
        switch(type)
        {
            case BoundEventFloat.EventType.SetDeathFloorValue:
            {
                globalData.player.SetDeathFloorValue(value);
            }
            break;

            case BoundEventFloat.EventType.SetPlayerCapsuleRadius:
            {
                globalData.player.characterController.motor.Capsule.radius = value;
            }
            break;
        }
    }

    public void RunEventString(BoundEventString.EventType type, GameObject gameObject, string value)
    {
        switch(type)
        {
            case BoundEventString.EventType.PlaySound:
            {
                if(gameObject != null && gameObject.TryGetComponent(out AudioSource audioSource))
                {
                    globalData.audioList.PlaySoundEffect(audioSource, value);
                }
                else
                {
                    globalData.audioList.PlaySoundEffect(globalData.player.generalSoundEffectSource, value);
                }
            }
            break;

            case BoundEventString.EventType.PlayMusic:
            {
                if(globalData.player.musicSourceB.isPlaying)
                {
                    globalData.player.musicSourceB.Stop();
                }

                globalData.audioList.PlayMusic(globalData.player.musicSourceA, value);
            }
            break;

            case BoundEventString.EventType.PlayMusicCrossFadeFromPrevious:
            {
                if(globalData.player.musicSourceA.isPlaying)
                {
                    globalData.audioList.PlayMusicAndCrossFadeFrom(globalData.player.musicSourceA, globalData.player.musicSourceB, value);
                }
                else if(globalData.player.musicSourceB.isPlaying)
                {
                    globalData.audioList.PlayMusicAndCrossFadeFrom(globalData.player.musicSourceA, globalData.player.musicSourceA, value);
                }
                else
                {
                    globalData.audioList.PlayMusic(globalData.player.musicSourceA, value);
                }
            }
            break;
        }
    }

    public void RunEventVector3(BoundEventVector3.EventType type, GameObject gameObject, Vector3 value)
    {
        switch(type)
        {
            case BoundEventVector3.EventType.SetPosition:
            {
                if(gameObject != null)
                {
                    if(gameObject.transform.TryGetComponent(out KinematicCharacterMotor kinematicCharacterController))
                    {
                        kinematicCharacterController.SetPosition(value, true);
                    }
                    else
                    {
                        gameObject.transform.SetPositionAndRotation(value, gameObject.transform.rotation);
                    }
                }
            }
            break;

            case BoundEventVector3.EventType.SetPositionRelative:
            {
                if(gameObject != null)
                {
                    if(gameObject.transform.TryGetComponent(out KinematicCharacterMotor kinematicCharacterController))
                    {
                        kinematicCharacterController.SetPosition(gameObject.transform.position + value, true);
                    }
                    else
                    {
                        gameObject.transform.SetPositionAndRotation(gameObject.transform.position + value, gameObject.transform.rotation);
                    }
                }
            }
            break;

            case BoundEventVector3.EventType.SetCameraFollowOffset:
            {
                Cinemachine.CinemachineTransposer transposer = globalData.player.virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();

                if(transposer != null)
                {
                    transposer.m_FollowOffset = value;
                }
            }
            break;

            case BoundEventVector3.EventType.SetCameraRotation:
            {
                globalData.player.virtualCamera.transform.SetPositionAndRotation(globalData.player.virtualCamera.transform.position, Quaternion.Euler(value));
            }
            break;
        }
    }

    void RunEvents(GameObject gameObject)
    {
        if(IsRequiredType(gameObject))
        {
            for(int it_index = 0; it_index < toggleEvents.Length; it_index += 1)
            {
                BoundEventToggle it = toggleEvents[it_index];

                if(it.applyToChildren && it.gameObject != null)
                {
                    for(int child_index = 0; child_index < it.gameObject.transform.childCount; child_index += 1)
                    {
                        RunEventToggle(it.type, it.gameObject.transform.GetChild(child_index).gameObject);
                    }
                }
                else
                {
                    RunEventToggle(it.type, it.gameObject);
                }
            }

            for(int it_index = 0; it_index < boolEvents.Length; it_index += 1)
            {
                BoundEventBool it = boolEvents[it_index];

                if(it.applyToChildren && it.gameObject != null)
                {
                    for(int child_index = 0; child_index < it.gameObject.transform.childCount; child_index += 1)
                    {
                        RunEventBool(it.type, it.gameObject.transform.GetChild(child_index).gameObject, it.value);
                    }
                }
                else
                {
                    RunEventBool(it.type, it.gameObject, it.value);
                }
            }

            for(int it_index = 0; it_index < intEvents.Length; it_index += 1)
            {
                BoundEventInt it = intEvents[it_index];

                if(it.applyToChildren && it.gameObject != null)
                {
                    for(int child_index = 0; child_index < it.gameObject.transform.childCount; child_index += 1)
                    {
                        RunEventInt(it.type, it.gameObject.transform.GetChild(child_index).gameObject, it.value);
                    }
                }
                else
                {
                    RunEventInt(it.type, it.gameObject, it.value);
                }
            }

            for(int it_index = 0; it_index < floatEvents.Length; it_index += 1)
            {
                BoundEventFloat it = floatEvents[it_index];

                if(it.applyToChildren && it.gameObject != null)
                {
                    for(int child_index = 0; child_index < it.gameObject.transform.childCount; child_index += 1)
                    {
                        RunEventFloat(it.type, it.gameObject.transform.GetChild(child_index).gameObject, it.value);
                    }
                }
                else
                {
                    RunEventFloat(it.type, it.gameObject, it.value);
                }
            }

            for(int it_index = 0; it_index < stringEvents.Length; it_index += 1)
            {
                BoundEventString it = stringEvents[it_index];

                if(it.applyToChildren && it.gameObject != null)
                {
                    for(int child_index = 0; child_index < it.gameObject.transform.childCount; child_index += 1)
                    {
                        RunEventString(it.type, it.gameObject.transform.GetChild(child_index).gameObject, it.value);
                    }
                }
                else
                {
                    RunEventString(it.type, it.gameObject, it.value);
                }
            }

            for(int it_index = 0; it_index < vector3Events.Length; it_index += 1)
            {
                BoundEventVector3 it = vector3Events[it_index];

                if(it.applyToChildren && it.gameObject != null)
                {
                    for(int child_index = 0; child_index < it.gameObject.transform.childCount; child_index += 1)
                    {
                        RunEventVector3(it.type, it.gameObject.transform.GetChild(child_index).gameObject, it.value);
                    }
                }
                else
                {
                    RunEventVector3(it.type, it.gameObject, it.value);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(trigger == BoundEventTrigger.Enter || trigger == BoundEventTrigger.EnterAndExit)
        {
            RunEvents(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(trigger == BoundEventTrigger.Exit || trigger == BoundEventTrigger.EnterAndExit)
        {
            RunEvents(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(trigger == BoundEventTrigger.Continious)
        {
            RunEvents(other.gameObject);
        }
    }
}
