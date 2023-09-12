using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

struct AdjacentSet
{
    public string name;
    public Vector3 pos;
    public AdjacentSet(string _name, Vector3 _pos)
    {
        name = _name;
        pos = _pos;
    }
}
enum GameStatus
{
    None,
    startwaitting,
    playing,
    end,
    pause
}
enum CubeStatus
{
    None,
    Center,
    Left,
    Up,
    Right,
    Down
}
enum NoteList
{
    Circle,
    Cross,
    Crescent,
    Square,
    Triangle,
    Hexagon
}
public class MainGame_s : MonoBehaviour ,IUI
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
    Dictionary<GameObject,NoteList> Notes;
    WaitForFixedUpdate NoteWait;
    NoteList CurImage;
    int score;
    bool IsMoving;
    CubeStatus CurCubeStatus;
    public void UIDefaultSetting()
    {
        GameCube = GameObject.Find("Cube").gameObject;
        BindSetting();
        //ButtonBind();
        CurStatus = GameStatus.startwaitting;
        Wait = new WaitForSeconds(0.1f/100f);
        Notes = new Dictionary<GameObject, NoteList>();
        NoteWait = new WaitForFixedUpdate();

        IsMoving = false;
        CurCubeStatus = CubeStatus.Center;
    }

    void BindSetting()
    {

        KeyBinds = new Dictionary<KeyCode, Action>();
        KeyPressCheck = new List<KeyCode>();
        BindAdd(KeyCode.LeftArrow, () => RotateCube(new Vector3(0, -90, 0),CubeStatus.Left));
        BindAdd(KeyCode.RightArrow, () => RotateCube(new Vector3(0, 90, 0), CubeStatus.Right));
        BindAdd(KeyCode.UpArrow, () => RotateCube(new Vector3(-90, 0, 0), CubeStatus.Up));
        BindAdd(KeyCode.DownArrow, () => RotateCube(new Vector3(90, 0, 0), CubeStatus.Down));
       
    }
    void BindAdd(KeyCode key,Action action)
    {
        KeyBinds.Add(key,action);
    }
    public void ButtonBind()
    {
        Transform buttons = transform.Find("Buttons");
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
    IEnumerator previousCoroutine;
    void RotateCube(Vector3 rotateposition,CubeStatus setstatus)
    {
        if (previousCoroutine!= RotateTimeLock(rotateposition, GameCube, setstatus))
        {
            
            if (IsMoving)
            {
                StopCoroutine(previousCoroutine);
                if (rotateposition != Vector3.zero)
                    GameCube.transform.localEulerAngles = new Vector3(0, 0, 0);
            }

            previousCoroutine = RotateTimeLock(rotateposition, GameCube,setstatus);
            StartCoroutine(previousCoroutine);
        }
    }
    IEnumerator RotateTimeLock(Vector3 rotateposition, GameObject targetobj,CubeStatus setstatus)
    {
        IsMoving = true;
        CurCubeStatus = CubeStatus.None;    
        Vector3 rotatepos = new Vector3((rotateposition.x - targetobj.transform.eulerAngles.x) / 100, (rotateposition.y - targetobj.transform.eulerAngles.y) / 100, (rotateposition.z - targetobj.transform.eulerAngles.z) / 100);
        if((targetobj.transform.rotation.w<0&&KeyPressCheck.Count==0)||targetobj.transform.rotation.x<0||targetobj.transform.rotation.y<0||targetobj.transform.rotation.z<0)
        {
            rotatepos = new Vector3(targetobj.transform.eulerAngles.x > 0 ? (360-rotateposition.x - targetobj.transform.eulerAngles.x) / 100:rotateposition.x-targetobj.transform.eulerAngles.x,
                targetobj.transform.eulerAngles.y > 0 ? (360 - rotateposition.y - targetobj.transform.eulerAngles.y) / 100 : rotateposition.y - targetobj.transform.eulerAngles.y,
                targetobj.transform.eulerAngles.z > 0 ? (360 - rotateposition.z - targetobj.transform.eulerAngles.z) / 100 : rotateposition.z - targetobj.transform.eulerAngles.z
                );
            
        }
        Debug.Log(targetobj.transform.rotation);
        Debug.Log(rotateposition+"and"+targetobj.transform.eulerAngles);
        for (int i = 0; i < 100; i++)
        {
            targetobj.transform.Rotate(rotatepos);
            yield return Wait;
        }
        targetobj.transform.localEulerAngles = new Vector3(MathF.Round(targetobj.transform.localEulerAngles.x), MathF.Round(targetobj.transform.localEulerAngles.y), MathF.Round(targetobj.transform.localEulerAngles.z));
        IsMoving = false;
        CurCubeStatus = setstatus;
        Debug.Log("moving ENd");
    }

    void GameStart()
    {
        if(CurStatus==GameStatus.startwaitting)
        {
            score = 0;
            CurStatus = GameStatus.playing;
            StartCoroutine(GameTest());
            transform.Find("Buttons").Find("GameStart").gameObject.SetActive(false);
        }
    }
    void GameEnd()
    {
        if (CurStatus == GameStatus.playing)
        {
            CurStatus = GameStatus.end;
            var copy = new Dictionary<GameObject, NoteList>(Notes);
            foreach (var data in copy)
            {
                Notes.Remove(data.Key);
                NoteEnd(data.Key);
            }
            StopCoroutine(GameTest());
            CurStatus = GameStatus.startwaitting;
            transform.Find("Buttons").Find("GameStart").gameObject.SetActive(true);
        }
      
    }
    void Start()
    {
        CurStatus = GameStatus.None;
        UIDefaultSetting();
        
    }
    void Update()
    { 
        if (Input.anyKeyDown)
        {
            foreach (var dic in KeyBinds)
            {
                if (Input.GetKey(dic.Key)&&!KeyPressCheck.Contains(dic.Key))
                {
                 
                        if (KeyPressCheck.Count!=0)
                        {
                            GameCube.transform.localEulerAngles = new Vector3(0, 0, 0);
                        }
                        KeyBinds[dic.Key]();
                    KeyPressCheck.Add(dic.Key);
                    CenterCheck = false;
                }
            }
        }
        if(!CenterCheck)
         PressCheck();      
    }
    bool CenterCheck;
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
        if(KeyPressCheck.Count==0)
        {
            RotateCube(Vector3.zero,CubeStatus.Center);
            CenterCheck = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
   {
        Debug.Log(Notes[collision.gameObject].ToString());
        NoteEnd(collision.gameObject);
   }
    IEnumerator GameTest()
    {
        var nodetime = new WaitForSeconds(1f);
        while (CurStatus == GameStatus.playing)
        {
            StartCoroutine(NoteMoving(CreateNote((NoteList)UnityEngine.Random.Range(0, 6))));
            yield return nodetime;
        }
    }
    GameObject CreateNote(NoteList list)
    {
        GameObject instnote = Instantiate(transform.Find("Notes").Find("Note").gameObject, transform.Find("Notes"));
        instnote.transform.localPosition = new Vector3(UnityEngine.Random.Range(0, 2) == 1 ? transform.GetComponent<RectTransform>().rect.width / 2 : transform.GetComponent<RectTransform>().rect.width / 2 * -1, UnityEngine.Random.Range(0, 2) == 1 ? transform.GetComponent<RectTransform>().rect.height / 2 : transform.GetComponent<RectTransform>().rect.height / 2 * -1, 0);
        foreach (var sprite in Resources.LoadAll<Sprite>("NoteImage"))
        {
            if (sprite.name == list.ToString())
            {
                instnote.GetComponent<SpriteRenderer>().sprite = sprite;
            }
        }
        Notes.Add(instnote, list);
        return instnote;
    }
    IEnumerator NoteMoving(GameObject target)
    {
        while (Vector3.Distance(target.transform.localPosition, Vector3.zero) > 25)
        {
            target.transform.localPosition = Vector3.Lerp(target.transform.localPosition, Vector3.zero, 0.02f);
            target.transform.localPosition = new Vector3(target.transform.localPosition.x, target.transform.localPosition.y, 0f);
            yield return NoteWait;
            if (target == null)
                break;
        }
        if (target != null)
            NoteEnd(target);
    }
    void NoteEnd(GameObject target)
    {
        if (CurStatus != GameStatus.end)
        {
            if (CurImage == Notes[target])
            {
                score++;
                transform.Find("Score").GetComponent<TextMeshProUGUI>().text = "Score : " + score.ToString();
            }
        }
        Notes.Remove(target);
        Destroy(target);
    }
}
