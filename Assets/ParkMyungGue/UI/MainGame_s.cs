using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum EGameStatus
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
public interface IAudio
{
    public void AudioPlay(AudioClip clip);
    public void AudioStop(AudioClip clip);
    public void AudioPause();
    public void AudioUnPause(TimeSpan time);
    public void ChangeVolume(float volume);
}
public partial class MainGame_s : MonoBehaviour  //Display
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
    //note
    private List<Note> _noteList;
    private Dictionary<Note, List<GameObject>> _notes;
    private List<Sprite> _noteImages;
    private List<Material> _noteMaterial;
    public delegate void NoteDelegate(Note note);
    public event NoteDelegate CreateNoteEvent;
    public event NoteDelegate DeleteNoteEvent;

    //time !!!!!TIME : defulat was seconds!!!!!
    private WaitForSeconds _GameWaitWFS;
    private WaitForSeconds _longNoteCreateWaitWFS;
    private WaitForFixedUpdate _noteMovingWaitWFS;
    public int _speedLoader;
    private float _beatCount;

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
    //time
    [SerializeField] private Transform _beatTsf;
    [Header("Only Display")]
    //main system
    [SerializeField] public AudioClip musicClip;
    [SerializeField] public TimeSpan curMainGameTime;
    //cube
    [SerializeField] public EGameStatus curGameStatus;
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
    [SerializeField] private int _missRange;
    [SerializeField] private float _fadeOutValue;
    [SerializeField] private float _noteSpeed;
    //time
    [SerializeField] private float _gameWait;
    [SerializeField] private float _longNoteCreateWait;
    [SerializeField] private float _beatTime;
    //test
    [SerializeField] private string _musicName;
}
public partial class MainGame_s : MonoBehaviour //main system
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
        //note
        _noteList = new List<Note>();
        _notes = new Dictionary<Note, List<GameObject>>();           
        _noteImages = new List<Sprite>();
        _noteMaterial = new List<Material>();
        //time
        _noteMovingWaitWFS = new WaitForFixedUpdate();
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
        CreateNoteEvent += CreateNote;
    }
    private void DefaultValueSetting()
    {
        _longNoteCreateWait = _longNoteCreateWait == 0 ? 0.5f : _longNoteCreateWait;
        _defaultRotateTime = _defaultRotateTime == 0 ? 100 : _defaultRotateTime;
        _perfect = _perfect == 0 ? 35f : _perfect;
        _good = _good == 0 ? 100f : _good;
        _missRange = _missRange == 0 ? 165 : _missRange;
        curGameStatus = EGameStatus.NONE;
        _fadeOutValue = _fadeOutValue== 0 ? 1.6f : _fadeOutValue;
        _noteSpeed = _noteSpeed == 0 ? 0.005f: _noteSpeed;
        _gameWait = _gameWait == 0 ? 0.05f : _gameWait;
        _speedLoader = _speedLoader == 0 ? 1 : _speedLoader;
        _beatTime = _beatTime == 0 ? 1f : _beatTime;
    }
    public void TimeSetting()
    {
        _GameWaitWFS = new WaitForSeconds(_gameWait/_speedLoader);
        _longNoteCreateWaitWFS = new WaitForSeconds(_longNoteCreateWait/_speedLoader);
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
                            ShowScore("");
                        }
                        else
                        {
                            ShowScore("Miss Beat");
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
        _curCubeImageStatus = ENoteImage.DEFAULT;
        _curCubeObjStatus = ECubeObjStatus.STOP;
        _isSetting = false;
        curGameStatus = EGameStatus.STARTWAITTING;
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
        if (curGameStatus == EGameStatus.STARTWAITTING)
        {
            _combo = 0;
            ShowCombo();
            curMainGameTime = new TimeSpan(0, 0, 0);
            curGameStatus = EGameStatus.PLAYING;
            _buttonsTsf.Find("GameStart").gameObject.SetActive(false);
            _audioManager.AudioPlay(musicClip);
            _noteList = musicData.LoadNoteAll();
            StartCoroutine(GamePlaying());
        }
    }
    IEnumerator GamePlaying()
    {            
        while (curMainGameTime.TotalSeconds<= musicClip.length&& (curGameStatus == EGameStatus.PLAYING || curGameStatus == EGameStatus.PAUSE))
        {
            if (curGameStatus == EGameStatus.PLAYING)
            {           
                if (_noteList.Count != 0)
                {
                    if (_noteList[0].time.TotalMilliseconds <= (curMainGameTime + new TimeSpan(0, 0, 0, 0, (int)(_gameWait * 1000))).TotalMilliseconds &&
                    _noteList[0].time.TotalMilliseconds >= (curMainGameTime - new TimeSpan(0, 0, 0, 0, (int)(_gameWait * 1000))).TotalMilliseconds)
                    {
                    
                    }
                    Note temp = _noteList.Find(temp => temp.time.TotalMilliseconds <= (curMainGameTime + new TimeSpan(0, 0, 0, 0, (int)(_gameWait * 1000))).TotalMilliseconds &&
                    temp.time.TotalMilliseconds >= (curMainGameTime - new TimeSpan(0, 0, 0, 0, (int)(_gameWait * 1000))).TotalMilliseconds);
                    if (temp != null&&!_notes.ContainsKey(temp))
                    {
                        temp = new Note(temp);
                        CreateNoteEvent.Invoke(temp);
                    }
                }
                curMainGameTime = curMainGameTime.Add(new TimeSpan(0,0,0,0,(int)(_gameWait*1000)));
                _beatCount =(float)((curMainGameTime.TotalSeconds / _beatTime) - (int)(curMainGameTime.TotalSeconds / _beatTime));
                BeatShow();
            }
            yield return _GameWaitWFS;         
        }
    }
    private void GameEnd()
    {
        _combo = 0;
        ShowCombo();
        ShowScore("");
        _audioManager.AudioStop(musicClip);
        _noteList.Clear();
        if (curGameStatus == EGameStatus.PLAYING||curGameStatus==EGameStatus.PAUSE)
        {
            curGameStatus = EGameStatus.END;
            var temp = new Dictionary<Note, List<GameObject>>(_notes);
            foreach (var data in temp)
            {
                DestroyNoteAll(data.Key);
            }        
            curGameStatus = EGameStatus.STARTWAITTING;
            _buttonsTsf.Find("GameStart").gameObject.SetActive(true);
        }
    }

    public List<GameObject> NoteToGameObjectOrNull(Note note)
    {
        if(_notes.ContainsKey(note))
        {
            return _notes[note];
        }
        return null;
    }
    public Note GameObjectToNoteOrNull(GameObject obj)
    {
        foreach (var data in _notes)
        {
            if(data.Value.Contains(obj))
            {
                return data.Key;
            }
        }
        return null;
    }   
    public bool ContainsGameOjbectCheck(GameObject obj)
    {
        foreach (var data in _notes)
        {
            if (data.Value.Contains(obj))
            {
                return true;
            }
        }
        return false;
    }

    void BeatShow()
    {
        _beatTsf.GetComponent<Image>().color = new Vector4(1, 0, 0, _beatCount);
        _beatTsf.Find("Left").transform.localPosition = new Vector3(-900+(950*_beatCount), 0, 0);
        _beatTsf.Find("Right").transform.localPosition = new Vector3(900 - (950 * _beatCount), 0, 0);
    }
}
public partial class MainGame_s : MonoBehaviour // cube
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
            while(curGameStatus==EGameStatus.PAUSE)
            {
                yield return _GameWaitWFS;
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
public partial class MainGame_s : MonoBehaviour// note
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Note" &&ContainsGameOjbectCheck(collision.gameObject))
        {
            if (ContainsGameOjbectCheck(collision.gameObject))
            {
                EndNote(GameObjectToNoteOrNull(collision.gameObject) , collision.gameObject);
            }
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
            _notes.Add(note,new List<GameObject>() {instnote});
            instnote.SetActive(true);
            StartCoroutine(NoteMoving(note, instnote));
        }
    }
    IEnumerator CreateLongNote(Note note)
    {
        GameObject instnote;
        _notes.Add(note, new List<GameObject>());
        TimeSpan startTime = curMainGameTime;
        for (double t = startTime.TotalMilliseconds; t<=note.time2.TotalMilliseconds; t+=_longNoteCreateWait)
        {
            if(curGameStatus!=EGameStatus.PLAYING)
            {
               while(curGameStatus!=EGameStatus.PLAYING)
                {
                    yield return _GameWaitWFS;
                }
            }
            instnote = Instantiate(_longNoteObj, _notesTsf);
            instnote.transform.localPosition = note.position;
            instnote.GetComponent<SpriteRenderer>().sprite = _noteImages.Find((Sprite sample) => sample.name == note.image.ToString());
            instnote.GetComponent<ParticleSystemRenderer>().material = _noteMaterial.Find((Material sample) => sample.name == note.image.ToString());
            instnote.GetComponent<ParticleSystem>().Play();
            _notes[note].Add(instnote);
            instnote.SetActive(true);
            StartCoroutine(NoteMoving(note,instnote));
            yield return _longNoteCreateWaitWFS;
        }
     
    }
    IEnumerator NoteMoving(Note target,GameObject obj)
    {
        Vector2 previouspos = obj.transform.position;
        Vector2 movingposition = new Vector2(obj.transform.localPosition.x * -1, obj.transform.localPosition.y * -1);
        while (obj != null)
        {
            if (curGameStatus == EGameStatus.PLAYING)
            {
                obj.transform.localPosition = Vector2.Lerp(obj.transform.localPosition, movingposition, _noteSpeed*_speedLoader);
                if ((obj.transform.localPosition.x * previouspos.x < 0 || obj.transform.localPosition.x == 0) && (obj.transform.localPosition.y * previouspos.y < 0 || obj.transform.localPosition.y == 0) && _notes.ContainsKey(target))
                {
                    obj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, _fadeOutValue - Vector2.Distance(previouspos, movingposition) / Vector2.Distance(obj.transform.localPosition, movingposition));
                    target.isPassCenter = true;               
                }
                else
                {
                    target.isPassCenter = false;
                }
            }
            if((Vector2.Distance(obj.transform.localPosition, Vector2.zero) >= _missRange && target.isPassCenter)|| (!ContainsGameOjbectCheck(obj)))
            {
                break;
            }
            yield return _noteMovingWaitWFS;
        }
        if (ContainsGameOjbectCheck(obj))
        {
            EndNote(target,obj);
        }
    }
    private void EndNote(Note target,GameObject obj)
    {
        if (_curCubeImageStatus == target.image&&_curCubeObjStatus!=ECubeObjStatus.ROTATION)
        {
            if (target.isPassCenter)
            {
                if (Vector2.Distance(obj.transform.localPosition, Vector2.zero) <= _perfect)
                {
                    ShowScore("PERFECT!");
                    _combo++;
                    ShowCombo();
                    DestroyNote(target, obj);
                    return;

                }
                else if (Vector2.Distance(obj.transform.localPosition, Vector2.zero) <= _good)
                {
                    ShowScore("GOOD");
                    _combo++;
                    ShowCombo();
                    DestroyNote(target, obj);
                    return;
                }
            }
            else if (Vector2.Distance(obj.transform.localPosition, Vector2.zero) <= _perfect)
            {
                ShowScore("PERFECT!");
                _combo++;
                ShowCombo();
                DestroyNote(target, obj);
                return;
            }
        }
        else if (!target.isPassCenter&&_curCubeObjStatus==ECubeObjStatus.ROTATION&& _curCubeImageStatus == target.image&& Vector2.Distance(obj.transform.localPosition, Vector2.zero) <= _good)
        {
            ShowScore("GOOD");
            _combo++;
            ShowCombo();
            DestroyNote(target, obj);
            return;
        }
        else if (target.isPassCenter && Vector2.Distance(obj.transform.localPosition, Vector2.zero) >=_missRange)
        {
            ShowScore("MISS");
            _combo=0;
            ShowCombo();
            DestroyNote(target, obj);
            return;
        }
    }
    private void DestroyNoteAll(Note target)
    {
        int count = _notes[target].Count;
        for (int i = 0; i < count; i++)
        {
            Destroy(_notes[target][i]);
        }
        _notes[target].Clear();
        _notes.Remove(target);
    }
    private void DestroyNote(Note target, GameObject obj)
    {
        _notes[target].Remove(obj);
        Destroy(obj);
       
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
