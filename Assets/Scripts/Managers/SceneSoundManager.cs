using System.Collections.Generic;
using UnityEngine;

// Regist로 오디오 클립에 사용할 소스 저장
// PlaySound로 사운드 종류와 Regist로 저장한 키 값을 전달하여 사운드 재생
public class SceneSoundManager : Singleton<SceneSoundManager>
{
    // 오디오 클립 저장용 Dictionary
    private readonly Dictionary<string, AudioClip> _bgmClips = new Dictionary<string, AudioClip>();
    private readonly Dictionary<string, AudioClip> _seClips = new Dictionary<string, AudioClip>();

    // 용도별 오디오 소스
    [Header("SFX Sound Sources")]
    // 배경음 오디오 소스
    public AudioSource _bgmSource;
    // 박자 오디오 소스
    public AudioSource _bitSeSource;
    // 플레이어 상호작용 오디오 소스
    [SerializeField] private AudioSource _interactionSeSource;
    // UI 상호작용 오디오 소스
    [SerializeField] private AudioSource _interfaceSeSource;

    /*private void Start()
    {
        UpdateValue();
    }*/

    // 오디오 값 갱신
    /*public void UpdateValue()
    {
        SettingsValueManager.ApplyPlayerPrefsValues(_bgmSource, _transitionSeSource, _interactionSeSource, _interfaceSeSource);
    }*/

    // 오디오 재생
    public void PlaySound(ESoundTypes Etypes, string soundKey)
    {
        switch(Etypes)
        {
            case ESoundTypes.BGM:
                if(_bgmClips.ContainsKey(soundKey))
                {
                    _bgmSource.clip = _bgmClips[soundKey];
                    _bgmSource.Play();
                }
                break;
            default:
                Debug.LogError("Check Select AudioSource is Null or " + soundKey + " Has Sound Clips");
                break;
        }
    }
    
    public void StopSound(ESoundTypes Etypes)
    {
        switch(Etypes)
        {
            case ESoundTypes.BGM:
                _bgmSource.Stop();
                break;
            default:
                Debug.LogError("Check Select AudioSource is Null or " + Etypes + " Has Sound Clips");
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
            case ESoundTypes.BGM:
                _bgmSource.mute = false;
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
            case ESoundTypes.BGM:
                _bgmSource.mute = true;
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
