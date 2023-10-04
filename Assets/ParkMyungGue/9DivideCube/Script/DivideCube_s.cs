using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
public enum ECubeMode
{
    ROTATE,
    MAINTAIN
}
public enum ERotatePosition
{
    NONE,
    LEFT,
    RIGHT,
    UP,
    DOWN
}
public enum EDivideGameStatus
{
    NONE,
    STARTWAITTING,
    PLAYING,
    END,
    PAUSE
}
public enum EInGameStatus
{
    SHOWPATH,
    PLAYERMOVE
}
public enum ECurCubeFace
{
    ONE,
    TWO,
    THREE,
    FOUR,
    FIVE,
    SIX
}

public enum EEnemyMode
{
    PATH,
    COIN,
    LINEATTACK,
    BLOCK
}
public enum ELineAttackMode
{
    NONE,
    SHOW,
    ATTACK
}
public struct CubeData
{
    public Vector2 position;
    public Vector2 transform;
    public GameObject coin;
    public GameObject fire;
    public GameObject path;
    public GameObject wall;
    public GameObject lineAttack;
    public CubeData(Vector2 _position,Vector2 _transform)
    {
        position = _position;
        transform = _transform;
        coin = null;
        fire = null;
        path = null;
        wall = null;
        lineAttack = null;
    }
    public bool isCanMakeCheck(bool isStack,string dataName)
    {
        if(this.GetType().GetField(dataName).GetValue(this)!=null)
        {
            return false;
        }
        if (isStack)
        {
            if (fire != null)
            {
                return false;
            }
            if (wall != null)
            {
                return false;
            }
        }
        else
        {
            if(coin!=null)
            {
                return false;
            }
            if (fire != null)
            {
                return false;
            }
            if (path != null)
            {
                return false;
            }
            if (wall != null)
            {
                return false;
            }
            if(lineAttack!=null)
            {
                return false;
            }
        }
        return true;
    }
}
public partial class DivideCube_s : MonoBehaviour, IUI  //Display
{
    //main system
    private Dictionary<KeyCode, Action> _otherKeyBinds;
    public Data musicData;
    private bool _isBeatChecked;
    private List<EEnemyMode> _curEnemyMods;
    private Dictionary<EEnemyMode, Action> _enemyModBinds;
    //cube
    private Dictionary<KeyCode, Action> _cubeKeyBinds;
    private CubeData[,] _cubeDatas; //x,y
    //player
    private Dictionary<KeyCode, Action> _playerKeyBinds;
    private Dictionary<KeyCode, Vector2> _playerMoveData;
    private Queue<Vector2> _playerMovePathQueue;
    private List<Vector2> _movePathCheckList;
    private Queue<GameObject> _curMovePathShowObj;
    //time !!!!!TIME : defulat was seconds!!!!!
    private WaitForSeconds _GameWaitWFS;
    public int _speedLoader;
    private float _beatCount;

    [Header("Required Value")]
    //main system
    [SerializeField] private Transform _buttonsTsf;
    [SerializeField] private AudioManager_s _audioManager;
    [SerializeField] private Transform _timeTsf;
    [SerializeField] private Transform _beatCheckTsf;
    [SerializeField] private Transform _playerHPTsf;
    [SerializeField] private float _beatEndScaleX;
    [SerializeField] private float _beatEndScaleY;
    //cube
    [SerializeField] private GameObject _gameCubeObj;
    [SerializeField] private Transform _cubeSizeTsf;
    [SerializeField] private Transform _rotateImageTsf;
    [SerializeField] private ECubeMode _curCubeMode;
    //player
    [SerializeField] private GameObject _playerObj;
    //enemy attack
    [SerializeField] private Transform _movePathTsf;
    [SerializeField] private GameObject _pathSampleObj;
    [SerializeField] private Transform _coinsTsf;
    [SerializeField] private GameObject _coinSampleObj;
    [SerializeField] private Transform _fireTsf;
    [SerializeField] private GameObject _fireSampleObj;
    [SerializeField] private Transform _blockTsf;
    [SerializeField] private GameObject _blockSampleObj;
    [SerializeField] private Transform _lineAttackTsf;
    [SerializeField] private GameObject _lineAttackSampleObj;
    //time
    [SerializeField] private Transform _beatTsf;
    [Header("Only Display")]
    //main system
    [SerializeField] public AudioClip musicClip;
    [SerializeField] public TimeSpan curMainGameTime;
    [SerializeField] public EDivideGameStatus curGameStatus;
    [SerializeField] private bool _IsInput;
    [SerializeField] private int _curPlayerHP;
    [SerializeField] private EInGameStatus _curInGameStatus;
    [SerializeField] private int _beatTimeCount;
    //cube
    [SerializeField] private ECurCubeFace _curSideName;
    [SerializeField] private ERotatePosition _rotateTarget;
    //player
    [SerializeField] private Vector2 _playerPos;
    //enemy attack
    [SerializeField] private ELineAttackMode _curLineAttackMod;

    [Header("Changeable Value")]
    //main system
    [SerializeField] private int _playerMaxHP;
    [SerializeField] private float _beatJudge;
    //cube
    [SerializeField] private int _defaultMovingTime;
    [SerializeField] private int _arrX;
    [SerializeField] private int _arrY;
  //time
  [SerializeField] private float _gameWait;
    [SerializeField] private float _beatTime;
    //test
    [SerializeField] private string _musicName;
}
public partial class DivideCube_s : MonoBehaviour, IUI //main system
{
    private void Awake()
    {
        //main system
        _otherKeyBinds = new Dictionary<KeyCode, Action>();
        _curEnemyMods = new List<EEnemyMode>();
        _enemyModBinds = new Dictionary<EEnemyMode, Action>();
        musicData = new Data();
        //cube
        _cubeKeyBinds = new Dictionary<KeyCode, Action>();
        _cubeDatas = new CubeData[_arrX, _arrY];
        //player
        _playerKeyBinds = new Dictionary<KeyCode, Action>();
        _playerMoveData = new Dictionary<KeyCode, Vector2>();
        _playerMovePathQueue = new Queue<Vector2>();
        _movePathCheckList = new List<Vector2>();
        _curMovePathShowObj = new Queue<GameObject>();
        DefaultDataSetting();
        DefaultValueSetting();
        TimeSetting();
        BindSetting();
        ButtonBind();
        //only Test
        UIOpen();
        MusicSetting(_musicName);
    }
    private void DefaultDataSetting()
    {
        if (_gameCubeObj == null)
        {
            _gameCubeObj = transform.Find("NoneUI").Find("Cube").gameObject;
        }
        if (_buttonsTsf == null)
        {
            _buttonsTsf = transform.Find("UI").Find("Canvas").Find("Buttons");
        }
        if (_audioManager == null)
        {
            _audioManager = transform.Find("AudioManager").GetComponent<AudioManager_s>();
        }
        if (_timeTsf == null)
        {
            _timeTsf = transform.Find("UI").Find("Canvas").Find("Time");
        }
    }
    private void DefaultValueSetting()
    {
        _beatJudge = _beatJudge == 0 ? 0.4f : _beatJudge;
        _defaultMovingTime = _defaultMovingTime == 0 ? 100 : _defaultMovingTime;
        _gameWait = _gameWait == 0 ? 0.05f : _gameWait;
        _speedLoader = _speedLoader == 0 ? 1 : _speedLoader;
        _beatTime = _beatTime == 0 ? 1f : _beatTime;
        _playerMaxHP = _playerMaxHP == 0 ? 100 : _playerMaxHP;
        _arrX = _arrX == 0 ? 4 : _arrX;
        _arrY = _arrY == 0 ? 4 : _arrY;
        float xChange = _cubeSizeTsf.GetComponent<RectTransform>().rect.width / _arrX;
        float yChange = _cubeSizeTsf.GetComponent<RectTransform>().rect.height / _arrY;
        for (int i=0;i<_arrX;i++)
        {
            for(int j=0;j<_arrY;j++)
            {
                _cubeDatas[i, j] = new CubeData(new Vector2(i,j),new Vector2(-1*(xChange+_playerObj.GetComponent<RectTransform>().rect.width) + xChange * i, yChange + _playerObj.GetComponent<RectTransform>().rect.height - yChange * j));
            }
        }
    }
    public void TimeSetting()
    {
        _GameWaitWFS = new WaitForSeconds(_gameWait / _speedLoader);
    }
    private void Update()
    {
        if (curGameStatus == EDivideGameStatus.PLAYING)
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
                if (!_IsInput)
                {
                    if (MathF.Abs(1 - _beatCount) <= _beatJudge)
                    {
                        if (_curCubeMode==ECubeMode.ROTATE&&_rotateTarget!=ERotatePosition.NONE)
                        {
                            foreach (var dic in _cubeKeyBinds)
                            {
                                if (Input.GetKey(dic.Key))
                                {
                                    _cubeKeyBinds[dic.Key]();
                                    _IsInput = true;
                                }
                            }
                        }
                        else if(_curCubeMode==ECubeMode.MAINTAIN&& _curInGameStatus == EInGameStatus.PLAYERMOVE)
                        {
                            foreach (var dic in _playerKeyBinds)
                            {
                                if (Input.GetKey(dic.Key))
                                {
                                    _playerKeyBinds[dic.Key]();
                                    _IsInput = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        HPDown("beat miss");
                        _IsInput = true;
                    }
                }
            }
            _timeTsf.GetComponent<TextMeshProUGUI>().text = curMainGameTime.Minutes.ToString() + curMainGameTime.Seconds.ToString();
        }
        else
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
    }
    public void UIOpen()
    {
        curGameStatus =EDivideGameStatus.STARTWAITTING;
    }
    public void UIPause()
    {
        if (curGameStatus == EDivideGameStatus.PAUSE)
        {
            curGameStatus = EDivideGameStatus.PLAYING;
            _audioManager.AudioUnPause(curMainGameTime);
        }
        else
        {
            if (curGameStatus == EDivideGameStatus.PLAYING)
            {
                curGameStatus = EDivideGameStatus.PAUSE;
                _audioManager.AudioPause();
            }
        }
    }
    public void UIClose()
    {
        curGameStatus = EDivideGameStatus.NONE;
    }
    public void MusicSetting(string name)
    {

        if (curGameStatus == EDivideGameStatus.STARTWAITTING)
        {
            musicData.SetPath((Resources.Load("ParkMyungGue\\XML\\" + name)));
            if (musicData.xdocPath != null)
            {
                musicClip = Resources.Load<AudioClip>("ParkMyungGue\\Music\\" + name);
            }
        }
    }
    private void BindSetting()
    {
        KeyBind(ref _cubeKeyBinds, KeyCode.LeftArrow, () => RotateCube(new Vector3(0, -90, 0)));
        KeyBind(ref _cubeKeyBinds, KeyCode.RightArrow, () => RotateCube(new Vector3(0, 90, 0)));
        KeyBind(ref _cubeKeyBinds, KeyCode.UpArrow, () => RotateCube(new Vector3(-90, 0, 0)));
        KeyBind(ref _cubeKeyBinds, KeyCode.DownArrow, () => RotateCube(new Vector3(90, 0, 0)));

        KeyBind(ref _playerKeyBinds, KeyCode.LeftArrow, () => MovePlayer(Vector2.left));
        _playerMoveData.Add(KeyCode.LeftArrow, Vector2.left);
        KeyBind(ref _playerKeyBinds, KeyCode.RightArrow, () => MovePlayer(Vector2.right));
        _playerMoveData.Add(KeyCode.RightArrow, Vector2.right);
        KeyBind(ref _playerKeyBinds, KeyCode.UpArrow, () => MovePlayer( new Vector2(0,-1)));
        _playerMoveData.Add(KeyCode.UpArrow, new Vector2(0,-1));
        KeyBind(ref _playerKeyBinds, KeyCode.DownArrow, () => MovePlayer( new Vector2(0,1)));
        _playerMoveData.Add(KeyCode.DownArrow, new Vector2(0,1));

        KeyBind(ref _otherKeyBinds, KeyCode.Space, () => UIPause());

        KeyBind(ref _enemyModBinds, EEnemyMode.PATH, PathAction);
        KeyBind(ref _enemyModBinds, EEnemyMode.COIN, CoinAction);
        KeyBind(ref _enemyModBinds, EEnemyMode.BLOCK, BlockAction);
        KeyBind(ref _enemyModBinds, EEnemyMode.LINEATTACK, LineAttackAction);
    }
    private void KeyBind(ref Dictionary<KeyCode, Action> binddic, KeyCode keycode, Action action)
    {
        if (!_otherKeyBinds.ContainsKey(keycode)&&!binddic.ContainsKey(keycode))
        {
            binddic.Add(keycode, action);
        }
    }
    private void KeyBind<T>(ref Dictionary<T, Action> binddic, T mod, Action action)
    {
        if (!binddic.ContainsKey(mod))
        {
            binddic.Add(mod, action);
        }
    }
    private void ButtonBind()
    {
        foreach (Transform child in _buttonsTsf)
        {
            child.GetComponent<Button>().onClick.AddListener((UnityAction)Delegate.CreateDelegate(typeof(UnityAction), this, child.name));
        }
    }
    private void GameStart()
    {
        if (curGameStatus == EDivideGameStatus.STARTWAITTING)
        {
            //main system
            _buttonsTsf.Find("GameStart").gameObject.SetActive(false);
            curMainGameTime = new TimeSpan(0, 0, 0);
            curGameStatus = EDivideGameStatus.PLAYING;
            _curInGameStatus = EInGameStatus.SHOWPATH;
            _curPlayerHP = _playerMaxHP;
            _playerHPTsf.GetComponent<TextMeshProUGUI>().text = _curPlayerHP.ToString();
            _IsInput = false;
            _beatTimeCount = 0;
            _audioManager.AudioPlay(musicClip);
            _isBeatChecked = false;
            //cube
            _gameCubeObj.transform.localEulerAngles = Vector3.zero;
            _curSideName = ECurCubeFace.ONE;
            _curCubeMode = ECubeMode.MAINTAIN;
            //player
            _playerPos = Vector2.zero;
            _playerObj.transform.localPosition = _cubeDatas[(int)_playerPos.x, (int)_playerPos.y].transform;
            //test
            _curEnemyMods.Add(EEnemyMode.COIN);
            _curEnemyMods.Add(EEnemyMode.LINEATTACK);
            StartCoroutine(GamePlaying());
        }
    }
    IEnumerator GamePlaying()
    {
        while (curMainGameTime.TotalSeconds <= musicClip.length && (curGameStatus == EDivideGameStatus.PLAYING || curGameStatus == EDivideGameStatus.PAUSE))
        {
            if (curGameStatus == EDivideGameStatus.PLAYING)
            {
                curMainGameTime = curMainGameTime.Add(new TimeSpan(0, 0, 0, 0, (int)(_gameWait * 1000)));
                _beatCount = (float)((curMainGameTime.TotalSeconds / _beatTime) - (int)(curMainGameTime.TotalSeconds / _beatTime));
                BeatShow();
                if(_beatCount <= 0.1f && _beatCount >= 0f && _isBeatChecked)
                {
                    _isBeatChecked = false;
                }
                if (_beatCount<=0.27f&&_beatCount>=0.23f&&!_isBeatChecked)
                {
                    MoveNextBeat();
                    _isBeatChecked = true;
                }            
            }
            yield return _GameWaitWFS;
        }
    }
    private void MoveNextBeat()
    {
        BeatCheck("");
        switch (_curInGameStatus)
        {
            case EInGameStatus.SHOWPATH:
                if (_curCubeMode==ECubeMode.MAINTAIN)
                {
                    if (PlayerMoveEndCheck())
                    {
                        DoEnemyMode();
                        _beatTimeCount = 2;
                    }
                    _beatTimeCount--;
                    if (_beatTimeCount == 0)
                    {
                        TransparentObjs();
                        _curInGameStatus = EInGameStatus.PLAYERMOVE;
                    }
                }
                break;
            case EInGameStatus.PLAYERMOVE:
                if(_curEnemyMods.Contains(EEnemyMode.LINEATTACK))
                {
                    if(_curLineAttackMod==ELineAttackMode.NONE)
                    {
                        GetRandomeLineAttack();
                    }
                    else if(_curLineAttackMod==ELineAttackMode.SHOW)
                    {
                        _curLineAttackMod = ELineAttackMode.ATTACK;
                        foreach(Transform data in _lineAttackTsf)
                        {
                            data.Find("Attack").gameObject.SetActive(true);
                        }

                    }else if(_curLineAttackMod==ELineAttackMode.ATTACK)
                    {
                        EndRandomeLineAttack();
                    }                   
                }
                if(PlayerMoveEndCheck())
                {
                    DoEnemyMode();
                    GetRandomeRotate();
                    StartCoroutine(RotateMode());
                    _curInGameStatus = EInGameStatus.SHOWPATH;
                }
                break;
            default:
                break;
        }
        if (!_IsInput)
        {
            PlayerPositionCheck();
        }
        _IsInput = false;
    }
    private void DoEnemyMode()
    {
        foreach(var data in _curEnemyMods)
        {
            _enemyModBinds[data]();
        }
    }
    private bool PlayerMoveEndCheck()
    {
        foreach(var data in _curEnemyMods)
        {
            switch(data)
            {
                case EEnemyMode.COIN:
                    if (_coinsTsf.childCount == 0)
                    {
                        return true;
                    }
                    break;
                case EEnemyMode.PATH:
                    if(_movePathTsf.childCount==0)
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }
        }
        return false;
    }
    private void GameEnd()
    {
        _audioManager.AudioStop(musicClip);
        if (curGameStatus == EDivideGameStatus.PLAYING || curGameStatus == EDivideGameStatus.PAUSE)
        {
            curGameStatus = EDivideGameStatus.END;
            curGameStatus = EDivideGameStatus.STARTWAITTING;
            _buttonsTsf.Find("GameStart").gameObject.SetActive(true);
            _playerMovePathQueue.Clear();
            _curMovePathShowObj.Clear();
            UpdatePath();
        }
    }
    private void GameOver()
    {
        GameEnd();
    }
    private void BeatShow()
    {
        _beatTsf.GetComponent<RectTransform>().localScale = Vector2.Lerp(new Vector2(1, 1), new Vector2(_beatEndScaleX, _beatEndScaleY), _beatCount);
        if (_beatCount > 0.8f)
        {
            _beatTsf.GetComponent<Image>().color = new Vector4(0.36f, 0, 1f, 1f);
        }
        else
        {
            _beatTsf.GetComponent<Image>().color = new Vector4(1f, 1f, 1f, 1f);
        }
    }
    private void BeatCheck(string data)
    {
        _beatCheckTsf.GetComponent<TextMeshProUGUI>().text = data;     
    }
    private void HPDown(string message)
    {
        BeatCheck(message);
        _curPlayerHP--;
        _playerHPTsf.GetComponent<TextMeshProUGUI>().text = _curPlayerHP.ToString();
        if (_curPlayerHP == 0)
        {
            GameOver();
        }
    }
    private ERotatePosition GetVec3ToERotatePosition(Vector3 position)
    {
        if(position== new Vector3(0, -90, 0))
        {
            return ERotatePosition.LEFT;
        }
        else if(position == new Vector3(0, 90, 0))
        {
            return ERotatePosition.RIGHT;
        }
        else if (position == new Vector3(-90,0, 0))
        {
            return ERotatePosition.UP;
        }
        else if (position == new Vector3(90,0, 0))
        {
            return ERotatePosition.DOWN;
        }
        return ERotatePosition.NONE;
    }
    private Vector3 GetERotatePositionToVec3(ERotatePosition position)
    {
        switch(position)
        {
            case ERotatePosition.LEFT:
                return new Vector3(0, -90, 0);
            case ERotatePosition.RIGHT:
                return new Vector3(0, 90, 0);
            case ERotatePosition.UP:
                return new Vector3(-90, 0, 0);
            case ERotatePosition.DOWN:
                return new Vector3(90, 0, 0);
            default:
                break;
        }
        return new Vector3(0, 0, 0);
    }
    private ERotatePosition GetVec2ToERotatePosition(Vector2 pos)
    {
        if(pos == new Vector2(-1, -1 * (_arrY - 1)))
        {
            return ERotatePosition.LEFT;
        }
        else if(pos == new Vector2(1,-1 * (_arrY - 1)))
        {
            return ERotatePosition.RIGHT;
        }
        else if(pos == new Vector2(-1 * (_arrX - 1),-1))
        {
            return ERotatePosition.UP;
        }
        else if(pos == new Vector2(-1 * (_arrX - 1),1))
        {
            return ERotatePosition.DOWN;
        }
        return ERotatePosition.NONE;
    }
    private Vector2 GetERotatePositionToVec2(ERotatePosition position)
    {
        switch (position)
        {
            case ERotatePosition.LEFT:
                return new Vector2(-1, -1 * (_arrY - 1));
            case ERotatePosition.RIGHT:
                return new Vector2(1, -1 * (_arrY - 1));
            case ERotatePosition.UP:
                return new Vector2(-1 * (_arrX - 1), -1);
            case ERotatePosition.DOWN:
                return new Vector2(-1 * (_arrX - 1), 1);
            default:
                break;
        }
        return new Vector2(0,0);
    }
}
public partial class DivideCube_s : MonoBehaviour, IUI // cube
{
    IEnumerator RotateMode()
    {
        _rotateImageTsf.Find(_rotateTarget.ToString()).gameObject.SetActive(true);
        _curCubeMode = ECubeMode.ROTATE;
        yield return new WaitForSeconds(_beatTime - 0.02f);
        if(_rotateTarget!=ERotatePosition.NONE)
        {
            HPDown("Don't Rotate");
            RotateCube(GetERotatePositionToVec3(_rotateTarget));
            MovePlayer(new Vector2(_playerPos.x * -1, _playerPos.y * -1));
        }     
    }
    private void RotateCube(Vector3 rotateposition)
    {
            /*switch(_rotateTarget)
            {
                case ERotatePosition.LEFT:
                    MovePlayer(new Vector2(GetERotatePositionToVec2(_rotateTarget).x* GetERotatePositionToVec2(_rotateTarget).y,0));
                    break;
                case ERotatePosition.RIGHT:
                    MovePlayer(new Vector2(GetERotatePositionToVec2(_rotateTarget).x * GetERotatePositionToVec2(_rotateTarget).y, 0));
                    break;
                case ERotatePosition.UP:
                    MovePlayer(new Vector2(0, GetERotatePositionToVec2(_rotateTarget).x * GetERotatePositionToVec2(_rotateTarget).y));
                    break;
                case ERotatePosition.DOWN:
                    MovePlayer(new Vector2(0,GetERotatePositionToVec2(_rotateTarget).x * GetERotatePositionToVec2(_rotateTarget).y));
                    break;
                default:
                    break;
            }*/
            StartCoroutine(RotateTimeLock(rotateposition, _gameCubeObj, _defaultMovingTime));
    }
    IEnumerator RotateTimeLock(Vector3 rotateposition, GameObject targetobj, int rotatetime)
    {
        if (_rotateTarget == GetVec3ToERotatePosition(rotateposition))
        {
            _rotateImageTsf.Find(_rotateTarget.ToString()).gameObject.SetActive(false);
            _rotateTarget = ERotatePosition.NONE;
            Vector3 rotatedivide = rotateposition / rotatetime;
            WaitForSeconds Wait = new WaitForSeconds(0.1f / rotatetime);
            float rotatevalue = 145f / rotatetime;
            for (int i = 0; i < rotatetime; i++)
            {
                if (i < rotatetime / 2)
                {
                    _playerObj.transform.localPosition = new Vector2(_playerObj.transform.localPosition.x, _playerObj.transform.localPosition.y + rotatevalue);
                }
                else
                {
                    _playerObj.transform.localPosition = new Vector2(_playerObj.transform.localPosition.x, _playerObj.transform.localPosition.y - rotatevalue);
                }
                while (curGameStatus == EDivideGameStatus.PAUSE)
                {
                    yield return _GameWaitWFS;
                }
                targetobj.transform.RotateAround(targetobj.transform.position, rotatedivide, Mathf.Abs(rotatedivide.x + rotatedivide.y + rotatedivide.z));
                yield return Wait;
            }
            targetobj.transform.localEulerAngles = new Vector3(MathF.Round(targetobj.transform.localEulerAngles.x), MathF.Round(targetobj.transform.localEulerAngles.y), MathF.Round(targetobj.transform.localEulerAngles.z));
            GetCubeImage();
            _curCubeMode = ECubeMode.MAINTAIN;
        }
    }
    void GetCubeImage()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(1920 / 2, 1080/2, 0));
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        _curSideName =(ECurCubeFace)Enum.Parse(typeof(ECurCubeFace), hit.transform.name);
    }
}
public partial class DivideCube_s : MonoBehaviour, IUI //player
{
    private bool PlayerMoveCheck(Vector2 pos)
    {
        if (_curEnemyMods.Contains(EEnemyMode.PATH))
        {
            if (_playerMovePathQueue.Count != 0)
            {
                if (_playerMovePathQueue.Peek() != _playerPos + pos)
                {
                    HPDown("Move Miss!!");
                    return false;
                }
            }
        }
        if(_curEnemyMods.Contains(EEnemyMode.BLOCK))
        {
            if (_cubeDatas[(int)(_playerPos.x+pos.x),(int)(_playerPos.y+pos.y)].wall!=null)
            {
                return false;
            }
        }
        return true;
    }
    private void PlayerPositionCheck()
    {
        if (_curEnemyMods.Contains(EEnemyMode.COIN))
        {
            if (_cubeDatas[(int)_playerPos.x, (int)_playerPos.y].coin != null)
            {
                RemoveTargetObj("coin", (int)_playerPos.x, (int)_playerPos.y);
            }
            if (_cubeDatas[(int)_playerPos.x, (int)_playerPos.y].fire != null)
            {
                HPDown("Fire!!!");
            }
        }
        if(_curEnemyMods.Contains(EEnemyMode.PATH))
        {
            UpdatePath();
        }
        if(_curEnemyMods.Contains(EEnemyMode.LINEATTACK))
        {
            if (_cubeDatas[(int)_playerPos.x, (int)_playerPos.y].lineAttack != null)
            {
                HPDown("Line Attack!!!");
            }
        }
    }
    private void MovePlayer(Vector2 pos)
    {
        if (PlayerMoveCheck(pos))
        {
            Vector2 previouspos = _playerPos;
            _playerPos += pos;
            _playerPos.x = _playerPos.x < 0 ? 0 : _playerPos.x;
            _playerPos.x = _playerPos.x > _arrX - 1 ? _arrX - 1 : _playerPos.x;
            _playerPos.y = _playerPos.y < 0 ? 0 : _playerPos.y;
            _playerPos.y = _playerPos.y > _arrY - 1 ? _arrY - 1 : _playerPos.y;
            if (_playerPos != previouspos)
            {
                StartCoroutine(MoveTimeLock(_cubeDatas[(int)_playerPos.x, (int)_playerPos.y].transform, _playerObj, _defaultMovingTime));
            }
        }
    }
    IEnumerator MoveTimeLock(Vector2 moveposition, GameObject targetobj, int rotatetime)
    {
        Vector2 movedividepos = new Vector2(
        (moveposition.x - targetobj.transform.localPosition.x) / rotatetime,
        (moveposition.y - targetobj.transform.localPosition.y) / rotatetime
        );
        WaitForSeconds Wait = new WaitForSeconds(0.1f / rotatetime);
        for (int i = 0; i < rotatetime; i++)
        {
            while (curGameStatus == EDivideGameStatus.PAUSE)
            {
                yield return _GameWaitWFS;
            }
            targetobj.transform.localPosition = new Vector2(targetobj.transform.localPosition.x + movedividepos.x, targetobj.transform.localPosition.y + movedividepos.y);
            if (_curEnemyMods.Contains(EEnemyMode.PATH)&&_curMovePathShowObj.Count!=0)
            {
                _movePathTsf.GetComponent<LineRenderer>().SetPosition(0, targetobj.transform.position);
            }
            yield return Wait;
        }
        targetobj.transform.localPosition = moveposition;
        PlayerPositionCheck();
    }
    private void GetRandomeRotate()
    {
        _rotateTarget = (ERotatePosition)Enum.GetValues(typeof(ERotatePosition)).GetValue(UnityEngine.Random.Range(1, Enum.GetValues(typeof(ERotatePosition)).Length));
    }
    private void TransparentObjs()
    {
        if (_curEnemyMods.Contains(EEnemyMode.COIN))
        {
            foreach (Transform data in _coinsTsf)
            {
                data.GetComponent<Image>().color = new Vector4(data.GetComponent<Image>().color.r, data.GetComponent<Image>().color.g, data.GetComponent<Image>().color.b, 0.5f);
            }
        }
        if (_curEnemyMods.Contains(EEnemyMode.PATH))
        {
            foreach (Transform data in _movePathTsf)
            {
                data.GetComponent<Image>().color = new Vector4(data.GetComponent<Image>().color.r, data.GetComponent<Image>().color.g, data.GetComponent<Image>().color.b, 0.5f);
            }
        }


    }
    private void GetRandomeObjs(string dataName, GameObject instTarget, bool isStack, Transform parent, int makeCount)
    {
        for (int i = 0; i < makeCount; i++)
        {
            bool isCreate = false;
            int block = 0;
            while (!isCreate)
            {
                if (block == 50)
                {
                    break;
                }
                int x = UnityEngine.Random.Range(0, _arrX);
                int y = UnityEngine.Random.Range(0, _arrY);
                if (new Vector2(x, y) == _playerPos || !_cubeDatas[x, y].isCanMakeCheck(isStack, dataName))
                {
                    continue;
                }
                else
                {
                    GameObject instObj = Instantiate(instTarget, parent);
                    instObj.transform.localPosition = _cubeDatas[x, y].transform;
                    TypedReference tr = __makeref(_cubeDatas[x, y]);
                    _cubeDatas[x, y].GetType().GetField(dataName).SetValueDirect(tr, instObj);
                    isCreate = true;
                }
                block++;
            }
        }
    }
    private void RemoveTargetObj(string dataName,int xpos, int ypos)
    {
        TypedReference tr = __makeref(_cubeDatas[xpos,ypos]);
        object target = _cubeDatas[xpos, ypos].GetType().GetField(dataName).GetValueDirect(tr);
        if (target != null)
        {
            if (target.GetType().Equals(typeof(GameObject)))
            {
                Destroy((GameObject)target);
                _cubeDatas[xpos, ypos].GetType().GetField(dataName).SetValueDirect(tr, null);
            }
        }
    }
    private void RemoveAllTargetObj(string dataName)
    {
        for(int i=0;i<_arrX;i++)
        {
            for(int j=0;j<_arrY;j++)
            {
                RemoveTargetObj(dataName, i, j);
            }
        }
    }
    private void GetRandomePath()//only test
    {
        Vector2 movedPlayerPos = _playerPos;
        for (int i = 0; i < 10; i++)
        {
            _movePathCheckList.Clear();
            if (movedPlayerPos.x - 1 >= 0)
            {
                _movePathCheckList.Add(new Vector2(movedPlayerPos.x - 1, movedPlayerPos.y));
            }
            if (movedPlayerPos.x + 1 <= _arrX - 1)
            {
                _movePathCheckList.Add(new Vector2(movedPlayerPos.x + 1, movedPlayerPos.y));
            }
            if (movedPlayerPos.y - 1 >= 0)
            {
                _movePathCheckList.Add(new Vector2(movedPlayerPos.x, movedPlayerPos.y - 1));
            }
            if (movedPlayerPos.y + 1 <= _arrY - 1)
            {
                _movePathCheckList.Add(new Vector2(movedPlayerPos.x, movedPlayerPos.y + 1));
            }
            _movePathCheckList.Remove(_playerPos);
            foreach (var data in _playerMovePathQueue)
            {
                _movePathCheckList.Remove(data);
            }
            if (_movePathCheckList.Count != 0)
            {
                Vector2 enqueuedata = _movePathCheckList[UnityEngine.Random.Range(0, _movePathCheckList.Count)];
                _playerMovePathQueue.Enqueue(enqueuedata);
                movedPlayerPos = enqueuedata;
            }
            else
            {
                break;
            }
        }
    }
    private Queue<T> ReverseQueue<T>(Queue<T> queue)
    {
        List<T> prim = new List<T>();
        Queue<T> temp = new Queue<T>();
        foreach(var data in queue)
        {
            prim.Add(data);
        }
        for(int i=prim.Count-1;i>=0;i--)
        {
            temp.Enqueue(prim[i]);
        }
        return temp;
    }
    private void ShowPath()
    {
        if (_playerMovePathQueue.Count != 0)
        {
            int count = 0;
            _movePathTsf.GetComponent<LineRenderer>().positionCount = _playerMovePathQueue.Count>=5?5:_playerMovePathQueue.Count;
            _movePathTsf.GetComponent<LineRenderer>().SetPosition(count, _playerObj.transform.position);
            count++;
            foreach (var data in _playerMovePathQueue)
            {
                if (data.x!= -1 * (_arrX - 1) && data.y!= -1 * (_arrY - 1))
                {
                    GameObject instPath = Instantiate(_pathSampleObj, _movePathTsf);
                    instPath.transform.localPosition = _cubeDatas[(int)data.x, (int)data.y].transform;                   
                    if (count < 5)
                    {
                        instPath.SetActive(true);
                        _movePathTsf.GetComponent<LineRenderer>().SetPosition(count, instPath.transform.position);
                        _curMovePathShowObj.Enqueue(instPath);
                    }
                    else
                    {
                        instPath.SetActive(false);
                    }
                    count++;
                }
            }
        }
    }
    private void UpdatePath()
    {
        if (_playerMovePathQueue.Count==0)
        {
            if (_movePathTsf.childCount != 0)
            {
                for (int i = 0; i < _movePathTsf.childCount; i++)
                {
                    Destroy(_movePathTsf.GetChild(i).gameObject);
                }
            }
            _movePathTsf.GetComponent<LineRenderer>().positionCount = 1;
        }
        else if(_curInGameStatus==EInGameStatus.PLAYERMOVE)
        {
            if (_playerPos == _playerMovePathQueue.Peek())
            {
                _playerMovePathQueue.Dequeue();
                Destroy(_curMovePathShowObj.Dequeue());
            }
            if (_curMovePathShowObj.Count == 1)
            {
                foreach (Transform data in _movePathTsf)
                {
                    if(!data.gameObject.activeSelf)
                    {
                        data.gameObject.SetActive(true);
                        _curMovePathShowObj.Enqueue(data.gameObject);
                        break;
                    }
                }
            }
            int count = 0;
            _movePathTsf.GetComponent<LineRenderer>().positionCount = _curMovePathShowObj.Count+1;
            _movePathTsf.GetComponent<LineRenderer>().SetPosition(count, _playerObj.transform.position);
            foreach (var data in _curMovePathShowObj)
            {
                count++;
                _movePathTsf.GetComponent<LineRenderer>().SetPosition(count, data.transform.position);             
            }
        }
    }
    private void PathAction()
    {
        if (_curInGameStatus == EInGameStatus.SHOWPATH)
        {
            GetRandomePath();
            ShowPath();
        }
        else
        {
            _rotateTarget = ERotatePosition.NONE;
            _playerMovePathQueue.Clear();
            _curMovePathShowObj.Clear();
            UpdatePath();
        }
    }
    private void GetRandomeCoin()
    {
        GetRandomeObjs("coin", _coinSampleObj, true, _coinsTsf, 5);
    }
    private void GetRandomeFire()
    {
        GetRandomeObjs("fire",_fireSampleObj, false, _fireTsf, 5);
    }
    private void EndFire()
    {
        RemoveAllTargetObj("fire");
    }
    private void CoinAction()
    {
        if (_curInGameStatus == EInGameStatus.SHOWPATH)
        {
            GetRandomeCoin();
            GetRandomeFire();
        }
        else
        {
            EndFire();
        }
    }

    private void GetRandomeBlock()
    {
        GetRandomeObjs("wall",_blockSampleObj, false, _blockTsf, 3);
    }
    private void EndBlock()
    {
        RemoveAllTargetObj("wall");
    }
    private void BlockAction()
    {
        if (_curInGameStatus == EInGameStatus.SHOWPATH)
        {
            GetRandomeBlock();
        }
        else
        {
            EndBlock();
        }
    }
    private void GetRandomeLineAttack()
    { 
        int count = UnityEngine.Random.Range(0, 2);
        if (count==0) // row attack
        {
            int y = UnityEngine.Random.Range(0, _arrY);
            for (int i=0;i<_arrX;i++)
            {
                if (_cubeDatas[i, y].isCanMakeCheck(true, "lineAttack"))
                {
                    GameObject instObj = Instantiate(_lineAttackSampleObj, _lineAttackTsf);
                    instObj.transform.Find("Attack").gameObject.SetActive(false);
                    instObj.transform.localPosition = _cubeDatas[i, y].transform;
                    _cubeDatas[i, y].lineAttack = instObj;
                }
            }
        }
        else // columns attack
        {
            int x = UnityEngine.Random.Range(0, _arrX);
            for (int i = 0; i < _arrY; i++)
            {
                if (_cubeDatas[x,i].isCanMakeCheck(true, "lineAttack"))
                {
                    GameObject instObj = Instantiate(_lineAttackSampleObj, _lineAttackTsf);
                    instObj.transform.Find("Attack").gameObject.SetActive(false);
                    instObj.transform.localPosition = _cubeDatas[x, i].transform;
                    _cubeDatas[x, i].lineAttack = instObj;
                }
            }
        }
        _curLineAttackMod = ELineAttackMode.SHOW;
    }
    private void EndRandomeLineAttack()
    {
        RemoveAllTargetObj("lineAttack");
        _curLineAttackMod = ELineAttackMode.NONE;
    }
    private void LineAttackAction()
    {
        if (_curInGameStatus == EInGameStatus.SHOWPATH)
        {
            GetRandomeLineAttack();
        }
        else
        {
            EndRandomeLineAttack();
        }
    }
}

