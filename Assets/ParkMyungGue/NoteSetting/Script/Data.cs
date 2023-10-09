using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;
public enum ENoteType
{
    DEFAULT,
    LONG
}
public enum ENoteImage
{
    DEFAULT,
    UP,
    DOWN,
    LEFT,
    RIGHT
}
public class Note
{
    //none autoload
    public TimeSpan time;
    public TimeSpan time2;
    //autoload
    public ENoteImage image;
    public ENoteType type;
    public Vector2 position;
    //not load
    public bool isPassCenter;

    public Note()
    {

    }
    public Note(Note note)
    {
        time = note.time;
        time2= note.time2;
        image = note.image;
        type = note.type;
        position = note.position;
    }
    public Note DeepCopy()
    {
        Note rnote = new Note();
        rnote.time= time;
        rnote.time2= time2;
        rnote.image= image;
        rnote.type= type;   
        rnote.position= position;   
        return rnote;  
    }
    public static bool operator==(Note a, Note b)
    {
        if(a is null&& b is null)
        {
            return true;
        }
        if(a is null || b is null)
        {
            return false;
        }
        bool isSameCheck = true;

            isSameCheck &= a.time == b.time;
            isSameCheck &= a.time2 == b.time2;
        isSameCheck &= a.image == b.image;
        isSameCheck &= a.type == b.type;
        return isSameCheck;
    }
    public static bool operator !=(Note a, Note b)
    {  
        if (a == b)
        { 
            return false;
        }
        else
        {
            return true;
        }

    }
    public override bool Equals(object o)
    {
        if(o is Note)
        {   
            if(this==(Note)o)
            {
                return true;
            }
        }
        return false;
    }
    public override int GetHashCode()
    {
        return time.Milliseconds;
    }
}
public class Data
{
    public XmlDocument xdocPath;
    object pathstring;
    public Data() { }
    public Data(object path)
    {
        SetPath(path);
    }
    public void SetPath(object path)
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
    public void SetPath(TextAsset path)
    {
        xdocPath.LoadXml(path.text);
        pathstring = path;
    }
    public void SetPath(string path)
    {
        xdocPath.Load(path);
        pathstring = path;
    }
    public void SetPath(XmlReader reader)
    {
        xdocPath.Load(reader);
        pathstring = reader;
    }
    public static void StringConverter(ref Note note,FieldInfo p,List<string>val)
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
                p.SetValue(note,new Vector2(float.Parse(val[0].Split(',')[0].Split('(')[1]),
                    float.Parse(val[0].Split(',')[1].Split(')')[0])));
            }
            /*if (p.FieldType == typeof(Vector3))
            {
                p.SetValue(note,new Vector3(float.Parse(val[0]), float.Parse(val[1]), float.Parse(val[2])));
            }*/
            if (p.FieldType == typeof(TimeSpan))
            {
                p.SetValue(note, TimeSpan.Parse(val[0]));
            }
        }
    }
    public Note GetNote(XmlNode notetime)
    {
        Note rValue = new Note();
        rValue.time = TimeSpan.Parse(notetime.Attributes["value"].Value);
        foreach (XmlNode notedata in notetime.ChildNodes)
        {
            var noteVariable = rValue.GetType().GetFields().ToList();
            foreach (var data in noteVariable)
            {
                if (notedata.Name == data.Name)
                {
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
        if(rValue.type==ENoteType.LONG)
        {
            rValue.time2 = TimeSpan.Parse(notetime.Attributes["value2"].Value);
        }
        return rValue;
    }
    public List<Note> LoadNote(TimeSpan time, object path = null)
    {
        if(path!=null)
            SetPath(path);
        return LoadNote(time);
    }
 
    public List<Note> LoadNote(TimeSpan time)
    {
        
        var rList = new List<Note>();
        XmlNodeList nodes = xdocPath.SelectNodes("/Note/text/time");
        foreach(XmlNode notetime in nodes)
        {
            if (time == TimeSpan.Parse(notetime.Attributes["value"].Value))
            {
                rList.Add(GetNote(notetime));
            }          
        }
            return rList;
    }
    public List<Note> LoadNoteAll(object path =null)
    {
        if (path != null)
            SetPath(path);
        var rList = new List<Note>();
        XmlNodeList nodes = xdocPath.SelectNodes("/Note/text/time");
        foreach (XmlNode notetime in nodes)
        {
            rList.Add(GetNote(notetime));
        }
            return rList;
    }
    public void ChangeNoteData(Note basenote,Note target)
    {
        XmlNodeList nodes = xdocPath.SelectNodes("/Note/text/time");
        Debug.Log(target.time.ToString());
        foreach (XmlNode notetime in nodes)
        {
            if(basenote==GetNote(notetime))
            {
                notetime.Attributes["value"].Value = target.time.ToString();
                notetime.Attributes["value2"].Value = target.time2.ToString();
                foreach (XmlNode notedata in notetime.ChildNodes)
                {
                    foreach (var data in typeof(Note).GetFields().ToList())
                    {
                        if (notedata.Name == data.Name)
                        {
                            if (notedata.ChildNodes.Count == 0)
                            {
                                notedata.InnerText = data.GetValue(target).ToString();
                            }
                            else
                            {
                                foreach (XmlNode childvalue in notedata.ChildNodes)
                                {
                                    childvalue.InnerText =(data.GetValue(target)).ToString();
                                }
                            }                        
                        }
                    }
                }
            }
        }
        //Debug.Log(AssetDatabase.GetAssetPath((TextAsset)pathstring));
        //xdocPath.Save(AssetDatabase.GetAssetPath((TextAsset)pathstring));

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
