using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Animation
{
    public static float ShowCharacterAnimation(List<string> animationNames, GameObject characterImageObj)
    {
        /*float animationTime = 0f;
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
        return animationTime;*/
        return 0;
    }
}
