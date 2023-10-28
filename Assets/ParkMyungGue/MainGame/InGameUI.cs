using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public static partial class InGameUI //ui
{
    public static void ShowBGMSlider(Transform sliderObj,float value)
    {
        sliderObj.GetComponent<Slider>().value = value;  
    }
    public static void ShowText(InGameData_s data_s, string message)
    {
       data_s.ShowTextTsf.GetComponent<TextMeshProUGUI>().text = message;
    }
}
public static partial class InGameUI//animation
{
    public static float ShowCharacterAnimation(List<string> animationNames,GameObject characterImageObj)
    {
        float animationTime = 0f;
        foreach (var data in animationNames)
        {
            if (animationNames.IndexOf(data) == 0)
            {
                characterImageObj.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, data, false);
            }
            else
            {
                characterImageObj.GetComponent<SkeletonAnimation>().AnimationState.AddAnimation(0, data, false, 0f);
            }
            animationTime += characterImageObj.GetComponent<SkeletonAnimation>().Skeleton.Data.FindAnimation(data).Duration;
        }
        characterImageObj.GetComponent<SkeletonAnimation>().AnimationState.AddAnimation(0, "idle", true, 0f);
        return animationTime;
    }
}