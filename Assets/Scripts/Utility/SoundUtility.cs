using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundUtility : MonoBehaviour
{
    private readonly Dictionary<string, AudioClip> _bgmClips = new Dictionary<string, AudioClip>();
    private readonly Dictionary<string, AudioClip> _seClips = new Dictionary<string, AudioClip>();
    
    // 용도별 오디오 소스
    [Header("SFX Sound Sources")]
    // 배경음 오디오 소스
    [SerializeField] private AudioSource _bgmSource;
    // 박자 재생용 오디오 소스
    [SerializeField] private AudioSource _bitSeSource;
    // 플레이어 상호작용 오디오 소스
    [SerializeField] private AudioSource _interactionSeSource;
    // UI 상호작용 오디오 소스
    [SerializeField] private AudioSource _interfaceSeSource;

    private void Start()
    {
        
    }

    public void PlaySound(ESoundTypes types, string soundKey)
    {
        switch(types)
        {
            case ESoundTypes.Bgm:
                if(_bgmClips.ContainsKey(soundKey))
                {
                    _bgmSource.clip = _bgmClips[soundKey];
                    _bgmSource.Play();
                }
                break;
            case ESoundTypes.Interaction:
                if (_seClips.ContainsKey(soundKey))
                {
                    _interactionSeSource.PlayOneShot(_seClips[soundKey]);
                }
                break;
            case ESoundTypes.Interface:
                if (_seClips.ContainsKey(soundKey))
                {
                    _interfaceSeSource.PlayOneShot(_seClips[soundKey]);
                }
                break;
            case ESoundTypes.Bit:
                if (_seClips.ContainsKey(soundKey) && !_bitSeSource.isPlaying)
                {
                    _bitSeSource.PlayOneShot(_seClips[soundKey]);
                }
                break;
            default:
                Debug.LogError("Check Select AudioSource is Null or " + soundKey + " Has Sound Clips");
                break;
        }
    }
    
    // 오디오 클립 저장
    public void RegistBgm(string sceneBgmName, AudioClip bgm)
    {
        Debug.Assert(bgm != null, "BGM AudioCilp is missing");
        if(_bgmClips.ContainsKey(sceneBgmName))
        {
            Debug.Log(sceneBgmName + "Key Already Registed");
            return;
        }
        _bgmClips.Add(sceneBgmName, bgm);

    }
    public void RegistSe(string sceneSeName, AudioClip se)
    {
        Debug.Assert(se != null, "SE AudioCilp is missing");
        if(_seClips.ContainsKey(sceneSeName))
        {
            Debug.Log(sceneSeName + "Key Already Registed");
            return;
        }
        _seClips.Add(sceneSeName, se);
    }
    // 사운드 활성화, 비활성화
    public void EnableVolume(ESoundTypes type)
    {
        switch (type)
        {
            case ESoundTypes.Bgm:
                _bgmSource.mute = false;
                break;
            case ESoundTypes.Se:
                _bitSeSource.mute = false;
                _interfaceSeSource.mute = false;
                _interactionSeSource.mute = false;
                break;
            default:
                Debug.Assert(false);
                break;
        }
    }
    public void DisableVolume(ESoundTypes type)
    {
        switch (type)
        {
            case ESoundTypes.Bgm:
                _bgmSource.mute = true;
                break;
            case ESoundTypes.Se:
                _bitSeSource.mute = true;
                _interfaceSeSource.mute = true;
                _interactionSeSource.mute = true;
                break;
            default:
                Debug.Assert(false);
                break;
        }
    }

    public void UnPauseSeSound()
    {
        _bitSeSource.UnPause();
        _interfaceSeSource.UnPause();
        _interactionSeSource.UnPause();
    }

    public void PauseSeSound()
    {
        _bitSeSource.Pause();
        _interfaceSeSource.Pause();
        _interactionSeSource.Pause();
    }
}
