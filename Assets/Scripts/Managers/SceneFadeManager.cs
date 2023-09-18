using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

// 간단 사용법: 사용할 스크립트에서 FadeCheck로 시간, bgm(null 넘겨도 됨) 넣어서 호출
// 만약 시작하자 마자 FadeIn을 호출해야 할 경우 Inspector에서 isFading 체크
public class SceneFadeManager : Singleton<SceneFadeManager>
{
    [SerializeField] private GameObject fadeImageActive;
    [SerializeField] private Image fadeImage;
    [SerializeField] private bool isPlaying = false;
    [SerializeField] private bool isFading = false;
    public bool _isFading
    {
        get
        {
            return isFading;
        }
    }
    public void FadeCheck(float t, AudioSource bgmSourceOrNull)
    {
        if(isFading)
        {
            StartCoroutine(FadeOut(t, bgmSourceOrNull));
        }
        else
        {
            if (!fadeImageActive.activeSelf)
            {
                fadeImageActive.SetActive(true);
            }
            StartCoroutine(FadeIn(t, bgmSourceOrNull));
        }
    }

    private IEnumerator FadeIn(float t, AudioSource bgmSourceOrNull)
    {
        fadeImage.enabled = true;
        if (isPlaying)
        {
            yield break;
        }
        yield return new WaitForSeconds(0.5f);
        isPlaying = true;
        Color color = fadeImage.color;

        float defaultValue = (float)ESettingsValue.On;
        float targetVolume = PlayerPrefs.GetFloat(PlayerPrefsKeyNames.MASTER_VOLUME, defaultValue);
        float time = 0f;

        while (fadeImage.color.a > 0f)
        {
            time += Time.deltaTime;
            color.a = math.lerp(1f, 0f, time / t);
            fadeImage.color = color;
            if (bgmSourceOrNull != null)
            {
                bgmSourceOrNull.volume = math.lerp(0f, targetVolume, time / t);
            }
            yield return null;
        }

        fadeImage.enabled = false;
        isPlaying = false;
        isFading = true;
    }

    private IEnumerator FadeOut(float t, AudioSource bgmSourceOrNull)
    {
        fadeImage.enabled = true;
        if (isPlaying)
        {
            yield break;
        }

        isPlaying = true;
        Color color = fadeImage.color;

        float defaultValue = (float)ESettingsValue.On;
        float originalVolume = PlayerPrefs.GetFloat(PlayerPrefsKeyNames.MASTER_VOLUME, defaultValue);
        float time = 0f;
        SceneSoundManager.Instance.PauseSeSound();
        while (fadeImage.color.a < 1f)
        {
            time += Time.deltaTime;
            color.a = math.lerp(0f, 1f, time / t);
            fadeImage.color = color;
            if (bgmSourceOrNull != null)
            {
                bgmSourceOrNull.volume = math.lerp(originalVolume, 0f, time / t);
            }

            color.a += Time.deltaTime / t;
            fadeImage.color = color;
            yield return null;
        }

        isPlaying = false;
        isFading = false;
    }
}