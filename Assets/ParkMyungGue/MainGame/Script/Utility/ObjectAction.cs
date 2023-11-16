using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class ObjectAction
{
    static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    public static IEnumerator MovingObj(GameObject target, Vector2 targetPos, float movingTime)
    {
        float lerpValue = 0;
        float startTime = InGameMusicManager_s.Instance.musicPosition;
        Vector2 startPos = target.transform.localPosition;
        while (lerpValue <= 1)
        {
            lerpValue = (InGameMusicManager_s.Instance.musicPosition - startTime) * 1 / movingTime;
            target.transform.localPosition = Vector2.Lerp(startPos, targetPos, lerpValue);
            yield return waitForEndOfFrame;
        }
        target.transform.localPosition = targetPos;
    }
    public static IEnumerator ImageFade(Image target, float time,bool isAllFade, float startValue, float endValue = 0)
    {
        float lerpValue = 0;
        float startTime = InGameMusicManager_s.Instance.musicPosition;
        while (lerpValue <= 1)
        {
            lerpValue = (InGameMusicManager_s.Instance.musicPosition - startTime) * 1 /time;
            if (isAllFade)
            {
                ImageAlphaChange(target, Mathf.Lerp(startValue, endValue, lerpValue));
            }
            else
            {
                if (lerpValue < time / 2)
                {
                    ImageAlphaChange(target, Mathf.Lerp(startValue, 0.5f, 0.5f/lerpValue));
                }
                else
                {
                    ImageAlphaChange(target, Mathf.Lerp(0.5f, startValue, 0.5f/lerpValue-1));
                }
            }
            yield return waitForEndOfFrame;
        }
    }
    public static IEnumerator ImageFade(SpriteRenderer target, float time, bool isAllFade,float startValue, float endValue = 0)
    {
        float lerpValue = 0;
        float startTime = InGameMusicManager_s.Instance.musicPosition;
        while (lerpValue <= 1)
        {
            lerpValue = (InGameMusicManager_s.Instance.musicPosition - startTime) * 1 / time;
            if (isAllFade)
            {
                ImageAlphaChange(target, Mathf.Lerp(startValue, endValue, lerpValue));
            }
            else
            {
                if (lerpValue < time / 2)
                {
                    ImageAlphaChange(target, Mathf.Lerp(startValue, 0.5f, 0.5f / lerpValue));
                }
                else
                {
                    ImageAlphaChange(target, Mathf.Lerp(0.5f, startValue, 0.5f / lerpValue - 1));
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
    public static IEnumerator ObjectScalePump(Transform target, Vector3 targetScale, float time)
    {
        float lerpValue = 0;
        float startTime = InGameMusicManager_s.Instance.musicPosition;
        while (lerpValue<=1)
        {
            lerpValue = (InGameMusicManager_s.Instance.musicPosition - startTime) * 1 / time;
            target.localScale = Vector3.Lerp(Vector3.one, targetScale, lerpValue);
            yield return waitForEndOfFrame;
        }
        target.localScale = Vector3.one;
    }
}