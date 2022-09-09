using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundScript : MonoBehaviour {

    public bool hasStartedPlaying = false;
    public AudioSource myAudio;

    private void Start() {
        myAudio = GetComponent<AudioSource>();
        myAudio.Play();
    }

    private void Update() {
        if (myAudio.isPlaying) {
            hasStartedPlaying = true;
        }

        if (myAudio.isPlaying == false && hasStartedPlaying == true) {
            Destroy(this.gameObject);
        }
    }
}
