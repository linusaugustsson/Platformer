using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;


[System.Serializable]
public struct CheckPointTransform {
    public Transform transform;
    public bool applyToChildren;
};

[System.Serializable]
public struct CheckPointDoorData
{
    public bool wasFrozen;
    public bool wasLocked;
    public bool wasOpen;
}

[System.Serializable]
public struct CheckPointPosition {
    public Vector3[] entries;
};

[System.Serializable]
public struct CheckPointBool {
    public bool[] entries;
};

public class CheckPoint : MonoBehaviour
{
    public GlobalData globalData;

    [Space(4)]
    public bool triggerOnAwake = true;
    public bool recordNumberOfKeys = true;
    public bool setThisAsCurrentOnAwake;

    [Space(4)]
    public bool tmpTriggerCheckPoint;

    [Space(4)]
    public CheckPointTransform[] positionsToReset;
    public CheckPointTransform[] activeObjectsToReset;
    public Door[] doorsToReset;

    [HideInInspector]
    public CheckPointPosition[] positions;
    [HideInInspector]
    public CheckPointBool[] activeObjects;
    [HideInInspector]
    public CheckPointDoorData[] doorEntries;
    [HideInInspector]
    public int numberOfKeys;

#if false
    // NOTE TODO(Patrik): Possibly???? Maybe???
    public Quaternion playerRotationWhenEntered;
#endif

    public void SetThisAsCurrentCheckPoint()
    {
        globalData.player.currentCheckPoint = this;
    }

    void SetPosition(Transform transform, Vector3 position)
    {
        if(transform.TryGetComponent(out KinematicCharacterMotor kinematicCharacterController))
        {
            kinematicCharacterController.SetPosition(position, true);
            kinematicCharacterController.BaseVelocity = new Vector3(0f, 0f, 0f);
        }
        else
        {
           transform.SetPositionAndRotation(position, transform.rotation);
        }
    }

    public void TriggerCheckPoint()
    {
        globalData.player.OnCheckpointTriggered(transform.position);

        for(int it_index = 0; it_index < positionsToReset.Length; it_index += 1)
        {
            CheckPointTransform toReset = positionsToReset[it_index];
            Vector3[] recorded = positions[it_index].entries;

            if(toReset.transform != null)
            {
                if(toReset.applyToChildren)
                {
                    for(int child_index = 0; child_index < toReset.transform.childCount; child_index += 1)
                    {
                        SetPosition(toReset.transform.GetChild(child_index), recorded[child_index]);
                    }
                }
                else
                {
                    SetPosition(toReset.transform, recorded[0]);
                }
            }
        }

        for(int it_index = 0; it_index < activeObjectsToReset.Length; it_index += 1)
        {
            CheckPointTransform toReset = activeObjectsToReset[it_index];
            bool[] recorded = activeObjects[it_index].entries;

            if(toReset.transform != null)
            {
                if(toReset.applyToChildren)
                {
                    for(int child_index = 0; child_index < toReset.transform.childCount; child_index += 1)
                    {
                        toReset.transform.GetChild(child_index).gameObject.SetActive(recorded[child_index]);
                    }
                }
                else
                {
                    toReset.transform.gameObject.SetActive(recorded[0]);
                }
            }
        }

        for(int it_index = 0; it_index < doorsToReset.Length; it_index += 1)
        {
            Door toReset = doorsToReset[it_index];
            ref CheckPointDoorData recorded = ref doorEntries[it_index];

            if(toReset != null)
            {
                toReset.SetDoorState(recorded.wasOpen, recorded.wasLocked, recorded.wasFrozen);
            }
        }

        if(recordNumberOfKeys)
        {
            globalData.player.numberOfKeys = numberOfKeys;
        }
    }

    public void RecordCheckPoint()
    {
        positions = new CheckPointPosition[positionsToReset.Length];
        for(int it_index = 0; it_index < positionsToReset.Length; it_index += 1)
        {
            CheckPointTransform toReset = positionsToReset[it_index];

            if(toReset.transform != null)
            {
                if(toReset.applyToChildren)
                {
                    positions[it_index].entries = new Vector3[toReset.transform.childCount];
                    for(int child_index = 0; child_index < toReset.transform.childCount; child_index += 1)
                    {
                        positions[it_index].entries[child_index] = toReset.transform.GetChild(child_index).position;
                    }
                }
                else
                {
                    positions[it_index].entries = new Vector3[1];
                    positions[it_index].entries[0] = toReset.transform.position;
                }
            }
        }

        activeObjects = new CheckPointBool[activeObjectsToReset.Length];
        for(int it_index = 0; it_index < activeObjectsToReset.Length; it_index += 1)
        {
            CheckPointTransform toReset = activeObjectsToReset[it_index];
            ref bool[] recorded = ref activeObjects[it_index].entries;

            if(toReset.transform != null)
            {
                if(toReset.applyToChildren)
                {
                    recorded = new bool[toReset.transform.childCount];
                    for(int child_index = 0; child_index < toReset.transform.childCount; child_index += 1)
                    {
                        recorded[child_index] = toReset.transform.GetChild(child_index).gameObject.activeSelf;
                    }
                }
                else
                {
                    recorded = new bool[1];
                    recorded[0] = transform.gameObject.activeSelf;
                }
            }
        }

        doorEntries = new CheckPointDoorData[doorsToReset.Length];
        for(int it_index = 0; it_index < doorsToReset.Length; it_index += 1)
        {
            Door toReset = doorsToReset[it_index];
            ref CheckPointDoorData recorded = ref doorEntries[it_index];

            if(toReset != null)
            {
                recorded.wasFrozen = toReset.isFrozen;
                recorded.wasLocked = toReset.isLocked;
                recorded.wasOpen   = toReset.isOpen;
            }
        }

        if(recordNumberOfKeys)
        {
            numberOfKeys = globalData.player.numberOfKeys;
        }
    }

    void Update()
    {
        if(tmpTriggerCheckPoint)
        {
            tmpTriggerCheckPoint = false;

            TriggerCheckPoint();
        }
    }

    void Awake()
    {
        if(triggerOnAwake)
        {
            RecordCheckPoint();
        }

        if(setThisAsCurrentOnAwake)
        {
            SetThisAsCurrentCheckPoint();
        }
    }
}
