using System;
using System.Collections;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;

public class FadeUtlity : Singleton<FadeUtlity>
{
    
    public void CallFade(float fadeTime, GameObject gObject, EGameObjectType gameObjectType, EFadeType fadeType)
    {
        switch (gameObjectType)
        {
            case EGameObjectType.GameObject:
                StartCoroutine(fadeType == EFadeType.FadeIn
                    ? FadeInGameObject(fadeTime, gObject)
                    : FadeOutGameObject(fadeTime, gObject));
                break;
            case EGameObjectType.UI:
                StartCoroutine(fadeType == EFadeType.FadeIn
                    ? FadeInUI(fadeTime, gObject)
                    : FadeOutUI(fadeTime, gObject));
                break;
            case EGameObjectType.Skeleton:
                StartCoroutine(fadeType == EFadeType.FadeIn
                    ? FadeInSkeleton(fadeTime, gObject)
                    : FadeOutSkeleton(fadeTime, gObject));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameObjectType), gameObjectType, null);
        }
    }

    public void BlinkUI(float fadeTime, GameObject gObject)
    {
        StartCoroutine(BlinkUICoroutine(fadeTime, gObject));
    }

    public void StopFade()
    {
        StopAllCoroutines();
    }
    
    private IEnumerator FadeInUI(float fadeTime, GameObject gObject)
    {
        float color = gObject.GetComponent<CanvasGroup>().alpha;
        float time = 0f;
        
        while (gObject.GetComponent<CanvasGroup>().alpha > 0f)
        {
            time += Time.deltaTime;
            color = math.lerp(1f, 0f, time / fadeTime);
            gObject.GetComponent<CanvasGroup>().alpha = color;
            yield return null;
        }
        gObject.SetActive(false);
    }
    
    private IEnumerator FadeOutUI(float fadeTime, GameObject gObject)
    {
        gObject.SetActive(true);
        float color = gObject.GetComponent<CanvasGroup>().alpha;
        float time = 0f;
        
        while (gObject.GetComponent<CanvasGroup>().alpha < 1f)
        {
            time += Time.deltaTime;
            color = math.lerp(0f, 1f, time / fadeTime);
            gObject.GetComponent<CanvasGroup>().alpha = color;
            yield return null;
        }
    }
    
    private IEnumerator FadeInGameObject(float fadeTime, GameObject gObject)
    {
        Color color = gObject.GetComponent<SpriteRenderer>().color;
        float time = 0f;
        
        while (gObject.GetComponent<SpriteRenderer>().color.a > 0f)
        {
            time += Time.deltaTime;
            color.a = math.lerp(1f, 0f, time / fadeTime);
            gObject.GetComponent<SpriteRenderer>().color = color;
            yield return null;
        }
    }
    
    private IEnumerator FadeOutGameObject(float fadeTime, GameObject gObject)
    {
        Color color = gObject.GetComponent<SpriteRenderer>().color;
        float time = 0f;
        
        while (gObject.GetComponent<SpriteRenderer>().color.a < 1f)
        {
            time += Time.deltaTime;
            color.a = math.lerp(0f, 1f, time / fadeTime);
            gObject.GetComponent<SpriteRenderer>().color = color;
            yield return null;
        }
    }
    
    private IEnumerator FadeOutSkeleton(float fadeTime, GameObject gObject)
    {
        Color color = gObject.GetComponent<SpriteRenderer>().color;
        float time = 0f;
        
        while (gObject.GetComponent<SpriteRenderer>().color.a < 1f)
        {
            time += Time.deltaTime;
            color.a = math.lerp(0f, 1f, time / fadeTime);
            gObject.GetComponent<SpriteRenderer>().color = color;
            yield return null;
        }
    }
    private IEnumerator FadeInSkeleton(float fadeTime, GameObject gObject)
    {
        Spine.Skeleton playerSkeleton = gObject.GetComponent<SkeletonAnimation>().Skeleton;
        float color = playerSkeleton.A;
        float time = 0f;
        
        while (playerSkeleton.A > 0f)
        {
            time += Time.deltaTime;
            color = math.lerp(1f, 0f, time / fadeTime);
            playerSkeleton.A = color;
            yield return null;
        }
    }

    private IEnumerator BlinkUICoroutine(float fadeTime, GameObject gObject)
    {
        while (true)
        {
            yield return StartCoroutine(FadeInUI(fadeTime, gObject));
            yield return StartCoroutine(FadeOutUI(fadeTime, gObject));
        }
    }
}
