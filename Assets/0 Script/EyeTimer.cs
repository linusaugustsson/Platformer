using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeTimer : MonoBehaviour
{
    public bool isInverted;
    public GameObject eyeQuad;
    public GameObject visualBeam;
    public EyeBeam fullBeam;
    public MeshRenderer eyeRenderer;

    [HideInInspector]
    public Vector3 initialScale;
    [HideInInspector]
    public float initialBeamRange;
}
