using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class ObjectAction
{
    public static IEnumerator MovingObj(GameObject target, Vector2 targetPos, float movingTime)
    {
        float lerpValue = 0;
        float startTime = InGameMusicManager_s.Instance.musicPosition;
        Vector2 startPos = target.transform.localPosition;
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        while (lerpValue <= 1)
        {
            lerpValue = (InGameMusicManager_s.Instance.musicPosition - startTime) * 1 / movingTime;
            target.transform.localPosition = Vector2.Lerp(startPos, targetPos, lerpValue);
            yield return waitForEndOfFrame;
        }
        target.transform.localPosition = targetPos;
    }
    public static IEnumerator ImageFade(Image target, float time, bool isAllFade)
    {
        float lerpValue = 0;
        float startTime = InGameMusicManager_s.Instance.musicPosition;
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        while (lerpValue <= 1)
        {
            lerpValue = (InGameMusicManager_s.Instance.musicPosition - startTime) * 1 /time;
            if (isAllFade)
            {
                ImageAlphaChange(target, Mathf.Lerp(1, 0, lerpValue));
            }
            else
            {
                if (lerpValue < time / 2)
                {
                    ImageAlphaChange(target, Mathf.Lerp(1, 0.5f, 0.5f/lerpValue));
                }
                else
                {
                    ImageAlphaChange(target, Mathf.Lerp(0.5f, 1, 0.5f/lerpValue-1));
                }
            }
            yield return waitForEndOfFrame;
        }
    }
    public static IEnumerator ImageFade(SpriteRenderer target, float time, bool isAllFade)
    {
        float lerpValue = 0;
        float startTime = InGameMusicManager_s.Instance.musicPosition;
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        while (lerpValue <= 1)
        {
            lerpValue = (InGameMusicManager_s.Instance.musicPosition - startTime) * 1 / time;
            if (isAllFade)
            {
                ImageAlphaChange(target, Mathf.Lerp(1, 0, lerpValue));
            }
            else
            {
                if (lerpValue < time / 2)
                {
                    ImageAlphaChange(target, Mathf.Lerp(1, 0.5f, 0.5f / lerpValue));
                }
                else
                {
                    ImageAlphaChange(target, Mathf.Lerp(0.5f, 1, 0.5f / lerpValue - 1));
                }
            }
            yield return waitForEndOfFrame;
        }
    }
    public static void ImageAlphaChange(Image target, float value)
    {
        Color targetCol = target.color;
        targetCol.a = value;
        target.color = targetCol;
    }
    public static void ImageAlphaChange(SpriteRenderer target, float value)
    {
        Color targetCol = target.color;
        targetCol.a = value;
        target.color = targetCol;
    }
}