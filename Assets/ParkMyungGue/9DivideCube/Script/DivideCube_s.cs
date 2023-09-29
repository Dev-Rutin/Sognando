using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum EDivideGameStatus
{
    NONE,
    STARTWAITTING,
    PLAYING,
    END,
    PAUSE
}
public enum EDivideCubeObjStatus
{
    ROTATION,
    MAINTAIN
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
public struct CubeData
{
    public Vector2 position;
    public Vector2 transform;
    public CubeData(Vector2 _position,Vector2 _transform)
    {
        position = _position;
        transform = _transform;
    }
}
public partial class DivideCube_s : MonoBehaviour, IUI  //Display
{
    //main system
    private Dictionary<KeyCode, Action> _otherKeyBinds;
    public Data musicData;
    private Queue<KeyCode> _playerInputQueue;
    private float _lastInputTime;
    private float _curTimeMsecFloat;
    //cube
    private Dictionary<KeyCode, Action> _cubeKeyBinds;
    private CubeData[,] _cubeDatas;
    //player
    private Dictionary<KeyCode, Action> _playerKeyBinds;
    private Dictionary<KeyCode, Vector2> _playerMoveData;
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
    //cube
    [SerializeField] private GameObject _gameCubeObj;
    //player
    [SerializeField] private GameObject _playerObj;
    //time
    [SerializeField] private Transform _beatTsf;
    [Header("Only Display")]
    //main system
    [SerializeField] public AudioClip musicClip;
    [SerializeField] public TimeSpan curMainGameTime;
    [SerializeField] public EDivideGameStatus curGameStatus;
    [SerializeField] private bool _IsInput;
    //cube
    [SerializeField] private EDivideCubeObjStatus _curCubeObjStatus;
    [SerializeField] private string _curSideName;
    //player
    [SerializeField] private Vector2 _playerPos;

    [Header("Changeable Value")]
    //cube
    [SerializeField] private int _defaultMovingTime;
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
        musicData = new Data();
        _playerInputQueue = new Queue<KeyCode>();
        //cube
        _cubeKeyBinds = new Dictionary<KeyCode, Action>();
        _cubeDatas = new CubeData[3, 3];
        //player
        _playerKeyBinds = new Dictionary<KeyCode, Action>();
        _playerMoveData = new Dictionary<KeyCode, Vector2>();
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
        _defaultMovingTime = _defaultMovingTime == 0 ? 100 : _defaultMovingTime;
        _gameWait = _gameWait == 0 ? 0.05f : _gameWait;
        _speedLoader = _speedLoader == 0 ? 1 : _speedLoader;
        _beatTime = _beatTime == 0 ? 1f : _beatTime;
        for(int i=0;i<3;i++)
        {
            for(int j=0;j<3;j++)
            {
                _cubeDatas[i, j] = new CubeData(new Vector2(i,j),new Vector2(-145+(145*i),145-(145*j)));
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
                    if (MathF.Abs(1 - _beatCount) <= 0.2f)
                    {
                        if (_curCubeObjStatus == EDivideCubeObjStatus.ROTATION)
                        {
                            foreach (var dic in _cubeKeyBinds)
                            {
                                if (Input.GetKey(dic.Key)&&_playerInputQueue.Count<2)
                                {
                                    _playerInputQueue.Enqueue(dic.Key);
                                    _lastInputTime = _curTimeMsecFloat;
                                }
                            }
                        }
                        else if (_curCubeObjStatus == EDivideCubeObjStatus.MAINTAIN)
                        {
                            foreach (var dic in _playerKeyBinds)
                            {
                                if (Input.GetKey(dic.Key)&&_playerInputQueue.Count<2)
                                {
                                    if (_playerInputQueue.Count != 0)
                                    {
                                        if (_playerInputQueue.Peek() != dic.Key)
                                        {
                                            _playerInputQueue.Enqueue(dic.Key);
                                            _lastInputTime = _curTimeMsecFloat;
                                        }
                                    }
                                    else
                                    {
                                        _playerInputQueue.Enqueue(dic.Key);
                                        _lastInputTime = _curTimeMsecFloat;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        BeatCheck("miss");
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
        if(_playerInputQueue.Count!=0 &&Mathf.Abs(_curTimeMsecFloat-_lastInputTime)>=0.18f&&!_IsInput)
        {
            PlayerQueueCheck();
        }
    }
    private void PlayerQueueCheck()
    {
        if (_curCubeObjStatus == EDivideCubeObjStatus.ROTATION)
        {
            _cubeKeyBinds[_playerInputQueue.Dequeue()]();
        }
        else if (_curCubeObjStatus == EDivideCubeObjStatus.MAINTAIN)
        {
           if(_playerInputQueue.Count==2)
            {
                Vector2 data = _playerMoveData[_playerInputQueue.Peek()];
                _playerInputQueue.Dequeue();
                if (data != _playerMoveData[_playerInputQueue.Peek()])
                {
                    data = new Vector2(data.x + _playerMoveData[_playerInputQueue.Peek()].x, data.y + _playerMoveData[_playerInputQueue.Peek()].y);
                    _playerInputQueue.Dequeue();
                }
                MovePlayer(data);
            }
           else
            {
                _playerKeyBinds[_playerInputQueue.Dequeue()]();
            }
        }
        _IsInput = true;
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
    }
    private void KeyBind(ref Dictionary<KeyCode, Action> binddic, KeyCode keycode, Action action)
    {
        if (!_otherKeyBinds.ContainsKey(keycode)&&!binddic.ContainsKey(keycode))
        {
            binddic.Add(keycode, action);
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
            _gameCubeObj.transform.localEulerAngles = Vector3.zero;
            _curSideName = "1";
            _curCubeObjStatus = EDivideCubeObjStatus.MAINTAIN;
            _playerPos = Vector2.zero;
            MovePlayer(new Vector2(1,1));
            curMainGameTime = new TimeSpan(0, 0, 0);
            curGameStatus = EDivideGameStatus.PLAYING;
            _buttonsTsf.Find("GameStart").gameObject.SetActive(false);
            _audioManager.AudioPlay(musicClip);
            _IsInput = false;
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
                _curTimeMsecFloat= (float)(curMainGameTime.TotalSeconds - (int)curMainGameTime.TotalSeconds);
                BeatShow();
                if (curMainGameTime.Milliseconds==250)
                {
                    _IsInput = false;
                    BeatCheck("");
                    _playerInputQueue.Clear();
                    if (curMainGameTime.Seconds % 5f == 0 && curMainGameTime.Seconds > 1 && _curCubeObjStatus == EDivideCubeObjStatus.MAINTAIN) //only test
                    {
                        StartCoroutine(RotateMode());
                    }
                }            
            }
            yield return _GameWaitWFS;
        }
    }
    private void GameEnd()
    {
        _audioManager.AudioStop(musicClip);
        if (curGameStatus == EDivideGameStatus.PLAYING || curGameStatus == EDivideGameStatus.PAUSE)
        {
            curGameStatus = EDivideGameStatus.END;
            curGameStatus = EDivideGameStatus.STARTWAITTING;
            _buttonsTsf.Find("GameStart").gameObject.SetActive(true);
        }
    }
    private void BeatShow()
    {

        _beatTsf.Find("Left").transform.localPosition = new Vector3(-900 + (950 * _beatCount), 0, 0);
        _beatTsf.Find("Right").transform.localPosition = new Vector3(900 - (950 * _beatCount), 0, 0);
        _beatTsf.GetComponent<Image>().color = new Vector4(1, 0, 0, _beatCount);
    }
    private void BeatCheck(string data)
    {
        _beatCheckTsf.GetComponent<TextMeshProUGUI>().text = data;     
    }
}
public partial class DivideCube_s : MonoBehaviour, IUI // cube
{
    IEnumerator RotateMode()
    {
        _curCubeObjStatus = EDivideCubeObjStatus.ROTATION;
        yield return new WaitForSeconds(_beatTime);
        _curCubeObjStatus = EDivideCubeObjStatus.MAINTAIN;
    }
    private void RotateCube(Vector3 rotateposition)
    {
        StartCoroutine(RotateTimeLock(rotateposition,_gameCubeObj ,_defaultMovingTime));
    }
    IEnumerator RotateTimeLock(Vector3 rotateposition, GameObject targetobj, int rotatetime)
    {
        Vector3 rotatedivide = rotateposition / rotatetime;
        WaitForSeconds Wait = new WaitForSeconds(0.1f / rotatetime);
        float rotatevalue = 145f / rotatetime;
        for (int i = 0; i < rotatetime; i++)
        {
            if (i < rotatetime / 2)
            {
                _playerObj.transform.localPosition =new Vector2(_playerObj.transform.localPosition.x, _playerObj.transform.localPosition.y+rotatevalue);
            }
            else
            {
                _playerObj.transform.localPosition = new Vector2(_playerObj.transform.localPosition.x, _playerObj.transform.localPosition.y - rotatevalue);
            }
            while (curGameStatus == EDivideGameStatus.PAUSE)
            {
                yield return _GameWaitWFS;
            }
            targetobj.transform.RotateAround(targetobj.transform.position, rotatedivide,Mathf.Abs(rotatedivide.x+rotatedivide.y+rotatedivide.z));
            yield return Wait;
        }
        targetobj.transform.localEulerAngles = new Vector3(MathF.Round(targetobj.transform.localEulerAngles.x), MathF.Round(targetobj.transform.localEulerAngles.y), MathF.Round(targetobj.transform.localEulerAngles.z));
        GetCubeImage();
        _playerObj.transform.localPosition = _cubeDatas[(int)_playerPos.x, (int)_playerPos.y].transform;
    }
    void GetCubeImage()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(1920 / 2, 1080/2, 0));
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        _curSideName = hit.transform.name;
    }
}
public partial class DivideCube_s : MonoBehaviour, IUI //player
{
    private void MovePlayer(Vector2 pos)
    {
        Vector2 previouspos = _playerPos;
        _playerPos += pos;
        _playerPos.x = _playerPos.x<0? 0: _playerPos.x;
        _playerPos.x = _playerPos.x > 2 ? 2 : _playerPos.x;
        _playerPos.y = _playerPos.y < 0 ? 0 : _playerPos.y;
        _playerPos.y = _playerPos.y > 2 ? 2 : _playerPos.y;
        if (_playerPos != previouspos)
        {
            StartCoroutine(MoveTimeLock(_cubeDatas[(int)_playerPos.x, (int)_playerPos.y].transform, _playerObj, _defaultMovingTime));
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
            yield return Wait;
        }
        targetobj.transform.localPosition = moveposition;
    }
}

