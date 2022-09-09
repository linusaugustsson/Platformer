using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {



    /*
    public GameObject menuClickObject;
    public GameObject screenClickObject;
    public GameObject teleportObject;
    public GameObject openMenuObject;
    public GameObject closeMenuObject;
    */

    //public AudioSource monitorSource;
    //public AudioSource natureSource;

    //public bool ambientIsPlaying = true;

    public GlobalData globalData;

    public GameObject breakableHit;
    public GameObject webBreak;

    private void Awake() {
        globalData.soundManager = this;
    }

    public void PlaySound(Vector3 _position, GameObject _sound) {
        Instantiate(_sound, _position, Quaternion.identity);
    }


    /*
    public void PlayClickedMenu(Vector3 _position) {
        Instantiate(menuClickObject, _position, Quaternion.identity);
    }

    public void PlayClickedScreen(Vector3 _position) {
        Instantiate(screenClickObject, _position, Quaternion.identity);
    }

    public void PlayTeleported(Vector3 _position) {
        Instantiate(teleportObject, _position, Quaternion.identity);
    }

    public void PlayOpenMenu(Vector3 _position) {
        Instantiate(openMenuObject, _position, Quaternion.identity);
    }

    public void PlayCloseMenu(Vector3 _position) {
        Instantiate(closeMenuObject, _position, Quaternion.identity);
    }
    /*

    /*
    public void TurnOffAmbientSounds() {
        ambientIsPlaying = false;

        monitorSource.Stop();
        natureSource.Stop();
    }

    public void TurnOnAmbientSounds() {
        ambientIsPlaying = true;

        monitorSource.Play();
        natureSource.Play();
    }
    */

    public void AdjustVolume() {

    }






}
