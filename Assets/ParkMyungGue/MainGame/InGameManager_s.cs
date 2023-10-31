using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;
public partial class InGameManager_s : MonoBehaviour, IUI  //Data
{
    //only Test
    public Transform lastInputTsf;
    public Transform autoTsf;
    public Transform beatJudgeValue;
    public void BeatMaxChange()
    {
        _inGameData_s.beatJudgeMax = beatJudgeValue.Find("Max").GetComponent<Slider>().value;
        beatJudgeValue.Find("MaxText").GetComponent<TextMeshProUGUI>().text = _inGameData_s.beatJudgeMax.ToString();
    }
    public void BeatMinChange()
    {
        _inGameData_s.beatJudgeMin = beatJudgeValue.Find("Min").GetComponent<Slider>().value;
        beatJudgeValue.Find("MinText").GetComponent<TextMeshProUGUI>().text = _inGameData_s.beatJudgeMin.ToString();
    }
    //
    //key
    private Dictionary<KeyCode, Action> _otherKeyBinds;
    private Dictionary<KeyCode, Func<bool>> _cubeKeyBinds;
    private Dictionary<KeyCode, Func<bool>> _playerKeyBinds;
    private Dictionary<KeyCode, Vector2Int> _playerMoveData;
    public bool IsInput;
    public Queue<KeyCode> InputQueue;
    //main system
    public int beatFreezeCount;
    [SerializeField] private bool _isBeatChecked;
    public EGameStatus curGameStatus;
    public EInGameStatus curInGameStatus;  
    public EInputMode curInputMode;
    public int score;
    public int combo;
    public GameObject lastBeatObj;
    public float lastBeatStartScaleX;
    public bool IsBeatObjCreate;
    public bool IsSecondBeatObjCreate;
    public bool IsGameRestart;
    public bool isFirstInput;
    //auto calibration
    private bool _isAutoCalibrationOn;
    private float _autoCalibrationValue;
    private int _trueCount;
    //pause
    public List<IEnumerator> curCoroutineList;
    //cube
    private Queue<ERotatePosition> _rotateQueue;
    public ECubeFace curFace; 
    private ERotatePosition _rotateTarget;
    //script
    [SerializeField] private InGameData_s _inGameData_s;
    [SerializeField] private InGameEnemy_s _inGameEnemy_s;
    [SerializeField] private InGamePlayer_s _inGamePlayer_s;
    [SerializeField] private InGameCube_s _inGameCube_s;
    [SerializeField] private InGameMusicManager_s _inGameMusicManager_s;
    public void PlayerPostionCheck_InGame(Vector2Int target,bool data)
    {
        _inGameEnemy_s.PlayerPositionCheck(target, data);
    }
    public InGameData_s GetInGameDataScript()
    {
        return _inGameData_s;
    }
    public IReadOnlyList<EEnemyMode> GetEnemyMods()
    {
        return _inGameEnemy_s.curEnemyMods;
    }
    public Queue<Vector2> GetPlayerMovePathQueue()
    {
        return _inGameEnemy_s.playerMovePathQueue;
    }

    public Vector2Int GetPlayerPos()
    {
        return _inGamePlayer_s.playerPos;
    }
    public GameObject GetPlayerObj()
    {
        return _inGamePlayer_s.inGamePlayerObj;
    }

    public float GetMusicPosition()
    {
        return _inGameMusicManager_s.musicPosition;
    }
    public bool GetIsPlayerMoveCheck()
    {
        return _inGamePlayer_s.IsplayerMoveCheck;
    }
    public void PlayerScoreShow(string name)
    {
        foreach(Transform data in _inGameData_s.ScoreShowTsf)
        {
            data.gameObject.SetActive(false);
        }
        if (name != "")
        {
            _inGameData_s.ScoreShowTsf.Find(name).gameObject.SetActive(true);
        }

    }
}
public partial class InGameManager_s //data
{
    private void Awake()
    {
        //Application.targetFrameRate=20;
        _inGameData_s.DefaultDataSetting();
        _inGameMusicManager_s.ScriptBind(this);
        _inGameMusicManager_s.DefaultDataSetting();
        _inGameEnemy_s.ScriptBind(this);
        _inGameEnemy_s.DefaultDataSetting();
        _inGamePlayer_s.ScriptBind(this);
        _inGamePlayer_s.DefaultDataSetting();
        _inGameCube_s.ScriptBind(this);
        _inGameCube_s.DefaultDataSetting();
        DefaultDataSetting();
        //only Test
        GameStartWaittingSetting();
    }
    public void DefaultDataSetting()
    {
        //key
        _otherKeyBinds = new Dictionary<KeyCode, Action>();
        _cubeKeyBinds = new Dictionary<KeyCode, Func<bool>>();
        _playerKeyBinds = new Dictionary<KeyCode, Func<bool>>();
        _playerMoveData = new Dictionary<KeyCode, Vector2Int>();
        IsInput = false;
        InputQueue = new Queue<KeyCode>();
        //main system
        beatFreezeCount = 0;
        _isBeatChecked = false;
        curGameStatus = EGameStatus.NONE;
        curInGameStatus = EInGameStatus.SHOWPATH;
        curInputMode = EInputMode.MAINTAIN;
        score = 0;
        combo = 0;
        lastBeatObj = null;
        lastBeatStartScaleX = 0f;
        IsBeatObjCreate = false;
        IsSecondBeatObjCreate = false;
        IsGameRestart = false;
        isFirstInput = false;
        //auto calibration
        _isAutoCalibrationOn = false;
        _autoCalibrationValue = 0f;
        _trueCount = 0;
        //cube
        _rotateQueue = new Queue<ERotatePosition>();
        _rotateTarget = ERotatePosition.NONE;
        foreach(Transform data in _inGameData_s.rotateImageTsf)
        {
            data.gameObject.SetActive(false);
        }
        GetFaceName();
        BindSetting();
        ButtonBind();
        InGameStart();
        GameEnd();
    }
    private void BindSetting()
    {
        CubeKeyBind(ref _cubeKeyBinds, KeyCode.LeftArrow,new Vector3(0, -90, 0));
        CubeKeyBind(ref _cubeKeyBinds, KeyCode.RightArrow,new Vector3(0, 90, 0));
        CubeKeyBind(ref _cubeKeyBinds, KeyCode.UpArrow,new Vector3(-90, 0, 0));
        CubeKeyBind(ref _cubeKeyBinds, KeyCode.DownArrow, new Vector3(90, 0, 0));

        CubeKeyBind(ref _cubeKeyBinds, KeyCode.A, new Vector3(0, -90, 0));
        CubeKeyBind(ref _cubeKeyBinds, KeyCode.D,new Vector3(0, 90, 0));
        CubeKeyBind(ref _cubeKeyBinds, KeyCode.W,new Vector3(-90, 0, 0));
        CubeKeyBind(ref _cubeKeyBinds, KeyCode.S,new Vector3(90, 0, 0));

        PlayerKeyBind(ref _playerKeyBinds, KeyCode.LeftArrow, Vector2Int.left,_inGameData_s.divideSize);
        PlayerKeyBind(ref _playerKeyBinds, KeyCode.RightArrow, Vector2Int.right, _inGameData_s.divideSize);
        PlayerKeyBind(ref _playerKeyBinds, KeyCode.UpArrow, Vector2Int.down, _inGameData_s.divideSize);
        PlayerKeyBind(ref _playerKeyBinds, KeyCode.DownArrow, Vector2Int.up, _inGameData_s.divideSize);

        PlayerKeyBind(ref _playerKeyBinds, KeyCode.A, Vector2Int.left, _inGameData_s.divideSize);
        PlayerKeyBind(ref _playerKeyBinds, KeyCode.D, Vector2Int.right, _inGameData_s.divideSize);
        PlayerKeyBind(ref _playerKeyBinds, KeyCode.W, Vector2Int.down, _inGameData_s.divideSize);
        PlayerKeyBind(ref _playerKeyBinds, KeyCode.S, Vector2Int.up, _inGameData_s.divideSize);

        KeyBind(ref _otherKeyBinds, KeyCode.Space, () => GamePause());
    }
    private void KeyBind(ref Dictionary<KeyCode, Action> binddic, KeyCode keycode, Action action)
    {
        if (!_otherKeyBinds.ContainsKey(keycode) && !binddic.ContainsKey(keycode))
        {
            binddic.Add(keycode, action);
        }
    }
    private void PlayerKeyBind(ref Dictionary<KeyCode, Func<bool>> binddic, KeyCode keycode, Vector2Int movePos, Vector2Int divideSize)
    {
        if (!_otherKeyBinds.ContainsKey(keycode) && !binddic.ContainsKey(keycode))
        {
            binddic.Add(keycode, () => IsCanMovePlayer(movePos, divideSize));
            _playerMoveData.Add(keycode, movePos);
        }
    }
    private void CubeKeyBind(ref Dictionary<KeyCode, Func<bool>> binddic,KeyCode keycode,Vector3 rotatePosition)
    {
        if (!_otherKeyBinds.ContainsKey(keycode) && !binddic.ContainsKey(keycode))
        {
            binddic.Add(keycode, () => CheckRotateCube(rotatePosition));
        }
    }

    private void ButtonBind()
    {
       /* foreach (Transform child in _inGameData_s.buttonsTsf)
        {
            child.GetComponent<Button>().onClick.AddListener((UnityAction)Delegate.CreateDelegate(typeof(UnityAction), this, child.name));
        }*/
    }
}
public partial class InGameManager_s //data change
{
    public void UpdateCombo(int changevalue)
    {
        combo += changevalue;
        _inGameData_s.comboTsf.GetComponent<TextMeshProUGUI>().text = combo.ToString();
        if (StageDataController.Instance.maxCombo < combo)
        {
            StageDataController.Instance.maxCombo = combo;
        }
    }
    public void UpdateScore(int changevalue)
    {
        score += changevalue;
        _inGameData_s.scoreTsf.GetComponent<TextMeshProUGUI>().text = score.ToString();
        StageDataController.Instance.score = score;
    }
    public void StageChange()
    {
        switch(_inGameData_s.curStage)
        {
            case EStage.STAGE_ONE:
                break;
            case EStage.STAGE_TWO:
                break;
            case EStage.STAGE_THREE:
                break;
            case EStage.STAGE_FOUR:
                break;
            case EStage.STAGE_FIVE:
                break;
            case EStage.STAGE_SIX:
                break;
        }
        _inGameData_s.curStage++;
    }
    public void AttackChange()
    {
        switch(_inGameData_s.curStage)
        {
            case EStage.STAGE_ONE:
                _inGameData_s.playerAttackEffectObj.transform.Find("VFX_MagicSphere_1").gameObject.SetActive(true);
                _inGameData_s.playerAttackEffectObj.transform.Find("VFX_MagicSphere_1").GetComponent<ParticleSystem>().Play();
                break;
            case EStage.STAGE_TWO:
                break;
            case EStage.STAGE_THREE:
                StartCoroutine(AttackIncreaseEffect());
                _inGameData_s.playerAttackEffectObj.transform.Find("VFX_MagicSphere_1").gameObject.SetActive(false);
                _inGameData_s.playerAttackEffectObj.transform.Find("VFX_MagicSphere_2").gameObject.SetActive(true);
                _inGameData_s.playerAttackEffectObj.transform.Find("VFX_MagicSphere_2").GetComponent<ParticleSystem>().Play();
                break;
            case EStage.STAGE_FOUR:
                break;
            case EStage.STAGE_FIVE:
                StartCoroutine(AttackIncreaseEffect());
                _inGameData_s.playerAttackEffectObj.transform.Find("VFX_MagicSphere_2").gameObject.SetActive(false);
                _inGameData_s.playerAttackEffectObj.transform.Find("VFX_MagicSphere_3").gameObject.SetActive(true);
                _inGameData_s.playerAttackEffectObj.transform.Find("VFX_MagicSphere_3").GetComponent<ParticleSystem>().Play();
                break;
            case EStage.STAGE_SIX:
                break;
        }
    }
    IEnumerator AttackIncreaseEffect()
    {
        _inGameData_s.playerAttackEffectObj.transform.Find("VFX_Growing").gameObject.SetActive(true);
        _inGameData_s.playerAttackEffectObj.transform.Find("VFX_Growing").GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(1f);
        _inGameData_s.playerAttackEffectObj.transform.Find("VFX_Growing").gameObject.SetActive(false);
    }
    public void EnemyHPChange(int value)
    {
        _inGameEnemy_s.UpdateEnemyHP(value);
    }
}
public partial class InGameManager_s //main system
{
    public void GameStartWaittingSetting()
    {
        //_inGameData_s.buttonsTsf.Find("GameStart").gameObject.SetActive(true);
        //_inGameData_s.exportCubeBGTsf.Find("Animation").gameObject.SetActive(false);
        //_inGameData_s.exportCubeBGTsf.gameObject.SetActive(true);
        curGameStatus = EGameStatus.STARTWAITTING;
        //GameStart();
    }
    public void GamePause()
    {
        if (curGameStatus == EGameStatus.PAUSE)
        {
            Time.timeScale = 1;
            curGameStatus = EGameStatus.PLAYING;
            _inGameMusicManager_s.AudioUnPause();

        }
        else
        {
            if (curGameStatus == EGameStatus.PLAYING)
            {
                Time.timeScale = 0;
                _inGameMusicManager_s.AudioPause();
                curGameStatus = EGameStatus.PAUSE;
            }
        }
    }
    public void GameClose()
    {
        curGameStatus = EGameStatus.NONE;
    }
    IEnumerator StartAnimation()
    {
        _inGameData_s.exportCubeBGTsf.Find("Animation").gameObject.SetActive(true);
        _inGameData_s.exportCubeBGTsf.Find("Animation").GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "cube_animation", false);
        yield return new WaitForSeconds(5.8f);
        InGameStart();
    }
    public void InGameStart()
    {
        _inGamePlayer_s.GameStart();
        _inGameEnemy_s.GameStart();
        _inGameCube_s.GameStart();
        _inGameEnemy_s.EnemyPhaseChange(EEnemyPhase.Ghost);
        //stage data controller
        StageDataController.Instance.isClear = false;
        StageDataController.Instance.maxCombo = 0;
        StageDataController.Instance.score = 0;
        StageDataController.Instance.perfectCount = 0;
        StageDataController.Instance.goodCount = 0;
        StageDataController.Instance.missCount = 0;
        //key
        isFirstInput = false;
        IsInput = false;
        InputQueue.Clear();
        //main system;
        _inGameData_s.curStage = EStage.STAGE_ONE;
        foreach (Transform target in _inGameData_s.playerAttackEffectObj.transform)
        {
            target.gameObject.SetActive(false);
        }
        AttackChange();
        beatFreezeCount = 0;
        _isBeatChecked = false;
        curInGameStatus = EInGameStatus.SHOWPATH;
        curInputMode = EInputMode.MAINTAIN;
        score = 0;
        combo = 0;
        lastBeatObj = null;
        lastBeatStartScaleX = 0f;
        IsBeatObjCreate = false;
        IsSecondBeatObjCreate = false;
        //auto calibration
        _isAutoCalibrationOn = false;
        _autoCalibrationValue = 0f;
        _trueCount = 0;
        _rotateQueue.Clear();
        _rotateQueue.Enqueue(ERotatePosition.RIGHT); //need fun
        _rotateQueue.Enqueue(ERotatePosition.UP);
        _rotateQueue.Enqueue(ERotatePosition.UP);
        _rotateQueue.Enqueue(ERotatePosition.UP);
        _rotateQueue.Enqueue(ERotatePosition.RIGHT);
        GetFaceName();
        _rotateTarget = ERotatePosition.NONE;
        foreach (Transform data in _inGameData_s.rotateImageTsf)
        {
            data.gameObject.SetActive(false);
        }
        //InGameUI.ShowCharacterAnimation(new List<string>() { "start" }, _inGameData_s.playerImageObj);
        //InGameUI.ShowCharacterAnimation(new List<string>() { "start" }, _inGameData_s.enemyImageObj);
        _inGameData_s.GameStart();
        _inGameMusicManager_s.GameStart();
        curGameStatus = EGameStatus.PLAYING;//need last
    }
    public void GameStart()
    {
        //show start animation
        //StartCoroutine(StartAnimation());
        //
        InGameStart();
    }
    private void GameEnd()
    {
        IsGameRestart = true;
        _inGameMusicManager_s.GameEnd();
        curGameStatus = EGameStatus.END;
        StopAllCoroutines();
        _inGamePlayer_s.GameEnd();
        _inGameEnemy_s.GameEnd();
        _inGameCube_s.GameEnd();
        //SceneManager.LoadScene("ResultScene");
        foreach (Transform target in _inGameData_s.beatTsf)
        {
            Destroy(target.gameObject);
        }
        GameStartWaittingSetting();
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
        yield return new WaitForSeconds(3f);
        GameEnd();
    }
    public IEnumerator BeatShow(GameObject beatObj,float scale)
    {
        float startTime = _inGameMusicManager_s.musicPosition;
        lastBeatObj = beatObj;
        float startData = _inGameMusicManager_s.musicPosition;
        float curData = _inGameMusicManager_s.musicPosition ;
        Vector2 startScale = beatObj.transform.localScale;
        lastBeatStartScaleX = beatObj.transform.localScale.x;
        while (curData - startData <= scale&&curInGameStatus==EInGameStatus.PLAYERMOVE)
        {
            float curPos = curData - startData;
            beatObj.GetComponent<RectTransform>().localScale = Vector2.Lerp(startScale, new Vector2(_inGameData_s.beatEndScaleX, _inGameData_s.beatEndScaleY), curPos*(1/scale));
            yield return null;
            curData = _inGameMusicManager_s.musicPosition;
        }
        //Debug.Log("startTime:" + startTime+" endTime:" + _inGameMusicManager_s.musicPosition);
        if (beatObj != null)
        {
            Destroy(beatObj);
            if(_inGameEnemy_s.curEnemyPhase==EEnemyPhase.Ghost)
            {
                _inGameEnemy_s.GhostAction();
            }
        }
    }
}
public partial class InGameManager_s //update
{
    float lastInputSec;
    private void Update()
    {
        if (curGameStatus == EGameStatus.PLAYING)
        {
            if (curInGameStatus == EInGameStatus.PLAYERMOVE)
            {
                if(_inGameMusicManager_s.loopPositionInBeats>_inGameData_s.beatJudgeMax&&_inGameMusicManager_s.loopPositionInBeats<_inGameData_s.beatJudgeMin)
                {
                    if (!_inGameMusicManager_s.IsMoveNextBit)
                    {
                        MoveNextBeat();
                    }
                    lastInputSec = 0f;
                    if (curInputMode != EInputMode.ROTATE)
                    {
                    }
                }       
                    if (!IsBeatObjCreate && _inGameMusicManager_s.loopPositionInBeats >= 0.5f)
                    {
                        GameObject beatObj = Instantiate(_inGameData_s.beatObj, _inGameData_s.beatTsf);
                        beatObj.GetComponent<RectTransform>().localScale = Vector2.Lerp(beatObj.transform.localScale * 1.5f, new Vector2(_inGameData_s.beatEndScaleX, _inGameData_s.beatEndScaleY),0);
                        StartCoroutine(BeatShow(beatObj,1.5f));
                        IsBeatObjCreate = true;
                    _inGameMusicManager_s.beatCreateCount = 0;
                    }
                
            }
            if (Input.anyKeyDown)
            {
                //lastInputTsf.GetComponent<TextMeshProUGUI>().text = _inGameMusicManager_s.musicPosition + " and loop position : " + _inGameMusicManager_s.loopPositionInBeats + " and completeLoop : " + _inGameMusicManager_s.completedLoops;
                if (curInGameStatus == EInGameStatus.PLAYERMOVE)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        return;
                    }

                    if (curInputMode == EInputMode.ROTATE)
                    {
                        foreach (var dic in _cubeKeyBinds)
                        {
                            if (Input.GetKey(dic.Key))
                            {
                                IsInput = true;
                                if (BeatJudgement())
                                {
                                    if (_cubeKeyBinds[dic.Key]())
                                    {
                                    }
                                }
                                else
                                {
                                    PlayerHPDown(-1, "beat miss");
                                }
                            }
                        }
                    }
                    else if (curInputMode == EInputMode.MAINTAIN)
                    {
                        foreach (var dic in _playerKeyBinds)
                        {
                            if (Input.GetKey(dic.Key))
                            {
                                IsInput = true;
                                if(!isFirstInput)
                                {
                                    isFirstInput = true;
                                    _inGameData_s.doremiTextTsf.gameObject.SetActive(false);
                                    _inGameEnemy_s.EnemyPhaseChange(EEnemyPhase.Phase1);
                                }
                                if (BeatJudgement())
                                {
                                    if (_playerKeyBinds[dic.Key]())
                                    {
                                        lastInputSec = _inGameMusicManager_s.musicPosition;
                                       if(InputQueue.Count==0)
                                        {
                                            InputQueue.Enqueue(dic.Key);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!_inGameMusicManager_s.IsMoveNextBit)
                                    {
                                        MoveNextBeat();
                                    }
                                    PlayerHPDown(-1, "beat miss");
                                }
                            }
                        }
                    }

                }
            }
        }
        if (curGameStatus == EGameStatus.PLAYING || curGameStatus == EGameStatus.PAUSE)
        {
            if (Input.anyKeyDown)
            {
                foreach (var dic in _otherKeyBinds)
                {
                    if (Input.GetKey(dic.Key))
                    {
                        dic.Value();
                    }
                }
            }
        }
        if(InputQueue.Count!=0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                return;
            }
            if (curInputMode == EInputMode.MAINTAIN)
            {
                //Debug.Log("is player Move " + _inGameMusicManager_s.loopPositionInBeats);
                _inGamePlayer_s.MovePlayer(_playerMoveData[InputQueue.Peek()], _inGameData_s.divideSize);
 
                if (!_inGameMusicManager_s.IsMoveNextBit)
                {
                    MoveNextBeat();
                    //Debug.Log("is player move and movenextbit " + _inGameMusicManager_s.loopPositionInBeats);
                }
            }
            InputQueue.Clear();
        }
    }
    public bool BeatJudgement()
    {
   
        if(_inGameMusicManager_s.musicPosition-lastInputSec<=0.4f) //연속으로 입력하는것을 막기 위함
        {
            return false;
        }
        if (_inGameMusicManager_s.loopPositionInBeats <=_inGameData_s.beatJudgeMax||_inGameMusicManager_s.loopPositionInBeats >= _inGameData_s.beatJudgeMin)
        {
            AutoCalibration(true,false);
            BeatScoreCount(_inGameMusicManager_s.loopPositionInBeats);
            return true;
        }
        else
        {
            if (_inGameMusicManager_s.loopPositionInBeats + _autoCalibrationValue <= _inGameData_s.beatJudgeMax || _inGameMusicManager_s.loopPositionInBeats + _autoCalibrationValue >= _inGameData_s.beatJudgeMin)
            {
                AutoCalibration(true,true);
                BeatScoreCount(_inGameMusicManager_s.loopPositionInBeats + _autoCalibrationValue);
                return true;
            }
            else
            {
                AutoCalibration(false, true) ;
                return false;
            }
        }
    }
    public void AutoCalibration(bool check,bool isAuto) 
    {
        float distance = _inGameMusicManager_s.loopPositionInBeats>_inGameData_s.beatJudgeMax+0.1f ? _inGameData_s.beatJudgeMin -_inGameMusicManager_s.loopPositionInBeats: _inGameData_s.beatJudgeMax-_inGameMusicManager_s.loopPositionInBeats ;
   
        if(!check) //miss
        {
            if (MathF.Abs(distance) <= 0.3f)
            {
                if (!_isAutoCalibrationOn)
                {
                    _isAutoCalibrationOn = true;
                    _trueCount = 0;
                    if (distance < 0)
                    {
                        _autoCalibrationValue = distance - 0.15f;
                    }
                    else
                    {
                        _autoCalibrationValue = distance + 0.15f;
                    }
                }
                else
                {
                    _isAutoCalibrationOn = false;
                    _autoCalibrationValue = 0f;
                }
            }
            else
            {
                _isAutoCalibrationOn = false;
                _autoCalibrationValue = 0f;
            }
        }
        else //not miss
        {
            if (isAuto)
            {
                if (_isAutoCalibrationOn)
                {
                    distance = Mathf.Lerp(_autoCalibrationValue, 0f, 1f - (distance / 0.2f));
                    if (MathF.Abs(distance) ==0)
                    {
                        _isAutoCalibrationOn = false;
                        _autoCalibrationValue = 0f;
                    }
                    else
                    {
                        Debug.Log("aa");
                        if (distance < 0)
                        {
                            _autoCalibrationValue = distance - 0.08f;
                        }
                        else
                        {
                            _autoCalibrationValue = distance + 0.08f;
                        }
                    }
                }
             
            }
        }
        //autoTsf.GetComponent<TextMeshProUGUI>().text = "is auto calibration :" + _isAutoCalibrationOn + " value : " + _autoCalibrationValue;
    }
    public void BeatScoreCount(float postionValue) //조정필요
    {
        if (postionValue <=_inGameData_s.beatJudgeMax-0.05f&& postionValue <= _inGameData_s.beatJudgeMin + 0.05f)
        {
            //perfect
            _inGameData_s.beatTsf.GetComponent<Image>().sprite = _inGameData_s.beatPrefectImg;
            UpdateCombo(1);
            UpdateScore(100);
            StageDataController.Instance.perfectCount++;
            PlayerScoreShow("Perfect");
        }
        else
        {
            _inGameData_s.beatTsf.GetComponent<Image>().sprite = _inGameData_s.beatGoodImg;
            UpdateCombo(1);
            UpdateScore(50);
            //good
            StageDataController.Instance.goodCount++;
            PlayerScoreShow("Good");
        }
    }
    public void MoveNextBeat()
    {
        _inGameMusicManager_s.IsMoveNextBit = true;
        InGameUI.ShowText(_inGameData_s,"");
        switch (curInGameStatus)
        {
            case EInGameStatus.SHOWPATH:
                if (beatFreezeCount > 0)
                {
                    beatFreezeCount--;
                }
                else
                {
                    _inGamePlayer_s.MoveNextBit(curInGameStatus);
                    _inGameEnemy_s.MoveNextBit(curInGameStatus);
                    _inGameCube_s.MoveNextBit(curInGameStatus);
                }
                if (beatFreezeCount == 0)
                {
                    ChangeInGameState(EInGameStatus.PLAYERMOVE);
                }
                break;
            case EInGameStatus.PLAYERMOVE:
                _inGamePlayer_s.MoveNextBit(curInGameStatus);
                _inGameCube_s.MoveNextBit(curInGameStatus);
                _inGameEnemy_s.MoveNextBit(curInGameStatus);
                if (_inGameEnemy_s.isEnemyPhaseEnd && _rotateTarget == ERotatePosition.NONE&&isFirstInput)
                {
                    if (_rotateQueue.Count != 0)
                    {
                        _rotateTarget = _rotateQueue.Dequeue();
                    }
                    else
                    {
                        GetRandomeRotate();
                    }
                    StartCoroutine(RotateMode());
                }
                break;
            case EInGameStatus.TIMEWAIT:
                break;
            default:
                break;
        }
        if (!IsInput)
        {
            _inGameData_s.beatTsf.GetComponent<Image>().sprite = _inGameData_s.beatDefaultImg;
            UpdateCombo(combo * -1);
            PlayerScoreShow("");
        }
        IsInput = false;
    }
    IEnumerator WaitTime(float time)
    {
        ChangeInGameState(EInGameStatus.TIMEWAIT);
        foreach (Transform target in _inGameData_s.beatTsf)
        {
            Destroy(target.gameObject);
        }
        _inGamePlayer_s.MovePlayer(new Vector2Int(_inGamePlayer_s.playerPos.x * -1, _inGamePlayer_s.playerPos.y * -1),_inGameData_s.divideSize);
        _isAutoCalibrationOn = false;
        _autoCalibrationValue = 0f;
        yield return new WaitForSeconds(time - 0.2f);
        _inGameData_s.cubeEffectObj.GetComponent<ParticleSystem>().Stop();
        _inGameData_s.cubeEffectObj.transform.Find(curFace.ToString()).gameObject.SetActive(false);
        if (curGameStatus==EGameStatus.PLAYING)
        {
            if(_inGameEnemy_s.curEnemyPhase==EEnemyPhase.Phase1)
            {
                _inGameEnemy_s.EnemyPhaseChange(EEnemyPhase.Phase2);
            }
            else if(_inGameEnemy_s.curEnemyPhase == EEnemyPhase.Phase2)
            {
                _inGameEnemy_s.EnemyPhaseChange(EEnemyPhase.Phase3);
            }
            else if (_inGameEnemy_s.curEnemyPhase == EEnemyPhase.Phase3)
            {
                _inGameEnemy_s.EnemyPhaseChange(EEnemyPhase.Phase2);
            }
            _inGameCube_s.RotateCube(InGameData_s.GetERotatePositionToVec3(_rotateTarget));
            _rotateTarget = ERotatePosition.NONE;
            yield return new WaitForSeconds(0.2f);
            _inGamePlayer_s.UpdatePlayerHP(1);
            ChangeInGameState(EInGameStatus.SHOWPATH);
            AttackChange();
            StageChange();
        }
    }
    public void ChangeInGameState(EInGameStatus target)
    {
        curInGameStatus = target;
        _inGameEnemy_s.ChangeInGameStatus(target);
        _inGameCube_s.ChangeInGameStatus(target);
    }
}
public partial class InGameManager_s //cube
{
    private bool CheckRotateCube(Vector3 rotateposition)
    {
        if (_rotateTarget != InGameData_s.GetVec3ToERotatePosition(rotateposition))
        {
            PlayerHPDown(-1, "wrong input");
            return false;
        }
        return true;
    }

    IEnumerator RotateMode()
    {
        curInputMode = EInputMode.ROTATE;
        IsInput = false;
        StartCoroutine(ShowRotateImage());
        yield return new WaitForSeconds(_inGameMusicManager_s.secPerBeat*3 - 0.02f);
        if (!IsInput)
        {
            PlayerHPDown(-1, "Don't Rotate");
        }
        //InGameUI.ShowCharacterAnimation(new List<string>() { "ready", "attack" }, _inGameData_s.playerImageObj);
        StartCoroutine(_inGamePlayer_s.PlayerAttack());
       // _inGameData_s.playerAttackEffectObj.GetComponent<ParticleSystem>().Play();
        _inGameData_s.cubeEffectObj.transform.Find(curFace.ToString()).gameObject.SetActive(true);
        _inGameData_s.cubeEffectObj.GetComponent<ParticleSystem>().Play();
        StartCoroutine(WaitTime(_inGameData_s.animationTime));
    }
    IEnumerator ShowRotateImage()
    {
        Image targetRotateImage = _inGameData_s.rotateImageTsf.Find(_rotateTarget.ToString()).GetComponent<Image>();
        _inGameData_s.rotateImageTsf.Find(_rotateTarget.ToString()).gameObject.SetActive(true);
        float timeCount = 0;
        WaitForSeconds waitTime = new WaitForSeconds(0.02f);
        while (timeCount <= _inGameMusicManager_s.secPerBeat*4)
        {
            if(curGameStatus==EGameStatus.PAUSE)
            {
                continue;
            }
            if (timeCount < (_inGameMusicManager_s.secPerBeat*4) / 2)
            {
                targetRotateImage.color = new Vector4(targetRotateImage.color.r, targetRotateImage.color.g, targetRotateImage.color.b, Mathf.Lerp(0, 1, (timeCount / _inGameMusicManager_s.secPerBeat * 4) / 2)+0.3f);
            }
            else
            {
                targetRotateImage.color = new Vector4(targetRotateImage.color.r, targetRotateImage.color.g, targetRotateImage.color.b, Mathf.Lerp(1, 0, timeCount / (_inGameMusicManager_s.secPerBeat *4) / 2));
            }
            yield return waitTime;
            timeCount += 0.02f;
        }
        _inGameData_s.rotateImageTsf.Find(_rotateTarget.ToString()).gameObject.SetActive(false);
    }
    public void EndRoateMode()
    {
        curInputMode = EInputMode.MAINTAIN;
        _inGameData_s.enemyHitEffectObj.GetComponent<ParticleSystem>().Stop();
    }
    private void GetRandomeRotate()
    {
        _rotateTarget = (ERotatePosition)Enum.GetValues(typeof(ERotatePosition)).GetValue(UnityEngine.Random.Range(1, Enum.GetValues(typeof(ERotatePosition)).Length));
    }
    public void GetFaceName()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(1920 / 2, 1080 / 2, 0));
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        curFace = (ECubeFace)Enum.Parse(typeof(ECubeFace), hit.transform.name);
    }
}
public partial class InGameManager_s  //player
{
    public void PlayerHPDown(int value,string message)
    {

        if(message=="beat miss")
        {
            _inGameData_s.beatTsf.GetComponent<Image>().sprite = _inGameData_s.beatMissImg;
        }
        _inGamePlayer_s.UpdatePlayerHP(value);
        InGameUI.ShowText(_inGameData_s, message);
        PlayerScoreShow("Miss");
    }
    public bool IsCanMovePlayer(Vector2Int movePos, Vector2Int divideSize)
    {
        return _inGamePlayer_s.PlayerMoveCheck(movePos, divideSize);
    }

    public void DoremiTextChange(string data)
    {
        _inGameData_s.doremiTextTsf.Find("Text").GetComponent<TextMeshProUGUI>().text = data;
    }
    public void InGameTextChange(string data)
    {
        _inGameData_s.inGameTextTsf.Find("Text").GetComponent<TextMeshProUGUI>().text = data;
    }
}
public partial class InGameManager_s //enemy
{
    public void UpdateMovePathLine()
    {
        if (_inGameEnemy_s.curMovePathShowObj.Count != 0)
        {
            _inGameData_s.movePathTsf.GetComponent<LineRenderer>().SetPosition(0, _inGamePlayer_s.inGamePlayerObj.transform.position);
        }
    }
}