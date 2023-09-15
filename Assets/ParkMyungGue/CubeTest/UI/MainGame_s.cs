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
using UnityEngine.SocialPlatforms.Impl;
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
    public void AudioUnPause(TimeSpan time);
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
    List<Sprite> NoteImages;
    List<Material> NoteMaterial;

    [Header("Required Value")]
    //main system
    [SerializeField] Transform ButtonsTsf;
    [SerializeField] AudioManager_s AudioManager;
    [SerializeField] Transform TimeTsf;
    //cube
    [SerializeField] GameObject GameCube;
    //note
    [SerializeField] Transform ScoreTsf;
    [SerializeField] Transform ComboTsf;
    [SerializeField] GameObject NoteObj;
    [SerializeField] GameObject LongNoteObj;
    [SerializeField] Transform NotesTsf;
    [Header("Only Display")]
    //main system
    [SerializeField] AudioClip MusicClip;
    [SerializeField] TimeSpan MainGameCurTime;
    //cube
    [SerializeField] GameStatus CurGameStatus;
    [SerializeField] CubeObjStatus CurCubeObjStatus;
    [SerializeField] NoteImage CurCubeImageStatus;
    //note
    [SerializeField] int Combo;
    
    [Header("Changeable Value")]
    //cube
    [SerializeField] int DefaultRotateTime;
    //note
    [SerializeField] float perfect;
    [SerializeField] float good;
    [SerializeField] float FadeOutValue;
    [SerializeField] float NoteSpeed;
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
        NoteImages = new List<Sprite>();
        NoteMaterial = new List<Material>();

        DefaultDataSetting();
        DefaultValueSetting();
        BindSetting();
        ButtonBind();
        //only Test
        UIOpen();
        MusicSetting("Test150");    
    }
    void DefaultDataSetting()
    {
        if (ScoreTsf == null)
            ScoreTsf = transform.Find("UI").Find("Canvas").Find("Score");
        if (ComboTsf == null)
            ComboTsf = transform.Find("UI").Find("Canvas").Find("Combo");
        if (GameCube == null)
            GameCube = GameObject.Find("Cube").gameObject;
        if (ButtonsTsf == null)
            ButtonsTsf = transform.Find("UI").Find("Canvas").Find("Buttons");
        if (AudioManager == null)
            AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager_s>();
        if (TimeTsf == null)
            TimeTsf = transform.Find("UI").Find("Canvas").Find("Time");
        if (NoteObj == null)
            NoteObj = Resources.Load<GameObject>("Prefabs\\ParkMyungGue\\Note");
        if (LongNoteObj == null)
            LongNoteObj = Resources.Load<GameObject>("Prefabs\\ParkMyungGue\\LongNote");
        if (NotesTsf == null)
            NotesTsf = transform.Find("UI").Find("Canvas").Find("Notes");
        foreach (var data in Resources.LoadAll<Sprite>("ParkMyungGue\\NoteImage"))
        {
            NoteImages.Add(data);
        }
        foreach (var data in Resources.LoadAll<Material>("ParkMyungGue\\NoteMaterial"))
        {
            NoteMaterial.Add(data);
        }
    }
    void DefaultValueSetting()
    {
        DefaultRotateTime = DefaultRotateTime == 0 ? 100 : DefaultRotateTime;
        perfect = perfect == 0 ? 0.3f : perfect;
        good = good == 0 ? 1f : good;
        CurGameStatus = GameStatus.NONE;
        FadeOutValue = FadeOutValue== 0 ? 1.6f : FadeOutValue;
        NoteSpeed = NoteSpeed == 0 ? 0.0025f : NoteSpeed;
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
            TimeTsf.GetComponent<TextMeshProUGUI>().text = MainGameCurTime.ToString();

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
        CurCubeImageStatus = NoteImage.DEFAULT;
        CurCubeObjStatus = CubeObjStatus.STOP;
        IsSetting = false;
        CurGameStatus = GameStatus.STARTWAITTING;
    }
    public void UIPause()
    {
        if (CurGameStatus == GameStatus.PAUSE)
        {
            CurGameStatus = GameStatus.PLAYING;
            AudioManager.AudioUnPause(MainGameCurTime);
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
    void BindSetting()
    {
        KeyBind(ref CubeKeyBinds,KeyCode.LeftArrow, () => RotateCube(new Vector3(0, -90, 0),NoteImage.LEFT));
        KeyBind(ref CubeKeyBinds,KeyCode.RightArrow, () => RotateCube(new Vector3(0, 90, 0), NoteImage.RIGHT));
        KeyBind(ref CubeKeyBinds, KeyCode.UpArrow, () => RotateCube(new Vector3(-90, 0, 0), NoteImage.UP));
        KeyBind(ref CubeKeyBinds, KeyCode.DownArrow, () => RotateCube(new Vector3(90, 0, 0), NoteImage.DOWN));
        KeyBind(ref CubeKeyBinds, KeyCode.A, () => RotateCube(new Vector3(0, -90, 0), NoteImage.LEFT));
        KeyBind(ref CubeKeyBinds, KeyCode.D, () => RotateCube(new Vector3(0, 90, 0), NoteImage.RIGHT));
        KeyBind(ref CubeKeyBinds, KeyCode.W, () => RotateCube(new Vector3(-90, 0, 0), NoteImage.UP));
        KeyBind(ref CubeKeyBinds, KeyCode.S, () => RotateCube(new Vector3(90, 0, 0), NoteImage.DOWN));
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
        if (CurGameStatus == GameStatus.STARTWAITTING)
        {
            Combo = 0;
            MainGameCurTime = new TimeSpan(0, 0, 0);
            CurGameStatus = GameStatus.PLAYING;
            ButtonsTsf.Find("GameStart").gameObject.SetActive(false);
            AudioManager.AudioPlay(MusicClip);
            StartCoroutine(GamePlaying());
        }
    }
  
    IEnumerator GamePlaying()
    {
      
        foreach(var data in MusicData.NoteLoadAll())
        {
            NoteQueue.Enqueue(data);
        }
        WaitForSeconds time = new WaitForSeconds(0.01f);       
        while (MainGameCurTime.TotalSeconds<= MusicClip.length)
        {
            if (CurGameStatus == GameStatus.PLAYING)
            {
                if (NoteQueue.Count != 0)
                {
                    if (MainGameCurTime == NoteQueue.Peek().time)
                    {
                        CreateNote(NoteQueue.Dequeue());
                    }
                }
                MainGameCurTime = MainGameCurTime.Add(new TimeSpan(0, 0, 0, 0, 010));
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
        if (CubeKeyPressCheck.Count == 0&&CurCubeImageStatus!=NoteImage.DEFAULT)
        {
            RotateCube(Vector3.zero, NoteImage.DEFAULT);
        }    
    }

    void RotateCube(Vector3 rotateposition, NoteImage setstatus)
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
                        RotationQueue.Enqueue(RotateTimeLock(Vector3.zero, GameCube, NoteImage.DEFAULT, rotatecount));
                        if(IPreviousRotation!=null)StopCoroutine(IPreviousRotation);
                        CurCubeObjStatus = CubeObjStatus.STOP;
                        break;
                    case CubeObjStatus.PRESS:
                        RotationQueue.Enqueue(RotateTimeLock(Vector3.zero, GameCube, NoteImage.DEFAULT, rotatecount));               
                        break;
                    default:
                        break;
                }
            }  
            RotationQueue.Enqueue(RotateTimeLock(rotateposition, GameCube, setstatus, DefaultRotateTime));
            IsSetting = false;
        }
      
    }
    IEnumerator RotateTimeLock(Vector3 rotateposition, GameObject targetobj, NoteImage setstatus,int rotatetime)
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
    void CreateNote(Note note)
    {
        note.passCenter = false;      
        if (note.type == NoteType.LONG)
        {
            StartCoroutine(CreateLongNote(note));
        }
        else
        {
            GameObject instnote;
            instnote = Instantiate(NoteObj, NotesTsf);
            instnote.transform.localPosition = note.position;
            instnote.GetComponent<SpriteRenderer>().sprite = NoteImages.Find((Sprite sample) => sample.name == note.image.ToString());
            Notes.Add(instnote, note);
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
            instnote = Instantiate(LongNoteObj, NotesTsf);
            instnote.transform.localPosition = note.position;
            instnote.GetComponent<SpriteRenderer>().sprite = NoteImages.Find((Sprite sample) => sample.name == note.image.ToString());
            instnote.GetComponent<ParticleSystemRenderer>().material = NoteMaterial.Find((Material sample) => sample.name == note.image.ToString());
            instnote.GetComponent<ParticleSystem>().Play();
            Notes.Add(instnote, note);
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
            if (CurGameStatus == GameStatus.PLAYING)
            {
                target.transform.localPosition = Vector2.Lerp(target.transform.localPosition, movingposition, NoteSpeed);
                if ((target.transform.localPosition.x * previouspos.x < 0 || target.transform.localPosition.x == 0) && (target.transform.localPosition.y * previouspos.y < 0 || target.transform.localPosition.y == 0) && Notes.ContainsKey(target))
                {
                    target.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, FadeOutValue - Vector2.Distance(previouspos, movingposition) / Vector2.Distance(target.transform.localPosition, movingposition));
                    Notes[target].passCenter = true;               
                }
            }
            yield return NoteWait;
            if (target == null)
                break;
        }
        if (target != null)
        {
            NoteEnd(target);
            Debug.Log("aa");
        }
    }
    void NoteEnd(GameObject target)
    {
        if (CurCubeImageStatus == Notes[target].image&&CurCubeObjStatus!=CubeObjStatus.ROTATION)
        {
            if (Notes[target].passCenter)
            {
                if (Vector2.Distance(target.transform.position, Vector2.zero) < perfect)
                {
                    ShowScore("PERFECT!");
                    Combo++;
                    ShowCombo();

                }
                else if (Vector2.Distance(target.transform.position, Vector2.zero) < good)
                {
                    ShowScore("GOOD");
                    Combo++;
                    ShowCombo();
                }
                Notes.Remove(target);
                Destroy(target);
            }
            else if (Vector2.Distance(target.transform.position, Vector2.zero) < perfect)
            {
                ShowScore("PERFECT!");
                Combo++;
                ShowCombo();
                Notes.Remove(target);
                Destroy(target);
            }
        }
        else if (Notes[target].passCenter && Vector2.Distance(target.transform.position, Vector2.zero) > good)
        {
            ShowScore("MISS");
            Combo=0;
            ShowCombo();
            Notes.Remove(target);
            Destroy(target);
        }
    }
    void ShowScore(string score)
    {
        ScoreTsf.GetComponent<TextMeshProUGUI>().text = score;
    }
    void ShowCombo()
    {
        ComboTsf.GetComponent<TextMeshProUGUI>().text = Combo.ToString();
    }
}