using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO(Patrik): Generic cross fade
[System.Serializable]
public struct AudioListCrossFade {
    public AudioSource sourceFrom;
    public AudioSource sourceTo;

    public bool stopFadedSource;
    public float fadeTime;
    public float totalFadeTime;
};


[System.Serializable]
public struct AudioListFade {
    public AudioSource source;

    public bool stopFadedSource;
    public float fadeTime;
    public float totalFadeTime;
};

[System.Serializable]
public class AudioListEntryClip
{
    public AudioClip clip;

    [Space(8)]
    public bool looping = false;

    [Space(8), Range(-128f, 128f)]
    public int priority = 0;

    [Space(8), Range(-1f, 0f)]
    public float volume;

    [Space(8)]
    [Range(-4f, 2f)]
    public float minPitch = 0f;
    [Range(-4f, 2f)]
    public float maxPitch = 0f;

    [Space(8), Range(-1f, 1f), Tooltip("Pan Stereo: Negative values are left, positive are right")]
    public float panStereo = 0f;
    [Range(-1f, 0f), Tooltip("Spatial Blend: -1 feels 2D while 0 feels 3D")]
    public float spatialBlend = 0f;
}

[System.Serializable]
public struct AudioListEntry
{
    // TODO(Patrik): Intro!!!
    // public string introName;

    public string name;
    public AudioListEntryClip[] entries;
}

[CreateAssetMenu(fileName = "New Audio List", menuName = "Data/Audio List"), System.Serializable]
public class AudioList : ScriptableObject
{
    public AudioListEntry[] soundEffects;
    public AudioListEntry[] music;

    public GameObject prefab;

    [HideInInspector]
    public AudioManager audioManager;


    public void SetSoundEffect(AudioSource source, string name, float time = 0f)
    {
        for(int it_index = 0; it_index < soundEffects.Length; it_index += 1)
        {
            if(soundEffects[it_index].name == name)
            {
                SetRandomFromList(source, soundEffects[it_index], time);

                break;
            }
        }
    }

    public void SetSoundEffect(GameObject gameObject, string name, float time = 0f)
    {
        if(gameObject != null && gameObject.TryGetComponent(out AudioSource source))
        {
            SetSoundEffect(source, name, time);
        }
    }


    public void PlaySoundEffect(string name, float time = 1f)
    {
        for(int it_index = 0; it_index < soundEffects.Length; it_index += 1)
        {
            if(soundEffects[it_index].name == name)
            {
                //PlayOneShotRandomFromList(, soundEffects[it_index], time);

                break;
            }
        }
    }

    public void PlaySoundEffect(AudioSource source, string name, float time = 0f)
    {
        for(int it_index = 0; it_index < soundEffects.Length; it_index += 1)
        {
            if(soundEffects[it_index].name == name)
            {
                PlayRandomFromList(source, soundEffects[it_index], time);

                break;
            }
        }
    }

    public void PlaySoundEffect(GameObject gameObject, string name, float time = 0f)
    {
        if(gameObject != null && gameObject.TryGetComponent(out AudioSource source))
        {
            PlaySoundEffect(source, name, time);
        }
    }

    public void PlaySoundEffectAndDestroy(Vector3 pos, string name, float time = 0f)
    {
        Instantiate(prefab, pos, Quaternion.identity);

        for(int it_index = 0; it_index < soundEffects.Length; it_index += 1)
        {
            if(soundEffects[it_index].name == name)
            {
                AudioListEntryClip entryClip = PlayRandomFromList(prefab.GetComponent<AudioSource>(), soundEffects[it_index], time);

                break;
            }
        }
    }


    public void StopSoundEffect(AudioSource source, string name)
    {
        if(source.isPlaying)
        {
            for(int it_index = 0; it_index < soundEffects.Length; it_index += 1)
            {
                if(soundEffects[it_index].name == name)
                {
                    for(int entry_index = 0; entry_index < soundEffects[it_index].entries.Length; entry_index += 1)
                    {
                        if(soundEffects[it_index].entries[entry_index].clip == source.clip)
                        {
                            source.Stop();
                            return;
                        }
                    }
                }
            }
        }
    }

    public void StopSoundEffect(GameObject gameObject, string name)
    {
        if(gameObject != null && gameObject.TryGetComponent(out AudioSource source))
        {
            StopSoundEffect(source, name);
        }
    }


    public bool IsSoundEffectPlaying(AudioSource source, string name)
    {
        if(source.isPlaying)
        {
            for(int it_index = 0; it_index < soundEffects.Length; it_index += 1)
            {
                if(soundEffects[it_index].name == name)
                {
                    for(int entry_index = 0; entry_index < soundEffects[it_index].entries.Length; entry_index += 1)
                    {
                        if(source.clip == soundEffects[it_index].entries[entry_index].clip)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public bool IsSoundEffectPlaying(GameObject gameObject, string name)
    {
        if(gameObject != null && gameObject.TryGetComponent(out AudioSource source))
        {
            return IsSoundEffectPlaying(source, name);
        }
        return false;
    }



    public void SetMusic(AudioSource source, string name, float time = 0f)
    {
        for(int it_index = 0; it_index < music.Length; it_index += 1)
        {
            if(music[it_index].name == name)
            {
                SetRandomFromList(source, music[it_index], time);

                break;
            }
        }
    }

    public void SetMusic(GameObject gameObject, string name, float time = 0f)
    {
        if(gameObject != null && gameObject.TryGetComponent(out AudioSource source))
        {
            SetMusic(source, name, time);
        }
    }

    public void PlayMusic(AudioSource source, string name, float time = 0f)
    {
        for(int it_index = 0; it_index < music.Length; it_index += 1)
        {
            if(music[it_index].name == name)
            {
                PlayRandomFromList(source, music[it_index], time);

                break;
            }
        }
    }

    public void PlayMusic(GameObject gameObject, string name, float time = 0f)
    {
        if(gameObject != null && gameObject.TryGetComponent(out AudioSource source))
        {
            PlayMusic(source, name, time);
        }
    }

    public void PlayMusicAndCrossFadeFrom(AudioSource from, AudioSource to, string name)
    {
        SetMusic(to, name);

        to.time = from.time;
        to.volume = 0f;
        if(!to.isPlaying)
        {
            to.Play();
        }

        AppendCrossFade(from, to, 1f, true);
    }


    public void StopMusic(AudioSource source, string name)
    {
        if(source.isPlaying)
        {
            for(int it_index = 0; it_index < soundEffects.Length; it_index += 1)
            {
                if(soundEffects[it_index].name == name)
                {
                    for(int entry_index = 0; entry_index < soundEffects[it_index].entries.Length; entry_index += 1)
                    {
                        if(soundEffects[it_index].entries[entry_index].clip == source.clip)
                        {
                            source.Stop();
                            return;
                        }
                    }
                }
            }
        }
    }

    public void StopMusic(GameObject gameObject, string name)
    {
        if(gameObject != null && gameObject.TryGetComponent(out AudioSource source))
        {
            StopMusic(source, name);
        }
    }


    public bool IsMusicPlaying(AudioSource source, string name)
    {
        if(source.isPlaying)
        {
            for(int it_index = 0; it_index < music.Length; it_index += 1)
            {
                if(music[it_index].name == name)
                {
                    for(int entry_index = 0; entry_index < music[it_index].entries.Length; entry_index += 1)
                    {
                        if(source.clip == music[it_index].entries[entry_index].clip)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public bool IsMusicPlaying(GameObject gameObject, string name)
    {
        if(gameObject != null && gameObject.TryGetComponent(out AudioSource source))
        {
            return IsSoundEffectPlaying(source, name);
        }
        return false;
    }


    public void AppendCrossFade(AudioListCrossFade fade)
    {
        if(audioManager != null)
        {
            if(fade.sourceFrom.isPlaying && fade.sourceTo.isPlaying)
            {
                audioManager.AppendCrossFade(fade);
            }
        }
    }

    public void AppendCrossFade(AudioSource from, AudioSource to, float totalFadeTime, bool stopFadedSource = true)
    {
        AudioListCrossFade fade = new AudioListCrossFade();
        fade.sourceFrom = from;
        fade.sourceTo = to;
        fade.totalFadeTime = totalFadeTime;
        fade.stopFadedSource = stopFadedSource;

        AppendCrossFade(fade);
    }

    public void AppendFade(AudioListFade fade)
    {
        if(audioManager != null)
        {
            audioManager.AppendFade(fade);
        }
    }

    public void AppendFade(AudioSource source, float totalFadeTime, bool stopFadedSource = true)
    {
        AudioListFade fade = new AudioListFade();
        fade.source = source;
        fade.totalFadeTime = totalFadeTime;
        fade.stopFadedSource = stopFadedSource;

        AppendFade(fade);
    }




    void SetAudioListEntryClip(AudioSource source, AudioListEntryClip entry, float time = 0f)
    {
        source.clip = entry.clip;
        source.volume = entry.volume + 1f;
        source.pitch = Random.Range(entry.minPitch, entry.maxPitch) + 1f;
        source.spread = 180;
        source.loop = entry.looping;
        source.priority = entry.priority + 128;
        source.panStereo = entry.panStereo;
        source.spatialBlend = entry.spatialBlend + 1f;
        source.time = time;
    }

    void PlayAudioListEntryClip(AudioSource source, AudioListEntryClip entry, float time = 0f)
    {
        SetAudioListEntryClip(source, entry, time);
        source.Play();
    }

    void PlayScheduledAudioListEntryClip(AudioSource source, AudioListEntryClip entry, double time)
    {
        SetAudioListEntryClip(source, entry);
        source.PlayScheduled(time);
    }


    AudioListEntryClip SetFromList(AudioSource source, AudioListEntry list, int index, float time = 0f)
    {
        SetAudioListEntryClip(source, list.entries[index], time);
        return list.entries[index];
    }

    AudioListEntryClip SetRandomFromList(AudioSource source, AudioListEntry list, float time = 0f)
    {
        if(list.entries.Length > 1)
        {
            return SetFromList(source, list, Random.Range(0, list.entries.Length - 1), time);
        }
        else if(list.entries.Length == 1)
        {
            return SetFromList(source, list, 0, time);
        }
        return null;
    }

    AudioListEntryClip PlayRandomFromList(AudioSource source, AudioListEntry list, float time = 0f)
    {
        AudioListEntryClip result = SetRandomFromList(source, list, time);
        if(result != null)
        {
            source.Play();
        }
        return result;
    }

    AudioListEntryClip PlayScheduledRandomFromList(AudioSource source, AudioListEntry list, double time)
    {
        AudioListEntryClip result = SetRandomFromList(source, list, 0f);
        if(result != null)
        {
            source.PlayScheduled(time);
        }
        return result;
    }
}
