using Spine.Unity;
using TMPro;
using UnityEngine;

public class DoremiUI_s : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation _animation;
    [SerializeField] private TextMeshProUGUI _doremiText;
    public void DoremiTextChange(string data)
    {
        _doremiText.text = data;
    }
    public void DoremiAnimation(string name)
    {
        _animation.AnimationState.SetAnimation(0,name,false);
    }
}
