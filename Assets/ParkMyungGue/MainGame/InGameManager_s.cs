using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;
    public partial class InGameManager_s : MonoBehaviour, IUI  //Data
{
    //only Test
    public Transform lastInputTsf;
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
}
public partial class InGameManager_s //data
{
    private void Awake()
    {
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
        GameOpen();
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

        PlayerKeyBind(ref _playerKeyBinds, KeyCode.LeftArrow, Vector2Int.left, _inGameData_s.divideSize);
        PlayerKeyBind(ref _playerKeyBinds, KeyCode.RightArrow, Vector2Int.right, _inGameData_s.divideSize);
        PlayerKeyBind(ref _playerKeyBinds, KeyCode.UpArrow, Vector2Int.down, _inGameData_s.divideSize);
        PlayerKeyBind(ref _playerKeyBinds, KeyCode.DownArrow, Vector2Int.up, _inGameData_s.divideSize);

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
        foreach (Transform child in _inGameData_s.buttonsTsf)
        {
            child.GetComponent<Button>().onClick.AddListener((UnityAction)Delegate.CreateDelegate(typeof(UnityAction), this, child.name));
        }
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
}
public partial class InGameManager_s //main system
{
    public void GameOpen()
    {
        curGameStatus = EGameStatus.STARTWAITTING;
    }
    public void GamePause()
    {
        if (curGameStatus == EGameStatus.PAUSE)
        {
            curGameStatus = EGameStatus.PLAYING;
        }
        else
        {
            if (curGameStatus == EGameStatus.PLAYING)
            {
                curGameStatus = EGameStatus.PAUSE;
            }
        }
    }
    public void GameClose()
    {
        curGameStatus = EGameStatus.NONE;
    }
    public void GameStart()
    {
        _inGameData_s.buttonsTsf.Find("GameStart").gameObject.SetActive(false);
        _inGamePlayer_s.GameStart();
        _inGameEnemy_s.GameStart();
        _inGameCube_s.GameStart();
        //stage data controller
        StageDataController.Instance.isClear = false;
        StageDataController.Instance.maxCombo = 0;
        StageDataController.Instance.score = 0;
        StageDataController.Instance.perfectCount = 0;
        StageDataController.Instance.goodCount = 0;
        StageDataController.Instance.missCount = 0;
        //key
        IsInput = false;
        InputQueue.Clear();
        //main system;
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
        InGameUI.ShowCharacterAnimation(new List<string>() { "start" }, _inGameData_s.playerImageObj);
        InGameUI.ShowCharacterAnimation(new List<string>() { "start" }, _inGameData_s.enemyImageObj);
        _inGameData_s.GameStart();
        _inGameMusicManager_s.GameStart();
        curGameStatus = EGameStatus.PLAYING;//need last
    }
    private void GameEnd()
    {
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
        _inGameData_s.buttonsTsf.Find("GameStart").gameObject.SetActive(true);
        curGameStatus = EGameStatus.STARTWAITTING;//need last
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
                    IsInput = false;
                }
                if (lastBeatObj == null)
                {
                    GameObject beatObj = Instantiate(_inGameData_s.beatObj, _inGameData_s.beatTsf);
                    beatObj.GetComponent<RectTransform>().localScale = Vector2.Lerp(beatObj.transform.localScale , new Vector2(_inGameData_s.beatEndScaleX, _inGameData_s.beatEndScaleY),_inGameMusicManager_s.loopPositionInBeats);
                    StartCoroutine(BeatShow(beatObj,1f));
                    IsBeatObjCreate = true;
                }
                else
                {
                    if (!IsBeatObjCreate && _inGameMusicManager_s.loopPositionInBeats >= 0.5f)
                    {
                        GameObject beatObj = Instantiate(_inGameData_s.beatObj, _inGameData_s.beatTsf);
                        beatObj.GetComponent<RectTransform>().localScale = Vector2.Lerp(beatObj.transform.localScale * 1.5f, new Vector2(_inGameData_s.beatEndScaleX, _inGameData_s.beatEndScaleY), _inGameMusicManager_s.loopPositionInBeats - 0.7f);
                        StartCoroutine(BeatShow(beatObj,1.5f));
                        IsBeatObjCreate = true;
                    }
                }
            }
            if (Input.anyKeyDown)
            {
                lastInputTsf.GetComponent<TextMeshProUGUI>().text = _inGameMusicManager_s.musicPosition + " and loop position : " + _inGameMusicManager_s.loopPositionInBeats + " and completeLoop : " + _inGameMusicManager_s.completedLoops;
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
                                        BeatScoreCount();
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
                _inGameEnemy_s.PlayerPositionCheck(_inGamePlayer_s.playerPos, true);
                if (!_inGameMusicManager_s.IsMoveNextBit)
                {
                    MoveNextBeat();
                    //Debug.Log("is player move and movenextbit " + _inGameMusicManager_s.loopPositionInBeats);
                }
                BeatScoreCount();
            }
            InputQueue.Clear();
        }
    }
    public bool BeatJudgement()
    {
        if(_inGameMusicManager_s.musicPosition-lastInputSec<=0.4f)
        {
            return false;
        }
        if (_inGameMusicManager_s.loopPositionInBeats <=_inGameData_s.beatJudgeMax||_inGameMusicManager_s.loopPositionInBeats>= _inGameData_s.beatJudgeMin)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void BeatScoreCount() //조정필요
    {
        /*if (_beatCountValue <= _inGameData_s.beatJudgeMax - 0.1f && _beatCountValue >= _inGameData_s.beatJudgeMin + 0.1f)
        {
            //perfect
            UpdateCombo(1);
            UpdateScore(100);
            StageDataController.Instance.perfectCount++;
        }
        else
        {
            UpdateCombo(1);
            UpdateScore(50);
            //good
            StageDataController.Instance.goodCount++;
        }*/
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
                if (_inGameEnemy_s.isEnemyPhaseEnd && _rotateTarget == ERotatePosition.NONE)
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
            UpdateCombo(combo * -1);
        }
    }
    IEnumerator WaitTime(float time)
    {
        curInGameStatus = EInGameStatus.TIMEWAIT;
        foreach (Transform target in _inGameData_s.beatTsf)
        {
            Destroy(target.gameObject);
        }
        _inGamePlayer_s.MovePlayer(new Vector2Int(_inGamePlayer_s.playerPos.x * -1, _inGamePlayer_s.playerPos.y * -1),_inGameData_s.divideSize);    
        yield return new WaitForSeconds(time - 0.2f);
        ChangeInGameState(EInGameStatus.TIMEWAIT);
        _inGameData_s.cubeEffectObj.transform.Find(curFace.ToString()).gameObject.SetActive(false);
        _inGameEnemy_s.UpdateEnemyHP(-1);
        if (curGameStatus==EGameStatus.PLAYING)
        {
            _inGameCube_s.RotateCube(InGameData_s.GetERotatePositionToVec3(_rotateTarget));
            _rotateTarget = ERotatePosition.NONE;
            yield return new WaitForSeconds(0.2f);
            _inGamePlayer_s.UpdatePlayerHP(1);
            ChangeInGameState(EInGameStatus.SHOWPATH);
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
        _inGameData_s.rotateImageTsf.Find(_rotateTarget.ToString()).gameObject.SetActive(true);
        yield return new WaitForSeconds(_inGameMusicManager_s.secPerBeat - 0.02f);
        if (!IsInput)
        {
            PlayerHPDown(-1, "Don't Rotate");
        }
        _inGameData_s.rotateImageTsf.Find(_rotateTarget.ToString()).gameObject.SetActive(false);
        InGameUI.ShowCharacterAnimation(new List<string>() { "ready", "attack" }, _inGameData_s.playerImageObj);
        _inGameData_s.playerAttackEffectObj.GetComponent<ParticleSystem>().Play();
        _inGameData_s.cubeEffectObj.transform.Find(curFace.ToString()).gameObject.SetActive(true);
        _inGameData_s.cubeEffectObj.GetComponent<ParticleSystem>().Play();
        StartCoroutine(WaitTime(_inGameData_s.animationTime));
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
        _inGamePlayer_s.UpdatePlayerHP(value);
        InGameUI.ShowText(_inGameData_s, message);
    }
    public bool IsCanMovePlayer(Vector2Int movePos, Vector2Int divideSize)
    {
        return _inGamePlayer_s.PlayerMoveCheck(movePos, divideSize);
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