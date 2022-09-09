using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessScript : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;

    [HideInInspector]
    public ColorGrading colorGrading;
    [HideInInspector]
    public Vignette vignette;
    [HideInInspector]
    public AmbientOcclusion ambientOcclusion;
    [HideInInspector]
    public Grain filmGrain;
    [HideInInspector]
    public Bloom bloom;
    [HideInInspector]
    public ChromaticAberration chromaticAberration;




    public bool startGlasses = false;
    public bool endGlasses = false;
    private float chromaticTimeElapsed = 0.0f;
    private float chromaticTimeDuration = 0.1f;
    private float chromaticValue = 0.0f;

    private float chromaticStartValue = 0.0f;
    private float chromaticEndValue = 1.0f;


    public float glassesOnTint = -1.0f;
    public float glassesOffTint = 0.0f;

    public void ChangedGlasses() {
        //chromaticTimeElapsed = 0.0f;
        startGlasses = true;
    }

    public void GlassesOn() {
        colorGrading.postExposure.value = glassesOnTint;
    }

    public void GlassesOff() {
        colorGrading.postExposure.value = glassesOffTint;
    }

    private void Start() {
        GetPostProcessingEffects();
    }

    private void Update() {

        if(startGlasses == true) {
            if(chromaticTimeElapsed < chromaticTimeDuration) {
                chromaticValue = Mathf.Lerp(chromaticStartValue, chromaticEndValue, chromaticTimeElapsed / chromaticTimeDuration);
                chromaticTimeElapsed += Time.deltaTime;
                chromaticAberration.intensity.value = chromaticValue;
            }
            if(chromaticTimeElapsed > chromaticTimeDuration) {
                startGlasses = false;
                endGlasses = true;
                chromaticTimeElapsed = 0.0f;
            }
        }
        if(endGlasses == true) {
            if (chromaticTimeElapsed < chromaticTimeDuration) {
                chromaticValue = Mathf.Lerp(chromaticEndValue, chromaticStartValue, chromaticTimeElapsed / chromaticTimeDuration);
                chromaticTimeElapsed += Time.deltaTime;
                chromaticAberration.intensity.value = chromaticValue;
            }
            if (chromaticTimeElapsed > chromaticTimeDuration) {
                startGlasses = false;
                endGlasses = false;
                chromaticTimeElapsed = 0.0f;
            }
        }
        

    }

    public void GetPostProcessingEffects() {
        ColorGrading tmpColorGrade;
        Vignette tmpVignette;
        AmbientOcclusion tmpAmbientOcclusion;
        Grain tmpFilmGrain;
        Bloom tmpBloom;
        ChromaticAberration tmpChromaticAberration;


        if(postProcessVolume.profile.TryGetSettings<ColorGrading>(out tmpColorGrade)) {
            colorGrading = tmpColorGrade;
        }

        if (postProcessVolume.profile.TryGetSettings<Vignette>(out tmpVignette)) {
            vignette = tmpVignette;
        }


        if (postProcessVolume.profile.TryGetSettings<AmbientOcclusion>(out tmpAmbientOcclusion)) {
            ambientOcclusion = tmpAmbientOcclusion;
        }

        if (postProcessVolume.profile.TryGetSettings<Grain>(out tmpFilmGrain)) {
            filmGrain = tmpFilmGrain;
        }

        if (postProcessVolume.profile.TryGetSettings<Bloom>(out tmpBloom)) {
            bloom = tmpBloom;
        }

        if (postProcessVolume.profile.TryGetSettings<ChromaticAberration>(out tmpChromaticAberration)) {
            chromaticAberration = tmpChromaticAberration;
        }

    }


    

}
