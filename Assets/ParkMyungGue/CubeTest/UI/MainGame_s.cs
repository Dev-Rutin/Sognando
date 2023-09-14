using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

enum GameStatus
{
    NONE,
    STARTWAITTING,
    PLAYING,
    END,
    PAUSE
}
public enum Cube_Note_Status
{
    DEFAULT,
    UP,
    DOWN,
    LEFT,
    RIGHT
}
public enum CubeObjStatus
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
    public void ChangeVolume(float volume);
}
public partial class MainGame_s : MonoBehaviour, IUI  //Display
{
    //main system
    Dictionary<KeyCode, Action> OtherKeyBinds;
    Data MusicData;
    //cube
    Dictionary<KeyCode, Action> CubeKeyBinds;
    Queue<KeyCode> KeyInputQueue;  
    List<KeyCode> CubeKeyPressCheck;
    Queue<IEnumerator> RotationQueue;
    bool IsSetting;
    Vector3 PreviousRoatePos;
    IEnumerator IPreviousRotation;
    int rotatecount;
    //note
    Queue<Note> NoteQueue;
    Dictionary<GameObject, Note> Notes;
    WaitForFixedUpdate NoteWait;

    [Header("Required Value")]
    //main system
    [SerializeField] Transform ButtonsTsf;
    [SerializeField] AudioManager_s AudioManager;
    //cube
    [SerializeField] GameObject GameCube;
    //note
    [SerializeField] GameObject ScoreObj;

    [Header("Only Display")]
    //main system
    [SerializeField] AudioClip MusicClip;
    //cube
    [SerializeField] GameStatus CurGameStatus;
    [SerializeField] CubeObjStatus CurCubeObjStatus;
    [SerializeField] Cube_Note_Status CurCubeImageStatus;
   
    [Header("Changeable Value")]
    //cube
    [SerializeField] int DefaultRotateTime;
    //note
    [SerializeField] float perfect;
    [SerializeField] float good;
}
public partial class MainGame_s : MonoBehaviour ,IUI //main system
{
    void Start()
    {
        //main system
        OtherKeyBinds = new Dictionary<KeyCode, Action>();
        MusicData = new Data();
        //cube
        CubeKeyBinds = new Dictionary<KeyCode, Action>();
        KeyInputQueue = new Queue<KeyCode>();
        CubeKeyPressCheck = new List<KeyCode>();
        RotationQueue = new Queue<IEnumerator>();
        //note
        NoteQueue = new Queue<Note>();
        Notes = new Dictionary<GameObject, Note>();           
        NoteWait = new WaitForFixedUpdate();

        DefaultDataSetting();
        DefaultValueSetting();
        BindSetting();
        ButtonBind();
        //only Test
        UIOpen();
        MusicSetting("Test150");    
    }
    void Update()
    {
        if (CurGameStatus == GameStatus.PLAYING)
        {
            if (Input.anyKeyDown)
            {
                foreach (var dic in OtherKeyBinds)
                {
                    if (Input.GetKey(dic.Key))
                    {
                        dic.Value();
                    }
                }
                foreach (var dic in CubeKeyBinds)
                {
                    if (Input.GetKey(dic.Key) && !CubeKeyPressCheck.Contains(dic.Key))
                    {
                        KeyInputQueue.Enqueue(dic.Key);
                        CubeKeyPressCheck.Add(dic.Key);
                    }
                }
            }
            if (KeyInputQueue.Count != 0 && !IsSetting)
            {
                CubeKeyBinds[KeyInputQueue.Dequeue()]();
            }
            if (RotationQueue.Count != 0 && CurCubeObjStatus != CubeObjStatus.ROTATION)
            {
                IPreviousRotation = RotationQueue.Peek();
                StartCoroutine(RotationQueue.Dequeue());
            }
            PressCheck();
        }
        else
        {
            if (Input.anyKeyDown)
            {
                foreach (var dic in OtherKeyBinds)
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
        CurCubeImageStatus = Cube_Note_Status.DEFAULT;
        CurCubeObjStatus = CubeObjStatus.STOP;
        IsSetting = false;
        CurGameStatus = GameStatus.STARTWAITTING;
    }
    public void UIPause()
    {
        if (CurGameStatus == GameStatus.PAUSE)
        {
            CurGameStatus = GameStatus.PLAYING;
        }
        else
        {
            CurGameStatus = GameStatus.PAUSE;
            AudioManager.AudioPause();   
        }
    }
    public void UIClose()
    {
        CurGameStatus = GameStatus.NONE;
    }
    void DefaultDataSetting()
    {
        if (ScoreObj == null)
            ScoreObj = transform.Find("UI").Find("Canvas").Find("Score").gameObject;  
        if(GameCube==null)
            GameCube = GameObject.Find("Cube").gameObject;
        if (ButtonsTsf==null)
            ButtonsTsf = transform.Find("UI").Find("Canvas").Find("Buttons");
        if (AudioManager == null)
            AudioManager= GameObject.Find("AudioManager").GetComponent<AudioManager_s>();
    }
    void DefaultValueSetting()
    {
        DefaultRotateTime = DefaultRotateTime == 0 ? 100 : DefaultRotateTime;
        perfect = perfect == 0 ? 0.3f : perfect;
        good = good == 0 ? 1f : good;
        CurGameStatus = GameStatus.NONE;
    }
    void BindSetting()
    {
        KeyBind(ref CubeKeyBinds,KeyCode.LeftArrow, () => RotateCube(new Vector3(0, -90, 0),Cube_Note_Status.LEFT));
        KeyBind(ref CubeKeyBinds,KeyCode.RightArrow, () => RotateCube(new Vector3(0, 90, 0), Cube_Note_Status.RIGHT));
        KeyBind(ref CubeKeyBinds, KeyCode.UpArrow, () => RotateCube(new Vector3(-90, 0, 0), Cube_Note_Status.UP));
        KeyBind(ref CubeKeyBinds, KeyCode.DownArrow, () => RotateCube(new Vector3(90, 0, 0), Cube_Note_Status.DOWN));
        KeyBind(ref CubeKeyBinds, KeyCode.A, () => RotateCube(new Vector3(0, -90, 0), Cube_Note_Status.LEFT));
        KeyBind(ref CubeKeyBinds, KeyCode.D, () => RotateCube(new Vector3(0, 90, 0), Cube_Note_Status.RIGHT));
        KeyBind(ref CubeKeyBinds, KeyCode.W, () => RotateCube(new Vector3(-90, 0, 0), Cube_Note_Status.UP));
        KeyBind(ref CubeKeyBinds, KeyCode.S, () => RotateCube(new Vector3(90, 0, 0), Cube_Note_Status.DOWN));
        KeyBind(ref OtherKeyBinds, KeyCode.Space, () => UIPause());
    }
    void KeyBind(ref Dictionary<KeyCode,Action> binddic,KeyCode keycode, Action action)
    {
        if (!OtherKeyBinds.ContainsKey(keycode) && !CubeKeyBinds.ContainsKey(keycode))
        {
            binddic.Add(keycode, action);
        }
    }
   void ButtonBind()
    {     
        foreach (Transform child in ButtonsTsf)
        {
                child.GetComponent<Button>().onClick.AddListener((UnityAction)Delegate.CreateDelegate(typeof(UnityAction), this,child.name));
        }
    }
    public void MusicSetting(string name)
    {
        if(CurGameStatus==GameStatus.STARTWAITTING)
        {
            MusicData.SetPath((Resources.Load("ParkMyungGue\\XML\\" + name)));
            if (MusicData.xdocPath != null)
            {
                MusicClip = Resources.Load<AudioClip>("ParkMyungGue\\Music\\" + name);
            }
        }
    }
    void GameStart()
    {
        if (CurGameStatus==GameStatus.STARTWAITTING)
        {
            CurGameStatus = GameStatus.PLAYING;
            ButtonsTsf.Find("GameStart").gameObject.SetActive(false);
        }
        AudioManager.AudioPlay(MusicClip);
        StartCoroutine(GamePlaying());
    }
  
    IEnumerator GamePlaying()
    {
        TimeSpan curtime = new TimeSpan(0,0,0);
        foreach(var data in MusicData.NoteLoadAll())
        {
            NoteQueue.Enqueue(data);
        }
        WaitForSeconds time = new WaitForSeconds(0.1f);       
        while (NoteQueue.Count!=0 )
        {
            if (CurGameStatus == GameStatus.PLAYING)
            {
                if (curtime == NoteQueue.Peek().time)
                {
                    StartCoroutine(NoteMoving(CreateNote(NoteQueue.Dequeue())));
                }
                curtime = curtime.Add(new TimeSpan(0, 0, 0, 0, 100));
            }
            yield return time;         
        }
    }
    void GameEnd()
    {
        AudioManager.AudioStop(MusicClip);
        NoteQueue.Clear();
        if (CurGameStatus == GameStatus.PLAYING)
        {
            CurGameStatus = GameStatus.END;
            var copy = new Dictionary<GameObject, Note>(Notes);
            foreach (var data in copy)
            {
                Notes.Remove(data.Key);
                Destroy(data.Key);
            }        
            CurGameStatus = GameStatus.STARTWAITTING;
            ButtonsTsf.Find("GameStart").gameObject.SetActive(true);
        }   
    }
}
public partial class MainGame_s : MonoBehaviour, IUI // cube
{
    void PressCheck()
    {     
        var checktemp = new List<KeyCode>(CubeKeyPressCheck);
        foreach (var keylist in checktemp)
        {
            if (!Input.GetKey(keylist))
            {
                CubeKeyPressCheck.Remove(keylist);
            }
        }
        if (CubeKeyPressCheck.Count == 0&&CurCubeImageStatus!=Cube_Note_Status.DEFAULT)
        {
            RotateCube(Vector3.zero, Cube_Note_Status.DEFAULT);
        }    
    }

    void RotateCube(Vector3 rotateposition, Cube_Note_Status setstatus)
    {     
        if (PreviousRoatePos != rotateposition)
        {
            IsSetting = true;           
            PreviousRoatePos = rotateposition;
            if (rotateposition != Vector3.zero)
            {
                switch (CurCubeObjStatus)
                {
                    case CubeObjStatus.ROTATION:
                        RotationQueue.Clear();
                        RotationQueue.Enqueue(RotateTimeLock(Vector3.zero, GameCube, Cube_Note_Status.DEFAULT, rotatecount));
                        if(IPreviousRotation!=null)StopCoroutine(IPreviousRotation);
                        CurCubeObjStatus = CubeObjStatus.STOP;
                        break;
                    case CubeObjStatus.PRESS:
                        RotationQueue.Enqueue(RotateTimeLock(Vector3.zero, GameCube, Cube_Note_Status.DEFAULT, rotatecount));               
                        break;
                    default:
                        break;
                }
            }  
            RotationQueue.Enqueue(RotateTimeLock(rotateposition, GameCube, setstatus, DefaultRotateTime));
            IsSetting = false;
        }
      
    }
    IEnumerator RotateTimeLock(Vector3 rotateposition, GameObject targetobj, Cube_Note_Status setstatus,int rotatetime)
    {    
        CurCubeObjStatus = CubeObjStatus.ROTATION;
        CurCubeImageStatus = setstatus;
        WaitForSeconds Wait = new WaitForSeconds(0.1f / rotatetime);
            rotatecount = 0;
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
            for (int i = 0; i < rotatetime; i++)
            {
            while(CurGameStatus==GameStatus.PAUSE)
            {
                yield return Wait;
            }
                targetobj.transform.Rotate(rotatepos);
                yield return Wait;
                rotatecount++;
            }
        targetobj.transform.eulerAngles = rotateposition;
        if(CubeKeyPressCheck.Count!=0)
        {
            CurCubeObjStatus = CubeObjStatus.PRESS;
        }
        else
        {
            CurCubeObjStatus = CubeObjStatus.STOP;
        }
    } 
}
public partial class MainGame_s : MonoBehaviour, IUI // note
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Note" && Notes.ContainsKey(collision.gameObject))
        {
            NoteEnd(collision.gameObject);
        }
    }
    GameObject CreateNote(Note note)
    {
        note.passCenter = false;
        GameObject instnote = Instantiate(Resources.Load<GameObject>("Prefabs\\ParkMyungGue\\Note"), transform.Find("UI").Find("Canvas").Find("Notes"));
        instnote.transform.localPosition = note.position;
        instnote.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("ParkMyungGue\\NoteImage\\" + note.type.ToString());
        Notes.Add(instnote, note);
        instnote.SetActive(true);
        return instnote;
    }
    IEnumerator NoteMoving(GameObject target)
    {
        Vector2 previouspos = target.transform.position;
        Vector2 movingposition = new Vector2(target.transform.localPosition.x * -1, target.transform.localPosition.y * -1);

        while (target != null && Vector2.Distance(target.transform.localPosition, movingposition) > 25)
        {
            if (CurGameStatus == GameStatus.PLAYING)
            {
                target.transform.localPosition = Vector2.Lerp(target.transform.localPosition, movingposition, 0.0025f);
                if ((target.transform.localPosition.x * previouspos.x < 0 || target.transform.localPosition.x == 0) && (target.transform.localPosition.y * previouspos.y < 0 || target.transform.localPosition.y == 0) && Notes.ContainsKey(target))
                {
                    Notes[target].passCenter = true;
                }
            }
            yield return NoteWait;
            if (target == null)
                break;
        }
        if (target != null)
            NoteEnd(target);
    }

    void NoteEnd(GameObject target)
    {
        if (CurCubeImageStatus == Notes[target].type&&CurCubeObjStatus!=CubeObjStatus.ROTATION)
        {
            if (Notes[target].passCenter)
            {
                if (Vector2.Distance(target.transform.position, Vector2.zero) < perfect)
                {
                    StartCoroutine(ShowScore("PERFECT!"));
                }
                else if (Vector2.Distance(target.transform.position, Vector2.zero) < good)
                {
                    StartCoroutine(ShowScore("GOOD"));
                }
                Notes.Remove(target);
                Destroy(target);
            }
            else
            {
                if (Vector2.Distance(target.transform.position, Vector2.zero) < perfect)
                {
                    StartCoroutine(ShowScore("PERFECT!"));
                    Notes.Remove(target);
                    Destroy(target);
                }
            }

        }
        else if (Notes[target].passCenter && Vector2.Distance(target.transform.position, Vector2.zero) > good)
        {
            StartCoroutine(ShowScore("MISS"));
            Notes.Remove(target);
            Destroy(target);
        }
    }
    IEnumerator ShowScore(string score)
    {
        transform.Find("UI").Find("Canvas").Find("Score").gameObject.SetActive(true);
        transform.Find("UI").Find("Canvas").Find("Score").GetComponent<TextMeshProUGUI>().text = score;
        yield return new WaitForSeconds(1f);
        transform.Find("UI").Find("Canvas").Find("Score").gameObject.SetActive(false);

    }
}