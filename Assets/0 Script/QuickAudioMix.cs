using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickAudioMix : MonoBehaviour
{
    public GlobalData globalData;

    public string soundEffectName;
    public string musicName;

    public bool playSoundEffect;
    public bool playMusic;

    public bool stopSoundEffect;
    public bool stopMusic;

    public AudioSource sfx;
    public AudioSource music;



    // Update is called once per frame
    void Update()
    {
        if(playSoundEffect)
        {
            playSoundEffect = false;
            globalData.audioList.PlaySoundEffect(sfx, soundEffectName);
        }

        if(playMusic)
        {
            playMusic = false;
            globalData.audioList.PlayMusic(music, musicName);
        }

        if(stopSoundEffect)
        {
            stopSoundEffect = false;
            sfx.Stop();
        }

        if(stopMusic)
        {
            stopMusic = false;
            music.Stop();
        }
    }
}
