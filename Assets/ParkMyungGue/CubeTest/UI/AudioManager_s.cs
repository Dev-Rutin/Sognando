using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager_s : MonoBehaviour,IAudio
{
    List<AudioClip> curAudiolist;
    AudioSource audioSource;
    void Start()
    {
        curAudiolist = new List<AudioClip>();
        audioSource = GetComponent<AudioSource>();  
    }
    public void AudioPlay(AudioClip clip)
    {
        curAudiolist.Add(clip);
        audioSource.clip = clip;
        audioSource.Play();

    }
    public void AudioStop(AudioClip clip) 
    {
        audioSource.clip = clip;
        audioSource.Stop();
        if(curAudiolist.Contains(clip))
            curAudiolist.Remove(clip);
    }
    public void AudioPause()
    {
        foreach (var data in curAudiolist)
        {
            audioSource.clip = data;
            audioSource.Pause();
        }
    }
    public void AudioUnPause(TimeSpan _time)
    {
        foreach(var data in curAudiolist)
        {    
                audioSource.clip = data;
                audioSource.time = (float)_time.TotalSeconds;
                audioSource.Play();
        }       
    }
    public void ChangeVolume(float volume) { }
}
