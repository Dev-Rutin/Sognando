using Spine.Unity;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoremiUI_s : Singleton<DoremiUI_s>
{
    [SerializeField] private SkeletonAnimation _animation;
    [SerializeField] private TextMeshProUGUI _doremiText;
    private void Start()
    {
        InGameFunBind_s.Instance.Epause += Pause;
        InGameFunBind_s.Instance.EunPause +=UnPause;
    }
    public void DoremiTextChange(string data)
    {
        _doremiText.text = data;
        if (data == "")
        {
            _doremiText.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            _doremiText.transform.parent.gameObject.SetActive(true);
        }
    }
    public void SingleDoremiAnimation(string name,bool isLoop)
    {
        _animation.AnimationState.SetAnimation(0,name,isLoop);
    }
    public void MutipleDoremiAnimation(List<string> data)
    {
        Animation.ShowCharacterAnimation(data, _animation);
    }
    public void Pause()
    {
        _animation.timeScale = 0;
    }
    public void UnPause()
    {
        _animation.timeScale = 1;
    }
}
