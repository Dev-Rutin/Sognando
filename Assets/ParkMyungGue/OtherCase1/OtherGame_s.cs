using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
enum ECubeImage
{
    ATTACK,
    DEFENSE,
    EVASION,
    PARRY,
    EXECUTE
}
public enum EOtherGameStatus
{
   NONE,
   PLAYERTURN,
   ENEMYTURN
}
public partial class OtherGame_s : MonoBehaviour //Display
{
    //main system
    private Dictionary<KeyCode, Action> _otherKeyBinds;
    public Data musicData;
    //cube
    private Dictionary<KeyCode, Action> _cubeKeyBinds;
    private Queue<KeyCode> _keyInputQueue;
    private List<KeyCode> _cubeKeyPressCheck;
    private Queue<IEnumerator> _rotationQueue;
    private bool _isSetting;
    private Vector3 _previousRoatePos;
    private IEnumerator _previousRotation;
    private int _rotateCount;
    private Dictionary<GameObject, ECubeImage> _playerStacksDic;
    //time !!!!!TIME : defulat was seconds!!!!!
    private WaitForSeconds _GameWaitWFS;
    public int _speedLoader;
    private float _beatCount;
    
    [Header("Required Value")]
    //main system
    [SerializeField] private Transform _buttonsTsf;
    [SerializeField] private AudioManager_s _audioManager;
    [SerializeField] private Transform _timeTsf;
    //cube
    [SerializeField] private GameObject _gameCube;
    [SerializeField] private Transform _cubeStackTSF;
    [SerializeField] private GameObject _cubeStackSample;
    //time
    [SerializeField] private Transform _beatTsf;
    [Header("Only Display")]
    //main system
    [SerializeField] public AudioClip musicClip;
    [SerializeField] public TimeSpan curMainGameTime;
    [SerializeField] public EGameStatus curGameStatus;
    [SerializeField] private EOtherGameStatus _curOtherGameStatus;
    //cube
    [SerializeField] private ECubeObjStatus _curCubeObjStatus;
    [SerializeField] private ECubeImage _curCubeImageStatus;

    [Header("Changeable Value")]
    //cube
    [SerializeField] private int _defaultRotateTime;
    //time
    [SerializeField] private float _gameWait;
    [SerializeField] private float _beatTime;
    //test
    [SerializeField] private string _musicName;
}
public partial class OtherGame_s : MonoBehaviour//main system
{
    private void Awake()
    {
        //main system
        _otherKeyBinds = new Dictionary<KeyCode, Action>();
        musicData = new Data();
        //cube
        _cubeKeyBinds = new Dictionary<KeyCode, Action>();
        _keyInputQueue = new Queue<KeyCode>();
        _cubeKeyPressCheck = new List<KeyCode>();
        _rotationQueue = new Queue<IEnumerator>();
        _playerStacksDic = new Dictionary<GameObject, ECubeImage>();
        //time
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
        if (_gameCube == null)
        {
            _gameCube = transform.Find("NoneUI").Find("Cube").gameObject;
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
        _defaultRotateTime = _defaultRotateTime == 0 ? 100 : _defaultRotateTime;
        _gameWait = _gameWait == 0 ? 0.05f : _gameWait;
        _speedLoader = _speedLoader == 0 ? 1 : _speedLoader;
        _beatTime = _beatTime == 0 ? 1f : _beatTime;
    }
    public void TimeSetting()
    {
        _GameWaitWFS = new WaitForSeconds(_gameWait / _speedLoader);
    }
    private void Update()
    {
        if (curGameStatus == EGameStatus.PLAYING)
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
                foreach (var dic in _cubeKeyBinds)
                {
                    if (Input.GetKey(dic.Key) && !_cubeKeyPressCheck.Contains(dic.Key))
                    {
                        if (MathF.Abs(1 - _beatCount) <= 0.2f)
                        {
                            _keyInputQueue.Enqueue(dic.Key);
                            _cubeKeyPressCheck.Add(dic.Key);
                        }
                        else
                        {
                        }
                    }
                }
            }
            if (_keyInputQueue.Count != 0 && !_isSetting)
            {
                _cubeKeyBinds[_keyInputQueue.Dequeue()]();
            }
            if (_rotationQueue.Count != 0 && _curCubeObjStatus != ECubeObjStatus.ROTATION)
            {
                _previousRotation = _rotationQueue.Peek();
                StartCoroutine(_rotationQueue.Dequeue());
            }
            PressCheck();
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
        _curCubeImageStatus = ECubeImage.ATTACK;
        _curCubeObjStatus = ECubeObjStatus.STOP;
        _isSetting = false;
        curGameStatus = EGameStatus.STARTWAITTING;
        _curOtherGameStatus = EOtherGameStatus.NONE;
    }
    public void UIPause()
    {
        if (curGameStatus == EGameStatus.PAUSE)
        {
            curGameStatus = EGameStatus.PLAYING;
            _audioManager.AudioUnPause(curMainGameTime);
        }
        else
        {
            if (curGameStatus == EGameStatus.PLAYING)
            {
                curGameStatus = EGameStatus.PAUSE;
                _audioManager.AudioPause();
            }
        }
    }
    public void UIClose()
    {
        curGameStatus = EGameStatus.NONE;
    }
    public void MusicSetting(string name)
    {

        if (curGameStatus == EGameStatus.STARTWAITTING)
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
        KeyBind(ref _cubeKeyBinds, KeyCode.LeftArrow, () => RotateCube(new Vector3(0, -90, 0), ECubeImage.EVASION));
        KeyBind(ref _cubeKeyBinds, KeyCode.RightArrow, () => RotateCube(new Vector3(0, 90, 0), ECubeImage.PARRY));
        KeyBind(ref _cubeKeyBinds, KeyCode.UpArrow, () => RotateCube(new Vector3(-90, 0, 0), ECubeImage.DEFENSE));
        KeyBind(ref _cubeKeyBinds, KeyCode.DownArrow, () => RotateCube(new Vector3(90, 0, 0), ECubeImage.EXECUTE));
        KeyBind(ref _otherKeyBinds, KeyCode.Space, () => UIPause());
    }
    private void KeyBind(ref Dictionary<KeyCode, Action> binddic, KeyCode keycode, Action action)
    {
        if (!_otherKeyBinds.ContainsKey(keycode) && !_cubeKeyBinds.ContainsKey(keycode))
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
        if (curGameStatus == EGameStatus.STARTWAITTING)
        {
            curMainGameTime = new TimeSpan(0, 0, 0);
            curGameStatus = EGameStatus.PLAYING;
            _buttonsTsf.Find("GameStart").gameObject.SetActive(false);
            _audioManager.AudioPlay(musicClip);
            StartCoroutine(GamePlaying());
        }
    }
    IEnumerator GamePlaying()
    {
        while (curMainGameTime.TotalSeconds <= musicClip.length && (curGameStatus == EGameStatus.PLAYING || curGameStatus == EGameStatus.PAUSE))
        {
            if (curGameStatus == EGameStatus.PLAYING)
            {
                curMainGameTime = curMainGameTime.Add(new TimeSpan(0, 0, 0, 0, (int)(_gameWait * 1000)));
                _beatCount = (float)((curMainGameTime.TotalSeconds / _beatTime) - (int)(curMainGameTime.TotalSeconds / _beatTime));
                BeatShow();
            }
            yield return _GameWaitWFS;
        }
    }
    private void GameEnd()
    {
        _audioManager.AudioStop(musicClip);
        if (curGameStatus == EGameStatus.PLAYING || curGameStatus == EGameStatus.PAUSE)
        {
            curGameStatus = EGameStatus.END;
            curGameStatus = EGameStatus.STARTWAITTING;
            _buttonsTsf.Find("GameStart").gameObject.SetActive(true);
        }
    }
    private void BeatShow()
    {
        _beatTsf.GetComponent<Image>().color = new Vector4(1, 0, 0, _beatCount);
        _beatTsf.Find("Left").transform.localPosition = new Vector3(-900 + (950 * _beatCount), 0, 0);
        _beatTsf.Find("Right").transform.localPosition = new Vector3(900 - (950 * _beatCount), 0, 0);
    }
}
public partial class OtherGame_s : MonoBehaviour// cube
{
    private void PressCheck()
    {
        var checktemp = new List<KeyCode>(_cubeKeyPressCheck);
        foreach (var keylist in checktemp)
        {
            if (!Input.GetKey(keylist))
            {
                _cubeKeyPressCheck.Remove(keylist);
            }
        }
        if (_cubeKeyPressCheck.Count == 0 && _curCubeImageStatus != ECubeImage.ATTACK)
        {
            RotateCube(Vector3.zero, ECubeImage.ATTACK);
        }
    }
    private void RotateCube(Vector3 rotateposition, ECubeImage setstatus)
    {
        if (_previousRoatePos != rotateposition)
        {
            _isSetting = true;
            _previousRoatePos = rotateposition;
            if (rotateposition != Vector3.zero)
            {
                switch (_curCubeObjStatus)
                {
                    case ECubeObjStatus.ROTATION:
                        _rotationQueue.Clear();
                        _rotationQueue.Enqueue(RotateTimeLock(Vector3.zero, _gameCube, ECubeImage.ATTACK, _rotateCount));
                        if (_previousRotation != null) StopCoroutine(_previousRotation);
                        _curCubeObjStatus = ECubeObjStatus.STOP;
                        break;
                    case ECubeObjStatus.PRESS:
                        _rotationQueue.Enqueue(RotateTimeLock(Vector3.zero, _gameCube, ECubeImage.ATTACK, _rotateCount));
                        break;
                    default:
                        break;
                }
            }
            _rotationQueue.Enqueue(RotateTimeLock(rotateposition, _gameCube, setstatus, _defaultRotateTime));
            _isSetting = false;
        }

    }
    IEnumerator RotateTimeLock(Vector3 rotateposition, GameObject targetobj, ECubeImage setstatus, int rotatetime)
    {
        _curCubeObjStatus = ECubeObjStatus.ROTATION;
        _rotateCount = 0;
        Vector3 rotatepos = new Vector3(
        (rotateposition.x - targetobj.transform.eulerAngles.x) / rotatetime,
        (rotateposition.y - targetobj.transform.eulerAngles.y) / rotatetime,
        (rotateposition.z - targetobj.transform.eulerAngles.z) / rotatetime
        );
        if (targetobj.transform.rotation.w < 0 || targetobj.transform.rotation.x < 0 || targetobj.transform.rotation.y < 0 || targetobj.transform.rotation.z < 0)
        {
            rotatepos = new Vector3(
            targetobj.transform.eulerAngles.x > 0 ? (360 - rotateposition.x - targetobj.transform.eulerAngles.x) / rotatetime : (rotateposition.x - targetobj.transform.eulerAngles.x) / rotatetime,
            targetobj.transform.eulerAngles.y > 0 ? (360 - rotateposition.y - targetobj.transform.eulerAngles.y) / rotatetime : (rotateposition.y - targetobj.transform.eulerAngles.y) / rotatetime,
            targetobj.transform.eulerAngles.z > 0 ? (360 - rotateposition.z - targetobj.transform.eulerAngles.z) / rotatetime : (rotateposition.z - targetobj.transform.eulerAngles.z) / rotatetime
            );
        }
        WaitForSeconds Wait = new WaitForSeconds(0.1f / rotatetime);
        for (int i = 0; i < rotatetime; i++)
        {
            while (curGameStatus == EGameStatus.PAUSE)
            {
                yield return _GameWaitWFS;
            }
            targetobj.transform.Rotate(rotatepos);
            yield return Wait;
            _rotateCount++;
        }
        targetobj.transform.eulerAngles = rotateposition;
        _curCubeImageStatus = setstatus;
        if (_cubeKeyPressCheck.Count != 0)
        {
            _curCubeObjStatus = ECubeObjStatus.PRESS;
        }
        else
        {
            _curCubeObjStatus = ECubeObjStatus.STOP;
        }
    }
    private void CubeImageChange(ECubeImage image)
    {
        GameObject instStack = Instantiate(_cubeStackSample, _cubeStackTSF);
        instStack.transform.localPosition = new Vector2(-100 + _cubeStackTSF.childCount * 100, 0);
        //instStack.GetComponent<Image>().sprite = Resources.Load()
    }
}