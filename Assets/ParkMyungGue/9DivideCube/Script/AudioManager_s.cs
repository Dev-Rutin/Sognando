using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
/*public interface IAudio
{
    public void AudioPlay(AudioClip clip);
    public void AudioStop(AudioClip clip);
    public void AudioPause();
    public void AudioUnPause(TimeSpan time);
    public void ChangeVolume(float volume);
}*/
public partial class AudioManager_s : MonoBehaviour,IAudio
{
    public Dictionary<AudioClip, AudioSource> _audioDic;
    void Start()
    {
        _audioDic = new Dictionary<AudioClip, AudioSource>();
        inputDelay = 0;
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
        if(_audioDic.Count==1)
        {
            mainMusicStartPosition = (float)AudioSettings.dspTime;
        }

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
public partial class AudioManager_s : MonoBehaviour, IAudio //audio system
{
    public float curMainMusicPosition;
    public float mainMusicStartPosition;
    public float musicSpeed;
    public float inputDelay;
    private void Update()
    {
        if (_audioDic.Count != 0)
        {
            curMainMusicPosition= (float)(AudioSettings.dspTime-mainMusicStartPosition)+inputDelay;
        }
    }
}
