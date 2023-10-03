using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class FadeUtlity : Singleton<FadeUtlity>
{
    
    public void CallFade(float fadeTime, GameObject gameObject, EGameObjectType gameObjectType, EFadeType fadeType)
    {
        switch (gameObjectType)
        {
            case EGameObjectType.GameObject:
                StartCoroutine(fadeType == EFadeType.FadeIn
                    ? FadeInGameObject(fadeTime, gameObject)
                    : FadeOutGameObject(fadeTime, gameObject));
                break;
            case EGameObjectType.UI:
                StartCoroutine(fadeType == EFadeType.FadeIn
                    ? FadeInUI(fadeTime, gameObject)
                    : FadeOutUI(fadeTime, gameObject));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameObjectType), gameObjectType, null);
        }
    }

    public void BlinkUI(float fadeTime, GameObject gameObject)
    {
        StartCoroutine(BlinkUICorutine(fadeTime, gameObject));
    }

    public void StopFade()
    {
        StopAllCoroutines();
    }
    
    private IEnumerator FadeInUI(float fadeTime, GameObject gameObject)
    {
        float color = gameObject.GetComponent<CanvasGroup>().alpha;
        float time = 0f;
        
        while (gameObject.GetComponent<CanvasGroup>().alpha > 0f)
        {
            time += Time.deltaTime;
            color = math.lerp(1f, 0f, time / fadeTime);
            gameObject.GetComponent<CanvasGroup>().alpha = color;
            yield return null;
        }
        gameObject.SetActive(false);
    }
    
    private IEnumerator FadeOutUI(float fadeTime, GameObject gameObject)
    {
        gameObject.SetActive(true);
        float color = gameObject.GetComponent<CanvasGroup>().alpha;
        float time = 0f;
        
        while (gameObject.GetComponent<CanvasGroup>().alpha < 1f)
        {
            time += Time.deltaTime;
            color = math.lerp(0f, 1f, time / fadeTime);
            gameObject.GetComponent<CanvasGroup>().alpha = color;
            yield return null;
        }
    }
    
    private IEnumerator FadeInGameObject(float fadeTime, GameObject gameObject)
    {
        Color color = gameObject.GetComponent<SpriteRenderer>().color;
        float time = 0f;
        
        while (gameObject.GetComponent<SpriteRenderer>().color.a > 0f)
        {
            time += Time.deltaTime;
            color.a = math.lerp(1f, 0f, time / fadeTime);
            gameObject.GetComponent<SpriteRenderer>().color = color;
            yield return null;
        }
    }
    
    private IEnumerator FadeOutGameObject(float fadeTime, GameObject gameObject)
    {
        Color color = gameObject.GetComponent<SpriteRenderer>().color;
        float time = 0f;
        
        while (gameObject.GetComponent<SpriteRenderer>().color.a < 1f)
        {
            time += Time.deltaTime;
            color.a = math.lerp(0f, 1f, time / fadeTime);
            gameObject.GetComponent<SpriteRenderer>().color = color;
            yield return null;
        }
    }

    private IEnumerator BlinkUICorutine(float fadeTime, GameObject gameObject)
    {
        while (true)
        {
            yield return StartCoroutine(FadeInUI(fadeTime, gameObject));
            yield return StartCoroutine(FadeOutUI(fadeTime, gameObject));
        }
    }
}
