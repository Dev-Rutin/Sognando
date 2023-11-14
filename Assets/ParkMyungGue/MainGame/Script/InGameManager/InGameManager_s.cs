using System.Collections;
using UnityEngine;
public partial class InGameManager_s : Singleton<InGameManager_s>//Data
{
    [Header("main system")]
    private int _score;
    public int combo { get; private set; }
    public EGameStatus curGameStatus { get; private set; }
    public EInGameStatus curInGameStatus { get; private set; }
    public EStage curStage { get; private set; }
    public int beatFreezeCount { get; private set; }
    private bool _isPause;
    public void Start()
    {
        InGameMusicManager_s.Instance.InGameBind();
        InGameBeatManager_s.Instance.InGameBind();
        InGameSideData_s.Instance.InGameBind();
        KeyInputManager_s.Instance.InGameBind();
        InGamePlayer_s.Instance.InGameBind();
        InGameEnemy_s.Instance.InGameBind();
        InGameCube_s.Instance.InGameBind();
        curGameStatus = EGameStatus.STARTWAITTING;
    }
}
    public partial class InGameManager_s //main system
    {
        public void GameStart()
        {
            InGameFunBind_s.Instance.GameStart();
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
            InGameFunBind_s.Instance.GamePlay();
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
                    InGameMusicManager_s.Instance.AudioUnPause();
                    _isPause = !_isPause;
                }
                else
                {
                    Time.timeScale = 0f;
                    InGameMusicManager_s.Instance.AudioPause();
                    _isPause = !_isPause;
                }
            }
        }
        public void GameEnd()
        {
            curGameStatus = EGameStatus.END;
            InGameFunBind_s.Instance.GameEnd();
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
                if (InGameEnemy_s.Instance.EnemyPhaseEndCheck())
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
        InGameFunBind_s.Instance.MoveNextBit(curInGameStatus);
    }

    public void ChangeInGameState(EInGameStatus target)
    {
        switch (target)
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
        InGameFunBind_s.Instance.ChangeInGameStatus(curInGameStatus);
    }
}
public partial class InGameManager_s //data Change
{
    public void DefaultShow()
    {
        UpdateCombo(combo * -1);
        SystemUI_s.Instance.DefaultShow();
    }
    public void GoodScroe()
    {
        UpdateCombo(1);
        UpdateScore(50);
        SystemUI_s.Instance.Good();
        StageDataController.Instance.judgementValue++;
    }
    public void PerfectScroe()
    {
        //perfect
        UpdateCombo(1);
        UpdateScore(100);
        SystemUI_s.Instance.Perfect();
        StageDataController.Instance.judgementValue += 2;
    }
    public void MissScore()
    {
        InGamePlayer_s.Instance.UpdatePlayerHP(-1);
        UpdateCombo(combo * -1);
        SystemUI_s.Instance.Miss();
    }
    public void UpdateCombo(int changevalue)
    {
        combo += changevalue;
        SystemUI_s.Instance.UpdateCombo(combo);
    }
    public void UpdateScore(int changevalue)
    {
        _score += changevalue;
        StageDataController.Instance.score = _score;
        SystemUI_s.Instance.UpdateCombo(_score);
    }
}
