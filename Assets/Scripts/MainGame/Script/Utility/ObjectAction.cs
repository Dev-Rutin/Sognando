using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class ObjectAction
{
    static WaitForEndOfFrame waitUpdate = new WaitForEndOfFrame();
    public static IEnumerator MovingObj(GameObject target, Vector2 targetPos, double movingTime)
    {
        float lerpValue = 0;
        double startTime = InGameMusicManager_s.Instance.musicPosition;
        Vector2 startPos = target.transform.localPosition;
        while (lerpValue <= 1)
        {
            lerpValue = (float)((InGameMusicManager_s.Instance.musicPosition - startTime) * 1 / movingTime);
            target.transform.localPosition = Vector2.Lerp(startPos, targetPos, lerpValue);
            yield return waitUpdate;
        }
        target.transform.localPosition = targetPos;
    }
    public static IEnumerator RotateObj(GameObject target, Vector3 rotatePos, double movingTime)
    {
        double rotateIncrease = 0;
        double curRotateValue = 0;
        double previousPosition = InGameMusicManager_s.Instance.musicPosition;
        double startTime = InGameMusicManager_s.Instance.musicPosition;
        while (InGameMusicManager_s.Instance.musicPosition - startTime <= movingTime)
        {
            curRotateValue = Mathf.Abs(rotatePos.x + rotatePos.y + rotatePos.z) * (InGameMusicManager_s.Instance.musicPosition - previousPosition) / movingTime;
            target.transform.RotateAround(target.transform.position, rotatePos, (float)curRotateValue);
            rotateIncrease += curRotateValue;
            previousPosition = InGameMusicManager_s.Instance.musicPosition;
            yield return waitUpdate;
        }
        target.transform.RotateAround(target.transform.position, rotatePos, (float)(Mathf.Abs(rotatePos.x + rotatePos.y + rotatePos.z) - rotateIncrease));
        target.transform.localEulerAngles = new Vector3(MathF.Round(target.transform.localEulerAngles.x), Mathf.Round(target.transform.localEulerAngles.y), Mathf.Round(target.transform.localEulerAngles.z));
    }
    public static IEnumerator ImageFade(Image target, double time,bool isAllFade, float startValue, float endValue = 0)
    {
        float lerpValue = 0;
        double startTime = InGameMusicManager_s.Instance.musicPosition;
        while (lerpValue <= 1)
        {
            lerpValue = (float)((InGameMusicManager_s.Instance.musicPosition - startTime) * 1 /time);
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
    public static IEnumerator ImageFade(SpriteRenderer target, double time, bool isAllFade,float startValue, float endValue = 0,int repeat = 0)
    {
        for (int i = 0; i < repeat; i++)
        {
            float lerpValue = 0;
            double startTime = InGameMusicManager_s.Instance.musicPosition;
            while (lerpValue <= 1)
            {
                lerpValue = (float)((InGameMusicManager_s.Instance.musicPosition - startTime) * 1 / time);
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
    }
    public static IEnumerator ImageFade(CanvasGroup target, double time, bool isAllFade, float startValue, float endValue = 0, int repeat = 0)
    {
        for (int i = 0; i < repeat; i++)
        {
            float lerpValue = 0;
            double startTime = InGameMusicManager_s.Instance.musicPosition;
            while (lerpValue <= 1)
            {
                lerpValue = (float)((InGameMusicManager_s.Instance.musicPosition - startTime) * 1 / time);
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
    public static void ImageAlphaChange(CanvasGroup target, float value)
    {
        target.alpha = value;
    }
    public static IEnumerator ObjectScalePump(Transform target, Vector3 targetScale, float time)
    {
        float lerpValue = 0;
        double startTime = InGameMusicManager_s.Instance.musicPosition;
        while (lerpValue<=1)
        {
            lerpValue = (float)((InGameMusicManager_s.Instance.musicPosition - startTime) * 1 / time);
            target.localScale = Vector3.Lerp(Vector3.one, targetScale, lerpValue);
            yield return waitUpdate;
        }
        target.localScale = Vector3.one;
    }
}