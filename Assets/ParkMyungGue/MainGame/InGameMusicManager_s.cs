using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
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
    public float lastNextBeatLoopPosition;
    //pause
    public float pausePosition;
    public float pauseStartDspPosition;
    public float totalPauseValue;
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
        lastNextBeatLoopPosition = 0f; //비트가 정상적으로 넘어갔을 경우의 현재 비트의 위치
      pausePosition=0;
  pauseStartDspPosition=0;
   totalPauseValue=0;
}
}
public partial class InGameMusicManager_s //main system
{
    public void GameStart()
    {
        musicPosition = 0;
        musicPositionInBeats = 0;
        dspMusicStartPosition = 0;
        if (!_inGameManager_s.IsGameRestart)
        {
            firstBeatOffset = 0.25f;
        }
        else
        {
            firstBeatOffset = 0f;
        }
       
        completedLoops = 0;
        loopPositionInBeats = 0;
        IsMoveNextBit = false;
        lastNextBeatLoopPosition = 0f;
        pausePosition = 0;
        pauseStartDspPosition = 0;
        totalPauseValue = 0;
        beatCreateCount = 0;
        AudioPlay(mainMusicClip); //need last
    }
    public void GameEnd()
    {
        AudioStop(mainMusicClip);//need first
    }
}
public partial class InGameMusicManager_s //game system
{
    public int beatCreateCount;
    private void Update()
    {
        if (_inGameManager_s.curGameStatus == EGameStatus.PLAYING)
        {
            musicPosition = (float)(AudioSettings.dspTime - dspMusicStartPosition - firstBeatOffset-totalPauseValue);
            musicPositionInBeats = musicPosition / secPerBeat;
            if (musicPositionInBeats >= (completedLoops + 1))
            {
                completedLoops++;
                if (!_inGameManager_s.GetIsPlayerMoveCheck()||_inGameManager_s.curInGameStatus!=EInGameStatus.PLAYERMOVE)
                {
                    _inGameManager_s.MoveNextBeat();
                    Debug.Log("just musicmnange next bit " + loopPositionInBeats);
                }
                else
                {
                    IsMoveNextBit = false;
                }
                 _inGameManager_s.IsBeatObjCreate = false;
                /*if (beatCreateCount == 1)
                {
                    _inGameManager_s.IsBeatObjCreate = false;
                }
                else
                {
                    beatCreateCount++;
                }*/
                lastNextBeatLoopPosition = loopPositionInBeats;
                //Debug.Log("move next bit" + "+ msicposition :" + musicPosition+"looppostion : "+ (musicPositionInBeats - completedLoops));
                //debugLogTsf.GetComponent<TextMeshProUGUI>().text =  "move next bit" + "+ msicposition :" + musicPosition+" completedloop : " +completedLoops;
            }
            loopPositionInBeats = musicPositionInBeats - completedLoops;
            /*float value = UnityEngine.Random.Range(0, 0.1f);
            if(loopPositionInBeats >=0.65f+value && loopPositionInBeats <= 0.67f+value&& _inGameManager_s.InputQueue.Count==0)
            {
                if (_inGameManager_s.BeatJudgement())
                {
                    _inGameManager_s.InputQueue.Enqueue(KeyCode.S);
                    _inGameManager_s.lastInputTsf.GetComponent<TextMeshProUGUI>().text = musicPosition + " and loop position : " + loopPositionInBeats + " and completeLoop : " + completedLoops;
                }
            }*/
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
            data.Value.Pause();
            pausePosition = data.Value.time;
            pauseStartDspPosition = musicPosition;
        }
    }
    public void AudioUnPause()
    {
        foreach (var data in _audioDic)
        {
            data.Value.time = pausePosition;
            totalPauseValue += (float)(AudioSettings.dspTime - dspMusicStartPosition - firstBeatOffset - totalPauseValue) - pauseStartDspPosition;
            data.Value.UnPause();
        }
    }
    public void ChangeVolume(float volume) { }


    
}
