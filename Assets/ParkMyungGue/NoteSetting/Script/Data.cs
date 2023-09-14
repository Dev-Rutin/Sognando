using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
public class Note
{
    public TimeSpan time;
    public Cube_Note_Status type;
    public Vector2 position;
    public bool passCenter;
}
public class Data
{
    public XmlDocument xdocPath;
    public Data() { }
    public Data(object path)
    {
        SetPath(path);
    }
    public void SetPath(object path)
    {
        try
        {
            if (xdocPath == null)
            {
                xdocPath = new XmlDocument();
            }
            if (path.GetType() == typeof(string))
            {
                SetPath((string)path);
            }
            else if (path.GetType() == typeof(TextAsset))
            {
                SetPath((TextAsset)path);
            }
            else if (path.GetType() == typeof(XmlReader))
            {
                SetPath((XmlReader)path);
            }
        }
        catch
        {
            xdocPath = null;
        }
    }
    public void SetPath(TextAsset path)
    {
        xdocPath.LoadXml(path.text);

    }
    public void SetPath(string path)
    {
        xdocPath.Load(path);
        
    }
    public void SetPath(XmlReader reader)
    {
        xdocPath.Load(reader);
    }
    public void StringConverter(ref Note note,FieldInfo p,List<string>val)
    {
        if (val.Count > 0 && val.Count <= 3)
        {
            if (p.FieldType == typeof(string))
            {
                p.SetValue(note,val[0]);
            }
            if (p.FieldType.IsEnum)
            {
                p.SetValue(note, Enum.Parse(p.FieldType, val[0]));
            }
            if (p.FieldType == typeof(Vector2))
            {               
                p.SetValue(note, new Vector2(float.Parse(val[0]), float.Parse(val[1])));
            }
            if (p.FieldType == typeof(Vector3))
            {
                p.SetValue(note,new Vector3(float.Parse(val[0]), float.Parse(val[1]), float.Parse(val[2])));
            }
            /*if (p.FieldType == typeof(TimeSpan))
            {
                p.SetValue(note, TimeSpan.Parse(val[0]));
            }*/
        }
    }
    public Note GetNote(XmlNode notetime,string time)
    {
        Note rValue = new Note();
        rValue.time = TimeSpan.Parse(time);
        foreach (XmlNode notedata in notetime.ChildNodes)
        {
            var noteVariable = rValue.GetType().GetFields().ToList();
            foreach (var data in noteVariable)
            {
                if (notedata.Name == data.Name)
                {
                    object point = data.GetValue(rValue);
                    List<string> sendvalues = new List<string>();
                    if (notedata.ChildNodes.Count == 0)
                    {
                        sendvalues.Add(notedata.InnerText);
                    }
                    else
                    {
                        foreach (XmlNode childvalue in notedata.ChildNodes)
                        {

                            sendvalues.Add(childvalue.InnerText);
                        }
                    }
                    StringConverter(ref rValue, data, sendvalues);
                }
            }
        }
        return rValue;
    }
    public List<Note> NoteLoad(TimeSpan time, object path = null)
    {
        if(path!=null)
            SetPath(path);
        return NoteLoad(time);
    }
 
    public List<Note> NoteLoad(TimeSpan time)
    {
        
        var rList = new List<Note>();
        XmlNodeList nodes = xdocPath.SelectNodes("/Note/text/time");
        foreach(XmlNode notetime in nodes)
        {
            if (time == TimeSpan.Parse(notetime.Attributes["value"].Value))
            {
                rList.Add(GetNote(notetime, notetime.Attributes["value"].Value));
            }          
        }
        if (rList.Count == 0)
            return null;
        else
            return rList;
    }
    public List<Note> NoteLoadAll(object path =null)
    {
        if (path != null)
            SetPath(path);
        var rList = new List<Note>();
        XmlNodeList nodes = xdocPath.SelectNodes("/Note/text/time");
        foreach (XmlNode notetime in nodes)
        {
            rList.Add(GetNote(notetime, notetime.Attributes["value"].Value));
        }
        if (rList.Count == 0)
            return null;
        else
            return rList;
    }
 
    /*private void Start()
    {
        /*foreach (var data in typeof(Note).GetFields().ToList())
        {
            //Debug.Log(data.FieldType);
           
        }*/
        /*string tests = "Circle";
        NoteType type = (NoteType)Enum.Parse(typeof(NoteType), tests);
        Debug.Log(type);
        TimeSpan timetest = new TimeSpan(0, 1, 0);
        Debug.Log(timetest);*/
        //SetPath(Resources.Load("XML/testbase"));
        //SetPath("C:\\Users\\ricos\\Desktop\\작업\\게임\\유니티\\CK\\CalibrationSample\\Assets\\Resources\\XML\\testbase.xml");
        //List<Note>test1= NoteLoad(new TimeSpan(11,11,11));
        //Debug.Log(test1[0].time + "/" + test1[0].type + "/" + test1[0].position);
        //Debug.Log(test1[1].time + "/" + test1[1].type+"/" + test1[1].position);
        //List<Note> noteall = NoteLoadAll();
        /*foreach(var data in noteall)
        {
            Debug.Log(data.time + "/" +data.type + "/" + data.position);
        }

    }*/
}
