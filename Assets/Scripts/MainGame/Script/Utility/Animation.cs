using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public static class Animation 
{
    public static float ShowCharacterAnimation(List<string> animationNames, SkeletonAnimation characterSkeleton)
    {
        float animationTime = 0f;
        foreach (var data in animationNames)
        {
            if (animationNames.IndexOf(data) == 0)
            {
                characterSkeleton.AnimationState.SetAnimation(0, data, false);
            }
            else if(animationNames.IndexOf(data)==animationNames.Count-1)
            {
                characterSkeleton.AnimationState.AddAnimation(0, data, true, 0f);
            }
            else
            {
                characterSkeleton.AnimationState.AddAnimation(0, data, false, 0f);
            }
            animationTime += characterSkeleton.Skeleton.Data.FindAnimation(data).Duration;
        }
        return animationTime;
    }
}
