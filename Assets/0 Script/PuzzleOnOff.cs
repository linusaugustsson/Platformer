using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleOnOff : MonoBehaviour
{
    public Material onMaterial;
    public Material offMaterial;

    public bool currentState;

    public void SetState(bool enabled)
    {
        currentState = enabled;

        if(transform.TryGetComponent(out Collider collider))
        {
            collider.enabled = enabled;
        }

        if(transform.TryGetComponent(out MeshRenderer renderer))
        {
            if(enabled)
            {
                renderer.material = onMaterial;
            }
            else
            {
                renderer.material = offMaterial;
            }
        }
    }
}
