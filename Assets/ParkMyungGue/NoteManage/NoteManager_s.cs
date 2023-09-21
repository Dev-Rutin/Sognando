using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public struct NoteObjTime
{
    public NoteObjTime(Note _note, Vector2 _position)
    {
        curnote = _note;
        curposition = _position;
    }
    public Note curnote;
    public Vector2 curposition;

}
public class NoteManager_s : MonoBehaviour
{
    Dictionary<AudioClip, Data> musicDic;
    public MainGame_s maingame_s;
    public Camera cam;
    public Dictionary<TimeSpan, List<NoteObjTime>> timeLineDic;
    public Transform notesTsf;
    public Transform timeLineTsf;
    public Transform noteStatusTsf;
    private Note _curSelectedNote;
    List<GameObject> statusObjList;
    void Start()
    {
        statusObjList = new List<GameObject>();
        transform.Find("InGame").gameObject.SetActive(false);
        GameObject.Find("MainGame").SetActive(false);
        timeLineDic = new Dictionary<TimeSpan, List<NoteObjTime>>();
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
        transform.Find("SelectMusic").gameObject.SetActive(true);

        /*_curSelectedNote = new Note();
        _curSelectedNote.time = new TimeSpan(0, 0, 1);
        _curSelectedNote.time2 = new TimeSpan(10, 10, 10);
        foreach (var data in typeof(Note).GetFields().ToList())
        {
            //if(data.FieldType.IsEnum)
            //Debug.Log((int)data.GetValue(_curSelectedNote));
        }*/
    }
    public void TestStart()
    {
        maingame_s.UIOpen();
        maingame_s.MusicSetting("Test160");
        TimeLineSetting();
        transform.Find("SelectMusic").gameObject.SetActive(false);
        transform.Find("InGame").Find("TimeLine").gameObject.SetActive(true);
        transform.Find("InGame").Find("NoteStatus").gameObject.SetActive(false);
        transform.Find("InGame").gameObject.SetActive(true);
        maingame_s.gameObject.SetActive(true);
    
    }
    void TimeLineSetting()
    {
        timeLineTsf.GetComponent<Slider>().minValue = 0;
        timeLineTsf.GetComponent<Slider>().maxValue = maingame_s.musicClip.length;
        timeLineTsf.GetComponent<Slider>().value = 0;
    }
    public void CreateNote(Note note)
    {     
    }
    public void DeleteNote(Note note)
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
                    _curSelectedNote = maingame_s.GameObjectToNoteOrNull(hit.transform.gameObject);
                    if (_curSelectedNote != null)
                    {
                        ShowNoteObj();
                    }
                }
            }
        }
        if(maingame_s.curGameStatus==EGameStatus.PLAYING)
        {
            if(!timeLineDic.ContainsKey(maingame_s.curMainGameTime))
            {
                timeLineDic.Add(maingame_s.curMainGameTime, new List<NoteObjTime>());
            }
            if(notesTsf.childCount!=0)
            {
                timeLineDic[maingame_s.curMainGameTime].Clear();
                foreach (Transform data in notesTsf)
                {
                    timeLineDic[maingame_s.curMainGameTime].Add(new NoteObjTime(maingame_s.GameObjectToNoteOrNull(data.gameObject), data.transform.localPosition));
                }
                //Debug.Log(timeline[maingame_s.curMainGameTime].Count);
            }

        }
    }
    void ShowNoteObj()
    {
        if (noteStatusTsf.gameObject.activeSelf == false)
        {
            foreach (var data in typeof(Note).GetFields().ToList())
            {
                GameObject instData = Instantiate(Resources.Load<GameObject>("Prefabs\\ParkMyungGue\\Canvas\\NoteStatusSample"), noteStatusTsf.Find("StatusList"));
                NoteDataAutoShow(data, instData.transform);
                instData.transform.localPosition = new Vector2(0, 230 - ((noteStatusTsf.Find("StatusList").childCount-1) * 50));
                statusObjList.Add(instData);
                instData.SetActive(true);
            }
            noteStatusTsf.gameObject.SetActive(true);
        }
    }
    void NoteDataAutoShow(FieldInfo info,Transform sampleTsf)
    {
        sampleTsf.Find("SampleName").gameObject.ChangeText(info.Name);
        sampleTsf.Find("SampleName").gameObject.SetActive(true);
        if (info.FieldType == typeof(TimeSpan))
        {
            sampleTsf.Find("SampleInputfield").gameObject.GetComponent<TMP_InputField>().text=((TimeSpan)info.GetValue(_curSelectedNote)).ToString();
            sampleTsf.Find("SampleInputfield").gameObject.SetActive(true);
        }
        if (info.FieldType == typeof(string))
        {
            sampleTsf.Find("SampleInputfield").GetComponent<TMP_InputField>().text = info.GetValue(_curSelectedNote).ToString();
            sampleTsf.Find("SampleInputfield").gameObject.SetActive(true);
        }
        if (info.FieldType.IsEnum)
        {
            List<TMP_Dropdown.OptionData> optionlist = new List<TMP_Dropdown.OptionData>();
            foreach(var data in Enum.GetNames(info.FieldType))
            {
                TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
                newData.text = data.ToString();
                optionlist.Add(newData);
            }
            sampleTsf.Find("SampleDropdown").GetComponent<TMP_Dropdown>().AddOptions(optionlist);
            sampleTsf.Find("SampleDropdown").GetComponent<TMP_Dropdown>().value = (int)info.GetValue(_curSelectedNote);
            sampleTsf.Find("SampleDropdown").gameObject.SetActive(true);
        }
        if (info.FieldType == typeof(Vector2))
        {
            sampleTsf.Find("SampleInputfield").GetComponent<TMP_InputField>().text = ((Vector2)info.GetValue(_curSelectedNote)).ToString();  
            sampleTsf.Find("SampleInputfield").gameObject.SetActive(true);
        }
    }
    public void UnShowNoteObj()
    {
        Note temp = new Note();
        foreach (Transform notestatus in noteStatusTsf.Find("StatusList"))
        {
            string fieldname=notestatus.Find("SampleName").GetComponent<TextMeshProUGUI>().text;
            List<string> datas = new List<string>();
            foreach(Transform notedata in notestatus)
            {
                if(notedata.gameObject.activeSelf)
                {
                    switch(notedata.name)
                    {
                        case "SampleInputfield":
                            datas.Add(notedata.GetComponent<TMP_InputField>().text);
                            break;
                        case "SampleInputfield2":
                            datas.Add(notedata.GetComponent<TMP_InputField>().text);
                            break;
                        case "SampleDropdown":
                            datas.Add(notedata.GetComponent<TMP_Dropdown>().options[notedata.GetComponent<TMP_Dropdown>().value].text);
                            break;
                        default: break;
                    }
                }
            }
            Data.StringConverter(ref temp, typeof(Note).GetField(fieldname), datas);
        }
        maingame_s.musicData.ChangeNoteData(_curSelectedNote, temp);
        for (int i = 0; i < noteStatusTsf.Find("StatusList").childCount; i++)
        {
            Destroy(noteStatusTsf.Find("StatusList").GetChild(i).gameObject);
        }
        noteStatusTsf.gameObject.SetActive(false);
    }
}
public static class ExClass
{
    public static void ChangeText(this GameObject obj,string data)
    {
 
        obj.GetComponent<TextMeshProUGUI>().text = data;
      
    }
}
