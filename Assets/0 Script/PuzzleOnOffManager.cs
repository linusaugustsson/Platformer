using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleOnOffManager : MonoBehaviour
{
    public GameObject[] entries;
    public Transform parent;

    public Material onMaterial;
    public Material offMaterial;

    public bool currentState;
    public bool previousState;

    GameObject[] GetObjects()
    {
        int objectCount = 0;

        if(entries.Length > 0)
        {
            objectCount += entries.Length;
        }

        if(parent != null)
        {
            objectCount += parent.childCount;
        }

        GameObject[] result = new GameObject[objectCount];

        for(int it_index = 0; it_index < entries.Length; it_index += 1)
        {
            result[it_index] = entries[it_index];
        }

        if(parent != null)
        {
            for(int it_index = 0; it_index < parent.childCount; it_index += 1)
            {
                result[it_index + entries.Length] = parent.GetChild(it_index).gameObject;
            }
        }

        return result;
    }

    void SetOnOffState()
    {
        GameObject[] objects = GetObjects();

        for(int it_index = 0; it_index < objects.Length; it_index += 1)
        {
            Transform it = objects[it_index].transform;

            if(it.TryGetComponent(out PuzzleOnOff onOff))
            {
                onOff.SetState(currentState);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        previousState = currentState;

        if(onMaterial != null || offMaterial != null)
        {
            GameObject[] objects = GetObjects();
            for(int it_index = 0; it_index < objects.Length; it_index += 1)
            {
                if(objects[it_index].transform.TryGetComponent(out PuzzleOnOff onOff))
                {
                    if(onMaterial != null && onOff.onMaterial == null)
                    {
                        onOff.onMaterial = onMaterial;
                    }

                    if(offMaterial != null && onOff.offMaterial == null)
                    {
                        onOff.offMaterial = offMaterial;
                    }
                }
            }
        }

        SetOnOffState();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState != previousState)
        {
            SetOnOffState();
        }

        previousState = currentState;
    }
}
