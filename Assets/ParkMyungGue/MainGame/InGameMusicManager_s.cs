using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public interface IAudio
{
    public void AudioPlay(AudioClip clip);
    public void AudioStop(AudioClip clip);
    public void AudioPause();
    public void AudioUnPause(TimeSpan time);
    public void ChangeVolume(float volume);
}
public partial class InGameMusicManager_s : MonoBehaviour, IDataSetting,IScript //data 
{
    //only Test
    public Transform debugLogTsf;
    //
    private InGameManager_s _inGameManager_s;
    private InGameData_s _inGameData_s;
    public Dictionary<AudioClip, AudioSource> _audioDic;
    public int bpm;
    public float secPerBeat;
    public float musicPosition;
    public float musicPositionInBeats;
    public float dspMusicStartPosition;
    public float firstBeatOffset;
    public int completedLoops;
    public float loopPositionInBeats;
    //public float musicSpeed;
    //public float inputDelay;
    public AudioClip mainMusicClip;
    public bool IsMoveNextBit;
    public void ScriptBind(InGameManager_s gameManager)
    {
        _inGameManager_s = gameManager;
        _inGameData_s = gameManager.GetInGameDataScript();
    }
    public void DefaultDataSetting()
    {
        _audioDic = new Dictionary<AudioClip, AudioSource>();
        bpm = _inGameData_s.bpm;
        secPerBeat = 60f / bpm;
        musicPosition = 0;
        musicPositionInBeats = 0;
        dspMusicStartPosition = 0;
        firstBeatOffset = 0;
        completedLoops = 0;
        loopPositionInBeats = 0;
        mainMusicClip = _inGameData_s.musicClip;
        IsMoveNextBit = false;
    }
}
public partial class InGameMusicManager_s //main system
{
    public void GameStart()
    {
        musicPosition = 0;
        musicPositionInBeats = 0;
        dspMusicStartPosition = 0;
        if(!_inGameManager_s.IsGameRestart)
        {
            firstBeatOffset = 0.75f;
        }
        else
        {
            firstBeatOffset = -0.1f;
        }
       
        completedLoops = 0;
        loopPositionInBeats = 0;
        IsMoveNextBit = false;
        AudioPlay(mainMusicClip); //need last
    }
    public void GameEnd()
    {
        AudioStop(mainMusicClip);//need first
    }
}
public partial class InGameMusicManager_s //game system
{
    private void Update()
    {
        if (_inGameManager_s.curGameStatus == EGameStatus.PLAYING)
        {
            musicPosition = (float)(AudioSettings.dspTime - dspMusicStartPosition - firstBeatOffset);
            musicPositionInBeats = musicPosition / secPerBeat;
            if (musicPositionInBeats >= (completedLoops + 1))
            {
                completedLoops++;
                if (_inGameManager_s.GetIsPlayerMoveCheck()||_inGameManager_s.curInGameStatus!=EInGameStatus.PLAYERMOVE)
                {
                    _inGameManager_s.MoveNextBeat();
                    //Debug.Log("just musicmnange next bit " + loopPositionInBeats);
                }
                else
                {
                    IsMoveNextBit = false;
                }
                _inGameManager_s.IsBeatObjCreate = false;
                Debug.Log("move next bit" + "+ msicposition :" + musicPosition+"looppostion : "+ (musicPositionInBeats - completedLoops));
                debugLogTsf.GetComponent<TextMeshProUGUI>().text =  "move next bit" + "+ msicposition :" + musicPosition+" completedloop : " +completedLoops;
            }
            loopPositionInBeats = musicPositionInBeats - completedLoops;
            InGameUI.ShowBGMSlider(_inGameData_s.bgmSlider, musicPosition / mainMusicClip.length);
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
            dspMusicStartPosition = (float)AudioSettings.dspTime;
            Debug.Log(dspMusicStartPosition);
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
            data.Value.Stop();
        }
    }
    public void AudioUnPause(TimeSpan _time)
    {
        foreach (var data in _audioDic)
        {
            if (data.Key.length >= _time.TotalSeconds)
            {
                data.Value.time = (float)_time.TotalSeconds;
            }
            data.Value.Play();
        }
    }
    public void ChangeVolume(float volume) { }


    
}
