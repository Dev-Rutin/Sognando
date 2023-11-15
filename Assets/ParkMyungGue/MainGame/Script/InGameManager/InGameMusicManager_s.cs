using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class InGameMusicManager_s : Singleton<InGameMusicManager_s>//data 
{
    [Header("scripts")]
    [SerializeField] private AudioClip _musicClip;
    private Dictionary<AudioClip, AudioSource> _audioDic;
    [SerializeField] private int _bpm;
    public float secPerBeat { get; private set; }
    public float musicPosition { get; private set; }
    private float _musicPositionInBeats;
    private float _dspMusicStartPosition;
    public int completedLoops { get; private set; }
    private float _lastBeatCount;
    public float loopPositionInBeats { get; private set; }
    private WaitForEndOfFrame _waitUpdate;
    //pause
    private float _pausePosition;
    private float _pauseStartDspPosition;
    private float _totalPauseValue;
    private void Start()
    {
        _audioDic = new Dictionary<AudioClip, AudioSource>();
        secPerBeat = 60f / _bpm;
        _waitUpdate = new WaitForEndOfFrame();       
    }
}
public partial class InGameMusicManager_s //main system
{
    public void InGameBind()
    {
        InGameFunBind_s.Instance.EgameStart += GameStart;
        InGameFunBind_s.Instance.EgamePlay += GamePlay;
        InGameFunBind_s.Instance.EgameEnd += GameEnd;
    }
    public void GameStart()
    {
        musicPosition = 0;
        _musicPositionInBeats = 0;
        _dspMusicStartPosition = 0;
        completedLoops = 0;
        _lastBeatCount = 0;
        loopPositionInBeats = 0;
        _pausePosition = 0;
        _pauseStartDspPosition = 0;
        _totalPauseValue = 0;
    }
    public void GamePlay()
    {
        AudioPlay(_musicClip); //need last
    }
    public void GameEnd()
    {
        AudioStop(_musicClip);//need first   
    }
}
public partial class InGameMusicManager_s //game system
{
    private void Update()
    {
        if (InGameManager_s.Instance.curGameStatus == EGameStatus.PLAYING)
        {
            musicPosition = (float)(AudioSettings.dspTime - _dspMusicStartPosition - _totalPauseValue);
            _musicPositionInBeats = musicPosition / secPerBeat;
            if (_musicPositionInBeats >= (completedLoops + 1))
            {
                //Debug.Log("music"+_scripts._inGameMusicManager_s.musicPosition);
                completedLoops++;
                InGameBeatManager_s.Instance.NextBit();
            }
            loopPositionInBeats = _musicPositionInBeats - completedLoops;
            if(loopPositionInBeats>InGameBeatManager_s.Instance.beatJudgeMax&&_lastBeatCount<completedLoops)
            {
                InGameManager_s.Instance.MoveNextBeat();
                _lastBeatCount = completedLoops;
            }
        }
    }

}
public partial class InGameMusicManager_s //music manage
{
    public void AudioPlay(AudioClip clip)
    {
        if (!_audioDic.ContainsKey(clip))
        {
            this.AddComponent<AudioSource>();
            _audioDic.Add(clip, this.GetComponents<AudioSource>()[_audioDic.Count]);
            _audioDic[clip].clip = clip;
        }
        if (_audioDic.Count == 1)
        {
            _dspMusicStartPosition = (float)AudioSettings.dspTime;
            _audioDic[clip].Play();
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
            data.Value.Pause();
            _pausePosition = data.Value.time;
            _pauseStartDspPosition = musicPosition;
        }
    }
    public void AudioUnPause()
    {
        foreach (var data in _audioDic)
        {
            data.Value.time = _pausePosition;
            _totalPauseValue += (float)(AudioSettings.dspTime - _dspMusicStartPosition - _totalPauseValue) - _pauseStartDspPosition;
            data.Value.UnPause();
        }
    }
    public void ChangeVolume(float volume) { }
}
