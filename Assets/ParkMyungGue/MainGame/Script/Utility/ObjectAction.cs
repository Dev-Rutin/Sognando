using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class ObjectAction
{
    static WaitForEndOfFrame waitUpdate = new WaitForEndOfFrame();
    public static IEnumerator MovingObj(GameObject target, Vector2 targetPos, float movingTime)
    {
        float lerpValue = 0;
        float startTime = InGameMusicManager_s.Instance.musicPosition;
        Vector2 startPos = target.transform.localPosition;
        while (lerpValue <= 1)
        {
            lerpValue = (InGameMusicManager_s.Instance.musicPosition - startTime) * 1 / movingTime;
            target.transform.localPosition = Vector2.Lerp(startPos, targetPos, lerpValue);
            yield return waitUpdate;
        }
        target.transform.localPosition = targetPos;
    }
    public static IEnumerator RotateObj(GameObject target, Vector3 rotatePos, float movingTime)
    {
        float rotateIncrease = 0;
        float curRotateValue = 0;
        float previousPosition = InGameMusicManager_s.Instance.musicPosition;
        float startTime = InGameMusicManager_s.Instance.musicPosition;
        while (InGameMusicManager_s.Instance.musicPosition - startTime <= movingTime)
        {
            curRotateValue = Mathf.Abs(rotatePos.x + rotatePos.y + rotatePos.z) * (InGameMusicManager_s.Instance.musicPosition - previousPosition) / movingTime;
            target.transform.RotateAround(target.transform.position, rotatePos, curRotateValue);
            rotateIncrease += curRotateValue;
            previousPosition = InGameMusicManager_s.Instance.musicPosition;
            yield return waitUpdate;
        }
        target.transform.RotateAround(target.transform.position, rotatePos, Mathf.Abs(rotatePos.x + rotatePos.y + rotatePos.z) - rotateIncrease);
        target.transform.localEulerAngles = new Vector3(MathF.Round(target.transform.localEulerAngles.x), Mathf.Round(target.transform.localEulerAngles.y), Mathf.Round(target.transform.localEulerAngles.z));
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
            yield return waitUpdate;
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
            yield return waitUpdate;
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
            yield return waitUpdate;
        }
        target.localScale = Vector3.one;
    }
}