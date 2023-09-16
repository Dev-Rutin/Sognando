using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class NoteManager_s : MonoBehaviour
{
    Dictionary<AudioClip, Data> musicDic;
    void Start()
    {
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
    }
    public void TestStart()
    {
        transform.Find("SelectMusic").gameObject.SetActive(false);
        transform.Find("PlayScreen").gameObject.SetActive(true);
        NoteWait = new WaitForSeconds(0.1f);
    }
    Dictionary<GameObject, Note> Notes;
    GameObject CreateNote(Note note)
    {
        note.isPassCenter = false;
        GameObject instnote = Instantiate(Resources.Load<GameObject>("Prefabs\\ParkMyungGue\\Note"), transform.Find("UI").Find("Canvas").Find("Notes"));
        instnote.transform.localPosition = note.position;
        instnote.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("ParkMyungGue\\NoteImage\\" + note.image.ToString());
        Notes.Add(instnote, note);
        instnote.SetActive(true);
        return instnote;
    }
    WaitForSeconds NoteWait;
   IEnumerator NoteMoving(GameObject target)
    {
        Vector2 previouspos = target.transform.position;
        Vector2 movingposition = new Vector2(target.transform.localPosition.x * -1, target.transform.localPosition.y * -1);

        while (target != null && Vector2.Distance(target.transform.localPosition, movingposition) > 25)
        {
            target.transform.localPosition = Vector2.Lerp(target.transform.localPosition, movingposition, 0.0025f);
            if ((target.transform.localPosition.x * previouspos.x < 0 || target.transform.localPosition.x == 0) && (target.transform.localPosition.y * previouspos.y < 0 || target.transform.localPosition.y == 0) && Notes.ContainsKey(target))
            {
                Notes[target].isPassCenter = true;
            }
            yield return NoteWait;
            if (target == null)
                break;
        }
        if (target != null)
        { }
            //  NoteEnd(target);
    }
    void Update()
    {
        
    }
}
