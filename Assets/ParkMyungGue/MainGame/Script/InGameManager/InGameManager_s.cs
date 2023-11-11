using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public partial class InGameManager_s : MonoBehaviour,IScript//Data
{
    [Header("script")]
    private ScriptManager_s _scripts;
    [SerializeField] private SystemUI_s _systemUI_s;

    [Header("main system")]
    private int _score;
    public int combo { get; private set; }
    public EGameStatus curGameStatus { get; private set; }
    public EInGameStatus curInGameStatus { get; private set; }
    public EStage curStage { get; private set; }
    public int beatFreezeCount { get; private set; }
    private bool _isPause;
}
public partial class InGameManager_s //data setting
{
    public void ScriptBind(ScriptManager_s script)
    {
        _scripts = script;
    }
    public void Start()
    {
        curGameStatus = EGameStatus.STARTWAITTING;
    }
}
public partial class InGameManager_s //main system
{
    public void GameStart()
    {
        _scripts._inGamefunBind_s.GameStart();
        //stage data controller
        StageDataController.Instance.isClear = false;
        StageDataController.Instance.score = 0;
        StageDataController.Instance.totalValue = 100;
        StageDataController.Instance.judgementValue = 0;
        StageDataController.Instance.loseValuel = 0;
        StageDataController.Instance.totalScroe = 0;
        StageDataController.Instance.rankScore = 0;
        //main system;
        curInGameStatus = EInGameStatus.NONE;
        curStage = EStage.NONE;
        beatFreezeCount = 0;
        _isPause = false;
        _score = 0;
        combo = 0;
        GamePlay();
    }
    private void GamePlay()
    {
        _scripts._inGamefunBind_s.GamePlay();
        ChangeInGameState(EInGameStatus.SHOWPATH);
        curGameStatus = EGameStatus.PLAYING;//need last
    }
    public void GamePause()
    {
        if (curGameStatus == EGameStatus.PLAYING)
        {
            if (_isPause)
            {
                Time.timeScale = 1f;
                _scripts._inGameMusicManager_s.AudioUnPause();
                _isPause = !_isPause;
            }
            else
            {
                Time.timeScale = 0f;
                _scripts._inGameMusicManager_s.AudioPause();
                _isPause = !_isPause;
            }
        }
    }
    public void GameEnd()
    {
        curGameStatus = EGameStatus.END;
        _scripts._inGamefunBind_s.GameEnd();
        StopAllCoroutines();
        //SceneManager.LoadScene("ResultScene");
    }
    public void GameOverByPlayer()
    {
        StageDataController.Instance.isClear = false;
        StartCoroutine(GameOver());
    }
    public void GameOverByEnemy()
    {
        StageDataController.Instance.isClear = true;
        StartCoroutine(GameOver());
    }
    IEnumerator GameOver()
    {
        curGameStatus = EGameStatus.ENDWAIT;
        yield return new WaitForSeconds(1f);
        GameEnd();
    }
  
}
public partial class InGameManager_s //update
{
    public void MoveNextBeat()
    {
        switch (curInGameStatus)
        {
            case EInGameStatus.SHOWPATH:
                if (beatFreezeCount > 0)
                {
                    beatFreezeCount--;
                }
                if (beatFreezeCount == 0)
                {
                    ChangeInGameState(EInGameStatus.PLAYERMOVE);
                }
                break;
            case EInGameStatus.PLAYERMOVE:
                if(_scripts._inGameEnemy_s.EnemyPhaseEndCheck())
                {
                    ChangeInGameState(EInGameStatus.CUBEROTATE);
                }
                break;
            case EInGameStatus.CUBEROTATE:
                if (beatFreezeCount > 0)
                {
                    beatFreezeCount--;
                }
                if (beatFreezeCount == 0)
                {
                    ChangeInGameState(EInGameStatus.TIMEWAIT);
                }
                break;
            case EInGameStatus.TIMEWAIT:
                if (beatFreezeCount > 0)
                {
                    beatFreezeCount--;
                }
                if (beatFreezeCount == 0)
                {
                    ChangeInGameState(EInGameStatus.SHOWPATH);
                }
                break;
            default:
                break;
        }
        _scripts._inGamefunBind_s.MoveNextBit(curInGameStatus);
    }

    public void ChangeInGameState(EInGameStatus target)
    {
        switch(target)
        {
            case EInGameStatus.SHOWPATH:
                curStage++;
                beatFreezeCount = 2;
                break;
            case EInGameStatus.CUBEROTATE:
                beatFreezeCount = 3;
                break;
            case EInGameStatus.TIMEWAIT:
                beatFreezeCount = 5;
                break;
        }
        curInGameStatus = target;
        _scripts._inGamefunBind_s.ChangeInGameStatus(curInGameStatus);
    }
}
public partial class InGameManager_s //data Change
{
    public void DefaultShow()
    {
        UpdateCombo(combo * -1);
        _systemUI_s.DefaultShow();
    }
    public void GoodScroe()
    {
        UpdateCombo(1);
        UpdateScore(50);
        _systemUI_s.Good();
        StageDataController.Instance.judgementValue++;
    }
    public void PerfectScroe()
    {
        //perfect
        UpdateCombo(1);
        UpdateScore(100);
        _systemUI_s.Perfect();
        StageDataController.Instance.judgementValue += 2;
    }
    public void MissScore()
    {
        _scripts._inGamePlayer_s.UpdatePlayerHP(-1);
        UpdateCombo(combo * -1);
        _systemUI_s.Miss();
    }
    public void UpdateCombo(int changevalue)
    {
        combo += changevalue;
        _systemUI_s.UpdateCombo(combo);
    }
    public void UpdateScore(int changevalue)
    {
        _score += changevalue;
        StageDataController.Instance.score = _score;
        _systemUI_s.UpdateCombo(_score); 
    }
}