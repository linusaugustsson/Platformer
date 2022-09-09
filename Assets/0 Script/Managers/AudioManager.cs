using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GlobalData globalData;

    public int crossFadeCount;
    public AudioListCrossFade[] crossFades;

    public int fadeCount;
    public AudioListFade[] fades;

    public void AppendCrossFade(AudioListCrossFade fade)
    {
        if(crossFadeCount < 16)
        {
            crossFades[crossFadeCount] = fade;
            crossFadeCount += 1;
        }
    }

    public void AppendFade(AudioListFade fade)
    {
        if(fadeCount < 16)
        {
            fades[fadeCount] = fade;
            fadeCount += 1;
        }
    }

    void PopCrossFade(int index)
    {
        if(index < crossFadeCount)
        {
            crossFades[index] = crossFades[crossFadeCount];
            crossFades[crossFadeCount] = new AudioListCrossFade();
        }
        crossFadeCount -= 1;
    }

    void PopFade(int index)
    {
        if(index < crossFadeCount)
        {
            fades[index] = fades[fadeCount];
            fades[fadeCount] = new AudioListFade();
        }
        fadeCount -= 1;
    }

    void Start()
    {
        globalData.audioList.audioManager = this;

        crossFades = new AudioListCrossFade[16];
        fades = new AudioListFade[16];
    }

    void Update()
    {
        for(int fade_index = crossFadeCount - 1; fade_index >= 0; fade_index -= 1)
        {
            if(crossFades[fade_index].fadeTime < crossFades[fade_index].totalFadeTime)
            {
                float t = crossFades[fade_index].fadeTime / crossFades[fade_index].totalFadeTime;

                crossFades[fade_index].sourceFrom.volume = 1f - t;
                crossFades[fade_index].sourceTo.volume = t;

                crossFades[fade_index].stopFadedSource = true;
                crossFades[fade_index].fadeTime += Time.deltaTime;
            }
            else
            {
                crossFades[fade_index].sourceFrom.volume = 0f;
                crossFades[fade_index].sourceTo.volume = 1f;

                if(crossFades[fade_index].stopFadedSource)
                {
                    crossFades[fade_index].sourceFrom.Stop();
                }

                PopCrossFade(fade_index);
            }
        }

        for(int fade_index = fadeCount - 1; fade_index >= 0; fade_index -= 1)
        {
            if(fades[fade_index].fadeTime < fades[fade_index].totalFadeTime)
            {
                float t = fades[fade_index].fadeTime / fades[fade_index].totalFadeTime;

                fades[fade_index].source.volume = 1f - t;
                fades[fade_index].fadeTime += Time.deltaTime;
            }
            else
            {
                fades[fade_index].source.volume = 0f;

                if(fades[fade_index].stopFadedSource)
                {
                    fades[fade_index].source.Stop();
                }

                PopCrossFade(fade_index);
            }
        }
    }
}
