using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
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
    private bool _isBeatChecked;
    //cube
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
  //player
  [SerializeField] private GameObject _playerObj;
    [SerializeField] private Transform _movePathTsf;
    [SerializeField] private GameObject _pathSampleObj;
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
    [SerializeField] private int _playerCount;
    //cube
    [SerializeField] private string _curSideName;
    [SerializeField] private ERotatePosition _rotateTarget;
    //player
    [SerializeField] private Vector2 _playerPos;

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
        musicData = new Data();
        //cube
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
                if (!_IsInput&&_curInGameStatus==EInGameStatus.PLAYERMOVE)
                {
                    if (MathF.Abs(1 - _beatCount) <= _beatJudge)
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
                    else
                    {
                        HPDown();
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
            //main system
            _buttonsTsf.Find("GameStart").gameObject.SetActive(false);
            curMainGameTime = new TimeSpan(0, 0, 0);
            curGameStatus = EDivideGameStatus.PLAYING;
            _curInGameStatus = EInGameStatus.SHOWPATH;
            _curPlayerHP = _playerMaxHP;
            _playerHPTsf.GetComponent<TextMeshProUGUI>().text = _curPlayerHP.ToString();
            _IsInput = false;
            _playerCount = 0;
            _audioManager.AudioPlay(musicClip);
            _isBeatChecked = false;
            //cube
            _gameCubeObj.transform.localEulerAngles = Vector3.zero;
            _curSideName = "1";
            //player
            _playerPos = Vector2.zero;
            _playerObj.transform.localPosition = _cubeDatas[(int)_playerPos.x, (int)_playerPos.y].transform;       
            
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
                if (_playerMovePathQueue.Count == 0)
                {
                    GetPath();
                }
                _playerCount--;
                if (_playerCount == 0)
                {
                    _playerCount = _playerMovePathQueue.Count;
                    _curInGameStatus = EInGameStatus.PLAYERMOVE;
                    TransparentPath();
                }
                break;
            case EInGameStatus.PLAYERMOVE:
                break;
            default:
                break;
        }
        _IsInput = false;
    }
    private void GetPath()
    {
        _rotateTarget = ERotatePosition.NONE;
        _playerMovePathQueue.Clear();
        _curMovePathShowObj.Clear();
        GetRandomePath();
        ShowPath();
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
    private void HPDown()
    {
        BeatCheck("miss");
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
    private void RotateCube(Vector3 rotateposition)
    {
            switch(_rotateTarget)
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
            }
            StartCoroutine(RotateTimeLock(rotateposition, _gameCubeObj, _defaultMovingTime));
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
        _curInGameStatus = EInGameStatus.SHOWPATH;
        GetPath();
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
        if (_playerMovePathQueue.Count >= 1)
        {
            if (_playerMovePathQueue.Count>1&&pos != _playerMovePathQueue.Peek() - _playerPos)
            {
                return;
            }
            Vector2 previouspos = _playerPos;
            _playerPos += pos;
            _playerPos.x = _playerPos.x < 0 ? 0 : _playerPos.x;
            _playerPos.x = _playerPos.x > _arrX - 1 ? _arrX - 1 : _playerPos.x;
            _playerPos.y = _playerPos.y < 0 ? 0 : _playerPos.y;
            _playerPos.y = _playerPos.y > _arrY - 1 ? _arrY - 1 : _playerPos.y;
            if (_playerPos != previouspos)
            {
                StartCoroutine(MoveTimeLock(_cubeDatas[(int)_playerPos.x, (int)_playerPos.y].transform, _playerObj, _defaultMovingTime));
                if (_playerMovePathQueue.Count != 0)
                {
                    _playerMovePathQueue.Dequeue();
                }
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
            _movePathTsf.GetComponent<LineRenderer>().SetPosition(0, _playerObj.transform.position);
            yield return Wait;
        }
        targetobj.transform.localPosition = moveposition;
        if (_curInGameStatus == EInGameStatus.PLAYERMOVE)
        {
            UpdatePath();
        }
        if (_playerMovePathQueue.Count == 1)
        {
            RotateCube(GetERotatePositionToVec3(_rotateTarget));
            _rotateImageTsf.Find(_rotateTarget.ToString()).gameObject.SetActive(false);
        }
        if (_playerMovePathQueue.Count == 2)
        {
            _rotateImageTsf.Find(_rotateTarget.ToString()).gameObject.SetActive(true);
        }
    }
    private void GetRandomePath()//only test
    {
        Vector2 movedPlayerPos = _playerPos;
        bool lastCheck = false;
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
        _playerMovePathQueue = ReverseQueue(_playerMovePathQueue);
        while (!lastCheck)
        {
            if ((_playerMovePathQueue.Peek().x == 0 || _playerMovePathQueue.Peek().x == _arrX - 1 || _playerMovePathQueue.Peek().y == 0 || _playerMovePathQueue.Peek().y == _arrY - 1) || _playerMovePathQueue.Count == 0)
            {
                if(_playerMovePathQueue.Count==0)
                {
                    _playerMovePathQueue.Enqueue(_playerPos);
                }
                lastCheck = true;
                break;
            }
            _playerMovePathQueue.Dequeue();
        }
        _movePathCheckList.Clear();
        if(_playerMovePathQueue.Peek().x==0)
        {
            _movePathCheckList.Add(new Vector2(-1, -1*(_arrY-1)));
        }
        if (_playerMovePathQueue.Peek().x == _arrX-1)
        {
            _movePathCheckList.Add(new Vector2(1, -1 * (_arrY - 1)));
        }
        if (_playerMovePathQueue.Peek().y == 0)
        {
            _movePathCheckList.Add(new Vector2(-1 * (_arrX - 1), -1));
        }
        if (_playerMovePathQueue.Peek().y == _arrY-1)
        {
            _movePathCheckList.Add(new Vector2(-1 * (_arrX - 1), 1));
        }
        _playerMovePathQueue = ReverseQueue(_playerMovePathQueue);
        Vector2 rotateTarget=_movePathCheckList[UnityEngine.Random.Range(0, _movePathCheckList.Count)];
        if (_playerMovePathQueue.Peek()==_playerPos)
        {
            _playerMovePathQueue.Dequeue(); 
        }
        _rotateTarget = GetVec2ToERotatePosition(rotateTarget);
        _playerMovePathQueue.Enqueue(rotateTarget);
        _playerCount = 2;
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
            ShowPath();
        }
        else if(_curInGameStatus==EInGameStatus.PLAYERMOVE)
        {
            Destroy(_curMovePathShowObj.Dequeue());
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
    private void TransparentPath()
    {
        foreach(Transform data in _movePathTsf)
        {
            data.GetComponent<Image>().color = new Vector4(1, 0.6f, 0, 0.5f);
        }
    }
}

