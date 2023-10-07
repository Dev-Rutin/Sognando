using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager_s : MonoBehaviour,IAudio
{
    private Dictionary<AudioClip, AudioSource> _audioDic;
    void Start()
    {
        _audioDic = new Dictionary<AudioClip, AudioSource>(); 
    }
    public void AudioPlay(AudioClip clip)
    {
        if(!_audioDic.ContainsKey(clip))
        {
            this.AddComponent<AudioSource>();
            _audioDic.Add(clip, this.GetComponents<AudioSource>()[_audioDic.Count]);
            _audioDic[clip].clip = clip;
        }
        _audioDic[clip].Play();

    }
    public void AudioStop(AudioClip clip) 
    {
        if (_audioDic.ContainsKey(clip))
        {
            _audioDic[clip].Stop();
        }
    }
    public void AudioPause()
    {
        foreach (var data in _audioDic)
        {
            data.Value.Stop();
        }
    }
    public void AudioUnPause(TimeSpan _time)
    {
        foreach(var data in _audioDic)
        {    
            if(data.Key.length>=_time.TotalSeconds)
            {
                data.Value.time = (float)_time.TotalSeconds;
            }
            data.Value.Play();
        }       
    }
    public void ChangeVolume(float volume) { }
}
