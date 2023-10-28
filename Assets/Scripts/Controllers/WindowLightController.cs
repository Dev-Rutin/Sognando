using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowLightController : MonoBehaviour
{
    [SerializeField] private float _fadeTime;

    public void FadeStart(float alpha)
    {
        StartCoroutine(FadeOutGameObject(alpha));
    }
    private IEnumerator FadeOutGameObject(float alpha)
    {
        Color color = gameObject.GetComponent<SpriteRenderer>().color;
        float originAlpha = color.a;
        float time = 0f;
        
        while (gameObject.GetComponent<SpriteRenderer>().color.a < alpha)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(originAlpha, alpha, time / _fadeTime);
            gameObject.GetComponent<SpriteRenderer>().color = color;
            yield return null;
        }
    }
}
