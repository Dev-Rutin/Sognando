using FMODPlus;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public partial class InGameManager_s : Singleton<InGameManager_s>//Data
{
    public static Vector2 throwVector2 = new Vector2(10000, 10000);
    [Header("main system")]
    private int _score;
    public int combo { get; private set; }
    public EGameStatus curGameStatus { get; private set; }
    public EInGameStatus curInGameStatus { get; private set; }
    public EStage curStage { get; private set; }
    public int beatFreezeCount { get; private set; }
    public bool isPause { get; private set; }

    [SerializeField] private float _fadeTime;
    [SerializeField] private GameObject _fadeObj;
    [SerializeField] private GameObject _puaseCanvas;
    [SerializeField] private UnityEngine.Animation _reTryAni;
    [SerializeField] private GameObject _pauseCanvasButtons;
    public void Start()
    {
        InGameMusicManager_s.Instance.InGameBind();
        InGameBeatManager_s.Instance.InGameBind();
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
            _score = 0;
            combo = 0;
        SystemUI_s.Instance.UpdateScore(_score);
        SystemUI_s.Instance.UpdateCombo(combo);
        isPause = false;
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
            if (!isPause)
            {
                InGameFunBind_s.Instance.Pause();
                isPause = !isPause;
                _puaseCanvas.SetActive(true);
                _pauseCanvasButtons.SetActive(true);
            }
        }
    }
    public void GameUnPuase()
    {
        StartCoroutine(UnPuaseWait());
    }
    private IEnumerator UnPuaseWait()
    {
        _pauseCanvasButtons.SetActive(false);
        _reTryAni.gameObject.SetActive(true);
        _reTryAni.Play();
        yield return new WaitForSeconds(3.2f);
        _puaseCanvas.SetActive(false);
        _reTryAni.gameObject.SetActive(false);
        InGameFunBind_s.Instance.UnPause();
        isPause = !isPause;
    }
    public void GameEnd()
        {
            curGameStatus = EGameStatus.END;
            InGameFunBind_s.Instance.GameEnd();
            StopAllCoroutines();
        StartCoroutine(FadeStart("ResultScene"));
        }
    public IEnumerator FadeStart(string sceneName)
    {
        FadeUtlity.Instance.CallFade(_fadeTime, _fadeObj, EGameObjectType.UI, EFadeType.FadeOut);
        yield return new WaitForSeconds(_fadeTime);
        SceneManager.LoadScene(sceneName);
    }
    public void GameOverByPlayer()
        {
            StageDataController.Instance.isClear = false;
            StartCoroutine(GameOver(1f));
        }
        public void GameOverByEnemy()
        {
            StageDataController.Instance.isClear = true;
            StartCoroutine(GameOver(7f));
        }
        IEnumerator GameOver(float waitTime)
        {
            curGameStatus = EGameStatus.ENDWAIT;
        if (waitTime !=1f)
        {
            yield return new WaitForSeconds(3f);
            EnemyUI_s.Instance.FadeEnemy(waitTime-3f);
            yield return new WaitForSeconds(waitTime - 3f);
            EnemyUI_s.Instance.UnShowEnemy(false);
        }
        else
        {
            yield return new WaitForSeconds(waitTime);
        }
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
                    beatFreezeCount = 8;
                break;
            default:
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
        if (curInGameStatus == EInGameStatus.PLAYERMOVE)
        {
            UpdateCombo(combo * -1);
        }
    }
    public void GoodScroe()
    {
        UpdateCombo(1);
        UpdateScore(50);
        SystemUI_s.Instance.Good();
        StageDataController.Instance.judgementValue++;
        StageDataController.Instance.goodCount++;
        InGamePlayer_s.Instance.AttackValueIncrease(EBeatJudgement.Good);
        CubeUI_s.Instance.HitEffect(EBeatJudgement.Good);
    }
    public void PerfectScroe()
    {
        //perfect
        UpdateCombo(1);
        UpdateScore(100);
        SystemUI_s.Instance.Perfect();
        StageDataController.Instance.judgementValue += 2;
        StageDataController.Instance.perfectCount++;
        InGamePlayer_s.Instance.AttackValueIncrease(EBeatJudgement.Perfect);
        CubeUI_s.Instance.HitEffect(EBeatJudgement.Perfect);
    }
    public void MissScore()
    {
        InGamePlayer_s.Instance.UpdatePlayerHP(-1);
        UpdateCombo(combo * -1);
        SystemUI_s.Instance.Miss();
        StageDataController.Instance.missCount++;
    }
    public void UpdateCombo(int changevalue)
    {
        combo += changevalue;
        SystemUI_s.Instance.UpdateCombo(combo);
        if(StageDataController.Instance.maxCombo<combo)
        {
            StageDataController.Instance.maxCombo = combo;
        }
    }
    public void UpdateScore(int changevalue)
    {
        _score += changevalue;
        StageDataController.Instance.score = _score;
        SystemUI_s.Instance.UpdateScore(_score);
    }
}
