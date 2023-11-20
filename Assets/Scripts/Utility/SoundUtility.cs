using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using FMODPlus;
using UnityEngine;

public class SoundUtility : Singleton<SoundUtility>
{
    public FMODAudioSource AMBAudioSource;
    public FMODAudioSource BGMAudioSource;
    public FMODAudioSource SFXAudioSource;

    public string[] Buses;

    private Bus _masterBus;
    private Bus _ambBus;
    private Bus _bgmBus;
    private Bus _sfxBus;

    private void Awake()
    {
        _masterBus = RuntimeManager.GetBus(Buses[0]);
        _ambBus = RuntimeManager.GetBus(Buses[1]);
        _bgmBus = RuntimeManager.GetBus(Buses[2]);
        _sfxBus = RuntimeManager.GetBus(Buses[3]);
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(ESoundTypes types)
    {
        switch(types)
        {
            case ESoundTypes.BGM:
                BGMAudioSource.Play();
                break;
            case ESoundTypes.AMB:
                AMBAudioSource.Play();
                break;
            case ESoundTypes.SFX:
                SFXAudioSource.Play();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(types), types, null);
        }
    }
    
    public void StopSound(ESoundTypes types, bool fadeOut = false)
    {
        switch(types)
        {
            case ESoundTypes.BGM:
                BGMAudioSource.AllowFadeout = fadeOut;
                BGMAudioSource.Stop();
                break;
            case ESoundTypes.AMB:
                AMBAudioSource.AllowFadeout = fadeOut;
                AMBAudioSource.Stop();
                break;
            case ESoundTypes.SFX:
                SFXAudioSource.AllowFadeout = fadeOut;
                SFXAudioSource.Stop();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(types), types, null);
        }
    }

    public void SetPause(ESoundTypes types, bool pause)
    {
        if (pause)
        {
            switch (types)
            {
                case ESoundTypes.BGM:
                    BGMAudioSource.Pause();
                    break;
                case ESoundTypes.AMB:
                    AMBAudioSource.Pause();
                    break;
                case ESoundTypes.SFX:
                    SFXAudioSource.Pause();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(types), types, null);
            }
        }
        else
        {
            switch (types)
            {
                case ESoundTypes.BGM:
                    BGMAudioSource.UnPause();
                    break;
                case ESoundTypes.AMB:
                    AMBAudioSource.UnPause();
                    break;
                case ESoundTypes.SFX:
                    SFXAudioSource.UnPause();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(types), types, null);
            }
        }
    }
    
    /// <summary>
    /// Adjust the Master's volume.
    /// </summary>
    /// <param name="value">0~1사이의 값, 0이면 뮤트됩니다.</param>
    public void SetMasterVolume(float value) => _masterBus.setVolume(value);

    /// <summary>
    /// Adjust the volume of the AMB.
    /// </summary>
    /// <param name="value">0~1사이의 값, 0이면 뮤트됩니다.</param>
    public void SetAMBVolume(float value) => _ambBus.setVolume(value);

    /// <summary>
    /// Adjust the volume of the BGM.
    /// </summary>
    /// <param name="value">0~1사이의 값, 0이면 뮤트됩니다.</param>
    public void SetBGMVolume(float value) => _bgmBus.setVolume(value);

    /// <summary>
    /// Adjusts the volume of SFX.
    /// </summary>
    /// <param name="value">0~1사이의 값, 0이면 뮤트됩니다.</param>
    public void SetSFXVolume(float value) => _sfxBus.setVolume(value);
    
    /// <summary>
    /// Create an instance in-place, play a sound effect, and destroy it immediately.
    /// </summary>
    /// <param name="path">재생할 효과음 경로</param>
    /// <param name="position">해당 위치에서 소리를 재생합니다.</param>
    public void PlayOneShot(EventReference path, Vector3 position = default)
    {
        RuntimeManager.PlayOneShot(path, position);
    }
}
