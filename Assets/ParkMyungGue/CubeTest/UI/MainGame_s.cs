using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

enum GameStatus
{
    None,
    startwaitting,
    playing,
    end,
    pause
}
public enum Cube_Note_Status
{
    NONE,
    DEFAULT,
    UP,
    DOWN,
    LEFT,
    RIGHT
}
public interface IUI
{
    public void UIDefaultSetting();
    public void UIOpen();
    public void UIClose();

    public void ButtonBind();
}
public partial class MainGame_s : MonoBehaviour ,IUI //main system
{
    public void UIOpen()
    {

    }
    public void UIClose()
    {

    }

    Dictionary<KeyCode, Action> KeyBinds;
    List<KeyCode> KeyPressCheck;
    public GameObject GameCube;
    GameStatus CurStatus;
    WaitForSeconds Wait;
    Dictionary<GameObject, Note> Notes;
    WaitForFixedUpdate NoteWait;
    int score;
    bool IsMoving;
    Cube_Note_Status CurCubeStatus;
    public void UIDefaultSetting()
    {
        transform.Find("UI").Find("Canvas").Find("Score").gameObject.SetActive(false);
        NoteQueue = new Queue<Note>();
        GameCube = GameObject.Find("Cube").gameObject;
        BindSetting();
        ButtonBind();
        CurStatus = GameStatus.startwaitting;
        Wait = new WaitForSeconds(0.1f/100f);
        Notes = new Dictionary<GameObject, Note>();
        NoteWait = new WaitForFixedUpdate();

        IsMoving = false;
        CurCubeStatus = Cube_Note_Status.DEFAULT;
        InputQueue = new Queue<KeyCode>();
    }

    void BindSetting()
    {

        KeyBinds = new Dictionary<KeyCode, Action>();
        KeyPressCheck = new List<KeyCode>();
        BindAdd(KeyCode.LeftArrow, () => RotateCube(new Vector3(0, -90, 0),Cube_Note_Status.LEFT));
        BindAdd(KeyCode.RightArrow, () => RotateCube(new Vector3(0, 90, 0), Cube_Note_Status.RIGHT));
        BindAdd(KeyCode.UpArrow, () => RotateCube(new Vector3(-90, 0, 0), Cube_Note_Status.UP));
        BindAdd(KeyCode.DownArrow, () => RotateCube(new Vector3(90, 0, 0), Cube_Note_Status.DOWN));
        BindAdd(KeyCode.A, () => RotateCube(new Vector3(0, -90, 0), Cube_Note_Status.LEFT));
        BindAdd(KeyCode.D, () => RotateCube(new Vector3(0, 90, 0), Cube_Note_Status.RIGHT));
        BindAdd(KeyCode.W, () => RotateCube(new Vector3(-90, 0, 0), Cube_Note_Status.UP));
        BindAdd(KeyCode.S, () => RotateCube(new Vector3(90, 0, 0), Cube_Note_Status.DOWN));
    }
    void BindAdd(KeyCode key,Action action)
    {
        KeyBinds.Add(key,action);
    }
    public void ButtonBind()
    {
        Transform buttons = transform.Find("UI").Find("Canvas").Find("Buttons");
        foreach (Transform child in buttons)
        {
                child.GetComponent<Button>().onClick.AddListener(stringFunctionToUnityAction(this,child.name));
        }
    }
    UnityAction stringFunctionToUnityAction(object target, string functionName)
    {
        UnityAction action = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), target, functionName);
        return action;
    }
    void GameStart()
    {
        if (CurStatus==GameStatus.startwaitting)
        {
            score = 0;
            CurStatus = GameStatus.playing;
            transform.Find("UI").Find("Canvas").Find("Buttons").Find("GameStart").gameObject.SetActive(false);
        }
        StartCoroutine(GamePlaying());
    }
    Queue<Note> NoteQueue;
    IEnumerator GamePlaying()
    {
        TimeSpan curtime = new TimeSpan(0,0,0);
        foreach(var data in GameObject.Find("Data").GetComponent<Data>().NoteLoadAll())
        {
            NoteQueue.Enqueue(data);
        }
        WaitForSeconds time = new WaitForSeconds(0.1f);
        
        while (NoteQueue.Count!=0 )
        {
            if (curtime==NoteQueue.Peek().time)
            {
                StartCoroutine(NoteMoving(CreateNote(NoteQueue.Dequeue())));
            }
            curtime= curtime.Add(new TimeSpan(0, 0, 0,0,100));
            yield return time;         
        }
    }
    void GameEnd()
    {
        NoteQueue.Clear();
        if (CurStatus == GameStatus.playing)
        {
            CurStatus = GameStatus.end;
            var copy = new Dictionary<GameObject, Note>(Notes);
            foreach (var data in copy)
            {
                Notes.Remove(data.Key);
                Destroy(data.Key);
            }        
            CurStatus = GameStatus.startwaitting;
            transform.Find("UI").Find("Canvas").Find("Buttons").Find("GameStart").gameObject.SetActive(true);
        }
      
    }
    void Start()
    {
        CurStatus = GameStatus.None;
        UIDefaultSetting();
        GameObject.Find("Data").GetComponent<Data>().SetPath(Resources.Load("ParkMyungGue\\XML/testbase"));        
    }


}
public partial class MainGame_s : MonoBehaviour, IUI // cube
{
    Queue<KeyCode> InputQueue;
    void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (var dic in KeyBinds)
            {
                if (Input.GetKey(dic.Key) && !KeyPressCheck.Contains(dic.Key))
                {

                    if (KeyPressCheck.Count != 0)
                    {
                        //
                    }
                    InputQueue.Enqueue(dic.Key);
                    KeyPressCheck.Add(dic.Key);
                    CenterCheck = false;
                }
            }
        }
        if (!CenterCheck)
            PressCheck();
    }
    bool CenterCheck;
    IEnumerator previousCoroutine;
    void RotateCube(Vector3 rotateposition, Cube_Note_Status setstatus)
    {
        if (previousCoroutine != RotateTimeLock(rotateposition, GameCube, setstatus))
        {

            if (IsMoving)
            {
                StopCoroutine(previousCoroutine);
                if (rotateposition != Vector3.zero)
                    GameCube.transform.localEulerAngles = new Vector3(0, 0, 0);
            }

            previousCoroutine = RotateTimeLock(rotateposition, GameCube, setstatus);
            StartCoroutine(previousCoroutine);
        }
    }
    IEnumerator RotateTimeLock(Vector3 rotateposition, GameObject targetobj, Cube_Note_Status setstatus)
    {
        IsMoving = true;
        CurCubeStatus = Cube_Note_Status.NONE;
        Vector3 rotatepos = new Vector3((rotateposition.x - targetobj.transform.eulerAngles.x) / 100, (rotateposition.y - targetobj.transform.eulerAngles.y) / 100, (rotateposition.z - targetobj.transform.eulerAngles.z) / 100);
        if ((targetobj.transform.rotation.w < 0 && KeyPressCheck.Count == 0) || targetobj.transform.rotation.x < 0 || targetobj.transform.rotation.y < 0 || targetobj.transform.rotation.z < 0)
        {
            rotatepos = new Vector3(targetobj.transform.eulerAngles.x > 0 ? (360 - rotateposition.x - targetobj.transform.eulerAngles.x) / 100 : rotateposition.x - targetobj.transform.eulerAngles.x,
                targetobj.transform.eulerAngles.y > 0 ? (360 - rotateposition.y - targetobj.transform.eulerAngles.y) / 100 : rotateposition.y - targetobj.transform.eulerAngles.y,
                targetobj.transform.eulerAngles.z > 0 ? (360 - rotateposition.z - targetobj.transform.eulerAngles.z) / 100 : rotateposition.z - targetobj.transform.eulerAngles.z
                );

        }
        for (int i = 0; i < 100; i++)
        {
            targetobj.transform.Rotate(rotatepos);
            yield return Wait;
        }
        targetobj.transform.localEulerAngles = new Vector3(MathF.Round(targetobj.transform.localEulerAngles.x), MathF.Round(targetobj.transform.localEulerAngles.y), MathF.Round(targetobj.transform.localEulerAngles.z));
        IsMoving = false;
        CurCubeStatus = setstatus;
    }
    void PressCheck()
    {
        var checktemp = new List<KeyCode>(KeyPressCheck);
        foreach (var keylist in checktemp)
        {
            if (!Input.GetKey(keylist))
            {
                KeyPressCheck.Remove(keylist);
            }
        }
        if (KeyPressCheck.Count == 0)
        {
            RotateCube(Vector3.zero, Cube_Note_Status.DEFAULT);
            CenterCheck = true;
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
            target.transform.localPosition = Vector2.Lerp(target.transform.localPosition, movingposition, 0.0025f);
            if ((target.transform.localPosition.x * previouspos.x < 0 || target.transform.localPosition.x == 0) && (target.transform.localPosition.y * previouspos.y < 0 || target.transform.localPosition.y == 0) && Notes.ContainsKey(target))
            {
                Notes[target].passCenter = true;
            }
            yield return NoteWait;
            if (target == null)
                break;
        }
        if (target != null)
            NoteEnd(target);
    }
    public float perfect = 0.3f;
    public float good = 0.5f;
    void NoteEnd(GameObject target)
    {
        if (CurCubeStatus == Notes[target].type)
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