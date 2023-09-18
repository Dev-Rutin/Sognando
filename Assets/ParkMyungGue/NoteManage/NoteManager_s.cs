using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public struct NoteObjTime
{
    public Note curnote;
    public Vector2 curposition;

}
public class NoteManager_s : MonoBehaviour
{
    Dictionary<AudioClip, Data> musicDic;
    public MainGame_s maingame_s;
    public Camera cam;
    public Dictionary<TimeSpan, List<NoteObjTime>> timeline;
    void Start()
    {
        cam = Camera.main;
        musicDic = new Dictionary<AudioClip, Data>();
        foreach(var data in Resources.LoadAll<AudioClip>("ParkMyungGue\\Music"))
        {
            Debug.Log(data.name);
           if(Resources.Load("ParkMyungGue\\XML\\"+data.name)!=null)
            {
                Data notefile = new Data();
                notefile.SetPath(Resources.Load("ParkMyungGue\\XML\\" + data.name));
                musicDic.Add(data, notefile);
            }
        }
        TMP_Dropdown musicList = transform.Find("SelectMusic").Find("MusicList").GetComponent<TMP_Dropdown>();
        List<string> dropoption = new List<string>();
        foreach (var data in musicDic)
        {
            dropoption.Add(data.Key.name);
        }
        musicList.AddOptions(dropoption);
   
        maingame_s.CreateNoteEvent += CreateNote;
        maingame_s.DeleteNoteEvent += DeleteNote;
    }
    public void TestStart()
    {
        transform.Find("SelectMusic").gameObject.SetActive(false);
        transform.Find("PlayScreen").gameObject.SetActive(true);
   
    }
    public void CreateNote(Note note)
    {     
    }
    public void DeleteNote(Note note)
    {
    }
    void ShowNoteObj(Note note)
    {

    }
    private void Update()
    {
        if(maingame_s.curGameStatus==EGameStatus.PAUSE&&Input.GetMouseButtonDown(0))
        {
           RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition),transform.forward);
            if (hit)
            {
                if(hit.transform.tag=="Note")
                {
                   
                }
            }
        }
        if(maingame_s.curGameStatus==EGameStatus.PLAYING)
        {
            if(!timeline.ContainsKey(maingame_s.curMainGameTime))
            {
                timeline.Add(maingame_s.curMainGameTime, new List<NoteObjTime>());
            }
            else
            {

            }
        }
    }
}
