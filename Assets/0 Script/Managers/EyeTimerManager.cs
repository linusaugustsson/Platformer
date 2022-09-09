using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EyeTimerManager : MonoBehaviour
{
    public GlobalData globalData;
    public AudioSource audioSource;

    [Space(4)]
    public int numberOfBeeps = 2;

    [Space(4)]
    public float waitInterval = 1f;
    public float beepInterval = 0.5f;

    [Space(4)]
    public CheckPointTransform[] eyeTransforms;

    [Space(4)]
    public Material openMaterial;
    public Material halfOpenMaterial;
    public Material halfClosedMaterial;
    public Material closedMaterial;

    [HideInInspector]
    public int currentStage;
    [HideInInspector]
    public float currentTime;
    [HideInInspector]
    public bool isOpen;

    [HideInInspector]
    public EyeTimer[] eyes;

    void InitEyeTimer(EyeTimer eye)
    {
        eye.initialScale = eye.eyeQuad.transform.localScale;

        if(eye.isInverted)
        {
            eye.visualBeam.SetActive(!isOpen);

            if(isOpen)
            {
                eye.eyeRenderer.material = closedMaterial;
            }
            else
            {
                eye.eyeRenderer.material = openMaterial;
            }
        }
        else
        {
            eye.visualBeam.SetActive(isOpen);

            if(isOpen)
            {
                eye.eyeRenderer.material = openMaterial;
            }
            else
            {
                eye.eyeRenderer.material = closedMaterial;
            }
        }
    }

    void Start()
    {
        if(eyeTransforms.Length > 0)
        {
            int eyeCount = 0;

            for(int it_index = 0; it_index < eyeTransforms.Length; it_index += 1)
            {
                if(eyeTransforms[it_index].applyToChildren)
                {
                    for(int child_index = 0; child_index < eyeTransforms[it_index].transform.childCount; child_index += 1)
                    {
                        if(eyeTransforms[it_index].transform.GetChild(child_index).TryGetComponent(out EyeTimer eye))
                        {
                            eyeCount += 1;
                        }
                    }
                }
                else
                {
                    if(eyeTransforms[it_index].transform.TryGetComponent(out EyeTimer eye))
                    {
                        eyeCount += 1;
                    }
                }
            }

            eyes = new EyeTimer[eyeCount];
            eyeCount = 0;
            for(int it_index = 0; it_index < eyeTransforms.Length; it_index += 1)
            {
                if(eyeTransforms[it_index].applyToChildren)
                {
                    for(int child_index = 0; child_index < eyeTransforms[it_index].transform.childCount; child_index += 1)
                    {
                        if(eyeTransforms[it_index].transform.GetChild(child_index).TryGetComponent(out EyeTimer eye))
                        {
                            InitEyeTimer(eye);
                            eyes[eyeCount] = eye;
                            eyeCount += 1;
                        }
                    }
                }
                else
                {
                    if(eyeTransforms[it_index].transform.TryGetComponent(out EyeTimer eye))
                    {
                        InitEyeTimer(eye);
                        eyes[eyeCount] = eye;
                        eyeCount += 1;
                    }
                }
            }
        }
        else
        {
            eyes = new EyeTimer[transform.childCount];
            for(int child_index = 0; child_index < transform.childCount; child_index += 1)
            {
                Transform child = transform.GetChild(child_index);

                if(child.TryGetComponent(out EyeTimer it))
                {
                    eyes[child_index] = it;
                    InitEyeTimer(it);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentStage > 0)
        {
            float oneOverBeepInterval = 1f / beepInterval;
            float s = ((Mathf.Sin(currentTime * oneOverBeepInterval * 6f) + 1f) * 0.125f) + 1f;

            for(int it_index = 0; it_index < eyes.Length; it_index += 1)
            {
                ref EyeTimer it = ref eyes[it_index];
                it.eyeQuad.transform.localScale = new Vector3(it.initialScale.x * s, it.initialScale.y * s, it.initialScale.z * s);
            }
        }

        if(currentStage == 0)
        {
            if(currentTime >= waitInterval)
            {
                currentTime = 0f;
                currentStage += 1;
                globalData.audioList.PlaySoundEffect(audioSource, "timer_eye_beep");

                for(int it_index = 0; it_index < eyes.Length; it_index += 1)
                {
                    ref EyeTimer it = ref eyes[it_index];

                    if(it.isInverted)
                    {
                        if(isOpen)
                        {
                            it.eyeRenderer.material = halfClosedMaterial;
                        }
                        else
                        {
                            it.eyeRenderer.material = halfOpenMaterial;
                        }
                    }
                    else
                    {
                        if(isOpen)
                        {
                            it.eyeRenderer.material = halfOpenMaterial;
                        }
                        else
                        {
                            it.eyeRenderer.material = halfClosedMaterial;
                        }
                    }
                }
            }
        }
        else if(currentStage < numberOfBeeps)
        {
            if(currentTime >= beepInterval)
            {
                currentTime = 0f;
                currentStage += 1;
                globalData.audioList.PlaySoundEffect(audioSource, "timer_eye_beep");

                if(currentStage == numberOfBeeps)
                {
                    for(int it_index = 0; it_index < eyes.Length; it_index += 1)
                    {
                        ref EyeTimer it = ref eyes[it_index];

                        if(it.isInverted)
                        {
                            if(isOpen)
                            {
                                it.eyeRenderer.material = halfOpenMaterial;
                            }
                            else
                            {
                                it.eyeRenderer.material = halfClosedMaterial;
                            }
                        }
                        else
                        {
                            if(isOpen)
                            {
                                it.eyeRenderer.material = halfClosedMaterial;
                            }
                            else
                            {
                                it.eyeRenderer.material = halfOpenMaterial;
                            }
                        }
                    }
                }
            }
        }
        else if(currentStage == numberOfBeeps)
        {
            if(currentTime >= beepInterval)
            {
                currentTime = 0f;
                currentStage = 0;

                isOpen = !isOpen;
                if(isOpen)
                {
                    globalData.audioList.PlaySoundEffect(audioSource, "timer_eye_open");
                }
                else
                {
                    globalData.audioList.PlaySoundEffect(audioSource, "timer_eye_close");
                }

                for(int it_index = 0; it_index < eyes.Length; it_index += 1)
                {
                    ref EyeTimer it = ref eyes[it_index];
                    it.eyeQuad.transform.localScale = it.initialScale;

                    if(it.isInverted)
                    {
                        it.visualBeam.SetActive(!isOpen);

                        if(isOpen)
                        {
                            it.eyeRenderer.material = closedMaterial;
                        }
                        else
                        {
                            it.eyeRenderer.material = openMaterial;
                        }
                    }
                    else
                    {
                        it.visualBeam.SetActive(isOpen);

                        if(isOpen)
                        {
                            it.eyeRenderer.material = openMaterial;
                        }
                        else
                        {
                            it.eyeRenderer.material = closedMaterial;
                        }
                    }
                }
            }
        }

        currentTime += Time.deltaTime;
    }
}
