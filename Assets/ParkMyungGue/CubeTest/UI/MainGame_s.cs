using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

enum EGameStatus
{
    NONE,
    STARTWAITTING,
    PLAYING,
    END,
    PAUSE
}
public enum ECubeObjStatus
{
    STOP,
    ROTATION,
    PRESS
}
interface IUI
{ 
    public void UIOpen();
    public void UIClose();
    public void UIPause();
}
public interface IAudio
{
    public void AudioPlay(AudioClip clip);
    public void AudioStop(AudioClip clip);
    public void AudioPause();
    public void AudioUnPause(TimeSpan time);
    public void ChangeVolume(float volume);
}
public partial class MainGame_s : MonoBehaviour, IUI  //Display
{
    //main system
    private Dictionary<KeyCode, Action> _otherKeyBinds;
    private Data _musicData;
    //cube
    private Dictionary<KeyCode, Action> _cubeKeyBinds;
    private Queue<KeyCode> _keyInputQueue;  
    private List<KeyCode> _cubeKeyPressCheck;
    private Queue<IEnumerator> _rotationQueue;
    private bool _isSetting;
    private Vector3 _previousRoatePos;
    private IEnumerator _previousRotation;
    private int _rotateCount;
    //note
    private Queue<Note> _noteQueue;
    private Dictionary<GameObject, Note> _notes;
    private WaitForFixedUpdate _noteWait;
    private List<Sprite> _noteImages;
    private List<Material> _noteMaterial;

    [Header("Required Value")]
    //main system
    [SerializeField] private Transform _buttonsTsf;
    [SerializeField] private AudioManager_s _audioManager;
    [SerializeField] private Transform _timeTsf;
    //cube
    [SerializeField] private GameObject _gameCube;
    //note
    [SerializeField] private Transform _scoreTsf;
    [SerializeField] private Transform _comboTsf;
    [SerializeField] private GameObject _noteObj;
    [SerializeField] private GameObject _longNoteObj;
    [SerializeField] private Transform _notesTsf;
    [Header("Only Display")]
    //main system
    [SerializeField] private AudioClip _musicClip;
    [SerializeField] private TimeSpan _mainGameCurTime;
    //cube
    [SerializeField] private EGameStatus _curGameStatus;
    [SerializeField] private ECubeObjStatus _curCubeObjStatus;
    [SerializeField] private ENoteImage _curCubeImageStatus;
    //note
    [SerializeField] private int _combo;
    
    [Header("Changeable Value")]
    //cube
    [SerializeField] private int _defaultRotateTime;
    //note
    [SerializeField] private float _perfect;
    [SerializeField] private float _good;
    [SerializeField] private float _fadeOutValue;
    [SerializeField] private float _noteSpeed;
}
public partial class MainGame_s : MonoBehaviour ,IUI //main system
{
    private void Start()
    {
        //main system
        _otherKeyBinds = new Dictionary<KeyCode, Action>();
        _musicData = new Data();
        //cube
        _cubeKeyBinds = new Dictionary<KeyCode, Action>();
        _keyInputQueue = new Queue<KeyCode>();
        _cubeKeyPressCheck = new List<KeyCode>();
        _rotationQueue = new Queue<IEnumerator>();
        //note
        _noteQueue = new Queue<Note>();
        _notes = new Dictionary<GameObject, Note>();           
        _noteWait = new WaitForFixedUpdate();
        _noteImages = new List<Sprite>();
        _noteMaterial = new List<Material>();

        DefaultDataSetting();
        DefaultValueSetting();
        BindSetting();
        ButtonBind();
        //only Test
        UIOpen();
        MusicSetting("Test140");    
    }
    private void DefaultDataSetting()
    {
        if (_scoreTsf == null)
        {
            _scoreTsf = transform.Find("UI").Find("Canvas").Find("Score");
        }
        if (_comboTsf == null)
        {
            _comboTsf = transform.Find("UI").Find("Canvas").Find("Combo");
        }
        if (_gameCube == null)
        {
            _gameCube = GameObject.Find("Cube").gameObject;
        }
        if (_buttonsTsf == null)
        {
            _buttonsTsf = transform.Find("UI").Find("Canvas").Find("Buttons");
        }
        if (_audioManager == null)
        { 
            _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager_s>();
        }
        if (_timeTsf == null)
        { 
            _timeTsf = transform.Find("UI").Find("Canvas").Find("Time");
        }
        if (_noteObj == null)
        { 
            _noteObj = Resources.Load<GameObject>("Prefabs\\ParkMyungGue\\Note");
        }
        if (_longNoteObj == null)
        { 
            _longNoteObj = Resources.Load<GameObject>("Prefabs\\ParkMyungGue\\LongNote");
        }
        if (_notesTsf == null)
        { 
            _notesTsf = transform.Find("UI").Find("Canvas").Find("Notes");
        }
        foreach (var data in Resources.LoadAll<Sprite>("ParkMyungGue\\NoteImage"))
        {
            _noteImages.Add(data);
        }
        foreach (var data in Resources.LoadAll<Material>("ParkMyungGue\\NoteMaterial"))
        {
            _noteMaterial.Add(data);
        }
    }
    private void DefaultValueSetting()
    {
        _defaultRotateTime = _defaultRotateTime == 0 ? 100 : _defaultRotateTime;
        _perfect = _perfect == 0 ? 0.3f : _perfect;
        _good = _good == 0 ? 1f : _good;
        _curGameStatus = EGameStatus.NONE;
        _fadeOutValue = _fadeOutValue== 0 ? 1.6f : _fadeOutValue;
        _noteSpeed = _noteSpeed == 0 ? 0.0025f : _noteSpeed;
    }
    private void Update()
    {
        if (_curGameStatus == EGameStatus.PLAYING)
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
                        _keyInputQueue.Enqueue(dic.Key);
                        _cubeKeyPressCheck.Add(dic.Key);
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
            _timeTsf.GetComponent<TextMeshProUGUI>().text = _mainGameCurTime.ToString();

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
        _curCubeImageStatus = ENoteImage.DEFAULT;
        _curCubeObjStatus = ECubeObjStatus.STOP;
        _isSetting = false;
        _curGameStatus = EGameStatus.STARTWAITTING;
    }
    public void UIPause()
    {
        if (_curGameStatus == EGameStatus.PAUSE)
        {
            _curGameStatus = EGameStatus.PLAYING;
            _audioManager.AudioUnPause(_mainGameCurTime);
        }
        else
        {
            _curGameStatus = EGameStatus.PAUSE;
            _audioManager.AudioPause();   
        }
    }
    public void UIClose()
    {
        _curGameStatus = EGameStatus.NONE;
    }
    public void MusicSetting(string name)
    {
        if (_curGameStatus == EGameStatus.STARTWAITTING)
        {
            _musicData.SetPath((Resources.Load("ParkMyungGue\\XML\\" + name)));
            if (_musicData.xdocPath != null)
            {
                _musicClip = Resources.Load<AudioClip>("ParkMyungGue\\Music\\" + name);
            }
        }
    }
    private void BindSetting()
    {
        KeyBind(ref _cubeKeyBinds,KeyCode.LeftArrow, () => RotateCube(new Vector3(0, -90, 0),ENoteImage.LEFT));
        KeyBind(ref _cubeKeyBinds,KeyCode.RightArrow, () => RotateCube(new Vector3(0, 90, 0), ENoteImage.RIGHT));
        KeyBind(ref _cubeKeyBinds, KeyCode.UpArrow, () => RotateCube(new Vector3(-90, 0, 0), ENoteImage.UP));
        KeyBind(ref _cubeKeyBinds, KeyCode.DownArrow, () => RotateCube(new Vector3(90, 0, 0), ENoteImage.DOWN));
        KeyBind(ref _cubeKeyBinds, KeyCode.A, () => RotateCube(new Vector3(0, -90, 0), ENoteImage.LEFT));
        KeyBind(ref _cubeKeyBinds, KeyCode.D, () => RotateCube(new Vector3(0, 90, 0), ENoteImage.RIGHT));
        KeyBind(ref _cubeKeyBinds, KeyCode.W, () => RotateCube(new Vector3(-90, 0, 0), ENoteImage.UP));
        KeyBind(ref _cubeKeyBinds, KeyCode.S, () => RotateCube(new Vector3(90, 0, 0), ENoteImage.DOWN));
        KeyBind(ref _otherKeyBinds, KeyCode.Space, () => UIPause());
    }
    private void KeyBind(ref Dictionary<KeyCode,Action> binddic,KeyCode keycode, Action action)
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
            child.GetComponent<Button>().onClick.AddListener((UnityAction)Delegate.CreateDelegate(typeof(UnityAction), this,child.name));
        }
    }
    private void GameStart()
    {
        if (_curGameStatus == EGameStatus.STARTWAITTING)
        {
            _combo = 0;
            ShowCombo();
            _mainGameCurTime = new TimeSpan(0, 0, 0);
            _curGameStatus = EGameStatus.PLAYING;
            _buttonsTsf.Find("GameStart").gameObject.SetActive(false);
            _audioManager.AudioPlay(_musicClip);
            StartCoroutine(GamePlaying());
        }
    }
    IEnumerator GamePlaying()
    {
        List<Note> notes = _musicData.LoadNoteAllOrNull();
        if (notes != null)
        {
            foreach (var data in notes)
            {
                _noteQueue.Enqueue(data);
            }
        }
        WaitForSeconds time = new WaitForSeconds(0.01f);       
        while (_mainGameCurTime.TotalSeconds<= _musicClip.length)
        {
            if (_curGameStatus == EGameStatus.PLAYING)
            {
                if (_noteQueue.Count != 0)
                {
                    if (_mainGameCurTime == _noteQueue.Peek().time)
                    {
                        CreateNote(_noteQueue.Dequeue());
                    }
                }
                _mainGameCurTime = _mainGameCurTime.Add(new TimeSpan(0, 0, 0, 0, 010));
            }
            yield return time;         
        }
    }
    private void GameEnd()
    {
        _combo = 0;
        ShowCombo();
        ShowScore("");
        _audioManager.AudioStop(_musicClip);
        _noteQueue.Clear();
        if (_curGameStatus == EGameStatus.PLAYING)
        {
            _curGameStatus = EGameStatus.END;
            var copy = new Dictionary<GameObject, Note>(_notes);
            foreach (var data in copy)
            {
                _notes.Remove(data.Key);
                Destroy(data.Key);
            }        
            _curGameStatus = EGameStatus.STARTWAITTING;
            _buttonsTsf.Find("GameStart").gameObject.SetActive(true);       
        }   
    }
}
public partial class MainGame_s : MonoBehaviour, IUI // cube
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
        if (_cubeKeyPressCheck.Count == 0&&_curCubeImageStatus!=ENoteImage.DEFAULT)
        {
            RotateCube(Vector3.zero, ENoteImage.DEFAULT);
        }    
    }
    private void RotateCube(Vector3 rotateposition, ENoteImage setstatus)
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
                        _rotationQueue.Enqueue(RotateTimeLock(Vector3.zero, _gameCube, ENoteImage.DEFAULT, _rotateCount));
                        if(_previousRotation!=null)StopCoroutine(_previousRotation);
                        _curCubeObjStatus = ECubeObjStatus.STOP;
                        break;
                    case ECubeObjStatus.PRESS:
                        _rotationQueue.Enqueue(RotateTimeLock(Vector3.zero, _gameCube, ENoteImage.DEFAULT, _rotateCount));               
                        break;
                    default:
                        break;
                }
            }  
            _rotationQueue.Enqueue(RotateTimeLock(rotateposition, _gameCube, setstatus, _defaultRotateTime));
            _isSetting = false;
        }
      
    }
    IEnumerator RotateTimeLock(Vector3 rotateposition, GameObject targetobj, ENoteImage setstatus,int rotatetime)
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
            while(_curGameStatus==EGameStatus.PAUSE)
            {
                yield return Wait;
            }
                targetobj.transform.Rotate(rotatepos);
                yield return Wait;
                _rotateCount++;
            }
        targetobj.transform.eulerAngles = rotateposition;
        _curCubeImageStatus = setstatus;
        if (_cubeKeyPressCheck.Count!=0)
        {
            _curCubeObjStatus = ECubeObjStatus.PRESS;
        }
        else
        {
            _curCubeObjStatus = ECubeObjStatus.STOP;
        }
    } 
}
public partial class MainGame_s : MonoBehaviour, IUI // note
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Note" && _notes.ContainsKey(collision.gameObject))
        {
            NoteEnd(collision.gameObject);
        }
    }
    private void CreateNote(Note note)
    {
        note.isPassCenter = false;      
        if (note.type == ENoteType.LONG)
        {
            StartCoroutine(CreateLongNote(note));
        }
        else
        {
            GameObject instnote;
            instnote = Instantiate(_noteObj, _notesTsf);
            instnote.transform.localPosition = note.position;
            instnote.GetComponent<SpriteRenderer>().sprite = _noteImages.Find((Sprite sample) => sample.name == note.image.ToString());
            _notes.Add(instnote, note);
            instnote.SetActive(true);
            StartCoroutine(NoteMoving(instnote));
        }
    }
    IEnumerator CreateLongNote(Note note)
    {
        GameObject instnote;
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        for (TimeSpan time = new TimeSpan(0,0,0,0); time<note.time2; time+=new TimeSpan(0,0,0,0,500))
        {
            if(_curGameStatus!=EGameStatus.PLAYING)
            {
                break;
            }
            instnote = Instantiate(_longNoteObj, _notesTsf);
            instnote.transform.localPosition = note.position;
            instnote.GetComponent<SpriteRenderer>().sprite = _noteImages.Find((Sprite sample) => sample.name == note.image.ToString());
            instnote.GetComponent<ParticleSystemRenderer>().material = _noteMaterial.Find((Material sample) => sample.name == note.image.ToString());
            instnote.GetComponent<ParticleSystem>().Play();
            _notes.Add(instnote, note);
            instnote.SetActive(true);
            StartCoroutine(NoteMoving(instnote));
            yield return wait ;
        }
     
    }
    IEnumerator NoteMoving(GameObject target)
    {
        Vector2 previouspos = target.transform.position;
        Vector2 movingposition = new Vector2(target.transform.localPosition.x * -1, target.transform.localPosition.y * -1);
        while (target != null && Vector2.Distance(target.transform.localPosition, movingposition) > 360)
        {
            if (_curGameStatus == EGameStatus.PLAYING)
            {
                target.transform.localPosition = Vector2.Lerp(target.transform.localPosition, movingposition, _noteSpeed);
                if ((target.transform.localPosition.x * previouspos.x < 0 || target.transform.localPosition.x == 0) && (target.transform.localPosition.y * previouspos.y < 0 || target.transform.localPosition.y == 0) && _notes.ContainsKey(target))
                {
                    target.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, _fadeOutValue - Vector2.Distance(previouspos, movingposition) / Vector2.Distance(target.transform.localPosition, movingposition));
                    _notes[target].isPassCenter = true;               
                }
            }
            yield return _noteWait;
            if (target == null)
                break;
        }
        if (target != null)
        {
            NoteEnd(target);
        }
    }
    private void NoteEnd(GameObject target)
    {
        if (_curCubeImageStatus == _notes[target].image&&_curCubeObjStatus!=ECubeObjStatus.ROTATION)
        {
            if (_notes[target].isPassCenter)
            {
                if (Vector2.Distance(target.transform.position, Vector2.zero) < _perfect)
                {
                    ShowScore("PERFECT!");
                    _combo++;
                    ShowCombo();

                }
                else if (Vector2.Distance(target.transform.position, Vector2.zero) < _good)
                {
                    ShowScore("GOOD");
                    _combo++;
                    ShowCombo();
                }
                _notes.Remove(target);
                Destroy(target);
            }
            else if (Vector2.Distance(target.transform.position, Vector2.zero) < _perfect)
            {
                ShowScore("PERFECT!");
                _combo++;
                ShowCombo();
                _notes.Remove(target);
                Destroy(target);
            }
        }
        else if (!_notes[target].isPassCenter&&_curCubeObjStatus==ECubeObjStatus.ROTATION&& _curCubeImageStatus == _notes[target].image&& Vector2.Distance(target.transform.position, Vector2.zero) < _good)
        {
            ShowScore("GOOD");
            _combo++;
            ShowCombo();
            _notes.Remove(target);
            Destroy(target);
        }
        else if (_notes[target].isPassCenter && Vector2.Distance(target.transform.position, Vector2.zero) > _good)
        {
            ShowScore("MISS");
            _combo=0;
            ShowCombo();
            _notes.Remove(target);
            Destroy(target);
        }
    }
    private void ShowScore(string score)
    {
        _scoreTsf.GetComponent<TextMeshProUGUI>().text = score;
    }
    private void ShowCombo()
    {
        _comboTsf.GetComponent<TextMeshProUGUI>().text = _combo.ToString();
    }
}