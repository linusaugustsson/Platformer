using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderRoomTrigger : MonoBehaviour
{

    public MeshRenderer meshRenderer;

    public Color fullColor;
    public Color fadeColor;


    public float maxFadeTime = 0.5f;
    private float currentFadeTime = 0.0f;
    private Color currentColor;

    private bool lerpToFull = false;
    private bool lerpToFade = false;

    private void Awake() {
        currentColor = meshRenderer.material.color;
    }

    private void Update() {
        if(lerpToFade) {
            currentFadeTime += Time.deltaTime;

            currentColor = Color.Lerp(fullColor, fadeColor, currentFadeTime / maxFadeTime);
            meshRenderer.material.color = currentColor;

            if (currentFadeTime >= maxFadeTime) {
                lerpToFade = false;
                currentFadeTime = 0.0f;
            }
        }

        if(lerpToFull) {
            currentFadeTime += Time.deltaTime;

            currentColor = Color.Lerp(fadeColor, fullColor, currentFadeTime / maxFadeTime);
            meshRenderer.material.color = currentColor;

            if (currentFadeTime >= maxFadeTime) {
                lerpToFull = false;
                currentFadeTime = 0.0f;
            }
        }


    }


    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player") {
            //meshRenderer.material.color = fadeColor;
            lerpToFull = false;
            lerpToFade = true;
        }
    }


    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            //meshRenderer.material.color = fullColor;

            lerpToFull = true;
            lerpToFade = false;

        }
    }

}
