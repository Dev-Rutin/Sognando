using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class IntroController : MonoBehaviour
{
    //[SerializeField] private Button skipBtn;
    [SerializeField] private float FadeTime;
    [Header("CutSceneSprite")]
    [SerializeField] private Image cutscene;
    [SerializeField] private Image fadeImage;
    [SerializeField] private Sprite[] cutsceneSprites;
    [Header("TextDialogs")]
    [TextArea]
    [SerializeField] private string[] dialogStrings;
    [SerializeField] private TextMeshProUGUI textOutput;
    [SerializeField] private float TextDelayTime;
    private int typingCount = 0;

    private void Start()
    {
        //skipBtn.onClick.AddListener(delegate { SkipIntro(); });
        SceneSoundManager.Instance.PlaySound(ESoundTypes.Bgm, SceneSoundNames.INTRO_BGM);
        SceneFadeManager.Instance.FadeCheck(FadeTime, null);
        StartCoroutine(TypingCoroutine());
        StartCoroutine(CutSceneLoopCoroutine());
        StartCoroutine(IntroEndCheck());
    }

    private IEnumerator IntroEndCheck()
    {
        while(typingCount != dialogStrings.Length)
        {
            yield return new WaitForSeconds(1f);
        }
        if (typingCount == dialogStrings.Length)
        {
            yield return new WaitForSeconds(10f);
            SceneFadeManager.Instance.FadeCheck(FadeTime, null);
            StartCoroutine(LoadGameScene());
            yield return null;
        }
    }

    // 문자 타이핑 코루틴
    private IEnumerator TypingCoroutine()
    {
        yield return new WaitForSeconds(FadeTime/5);
        while(typingCount < dialogStrings.Length)
        {
            TypingManager.Instance.Typing(dialogStrings, textOutput);
            typingCount++;
            yield return new WaitForSeconds(TextDelayTime);
        }
        yield return null;
    }

    // 컷 씬 변경용 코루틴
    private IEnumerator CutSceneLoopCoroutine()
    {
        yield return new WaitForSeconds(FadeTime/5);
        while (typingCount <= dialogStrings.Length)
        {
            cutscene.sprite = cutsceneSprites[typingCount - 1];
            if(typingCount == dialogStrings.Length)
            {
                yield break;
            }
            yield return new WaitForSeconds(TextDelayTime * 2/ 3);
            StartCoroutine(FadeOutImageCoroutine());
            yield return new WaitForSeconds(TextDelayTime / 3);
            StartCoroutine(FadeInImageCoroutine());
        }
        yield return null;
    }

    // 인트로 이미지용 페이드 인/아웃
    private IEnumerator FadeOutImageCoroutine()
    {
        Color color = fadeImage.color;
        float time = 0f;
        while (fadeImage.color.a < 1f)
        {
            time += Time.deltaTime;
            color.a = math.lerp(0f, 1f, time / (FadeTime - 1));
            fadeImage.color = color;
            yield return null;
        }
        yield return null;
    }
    private IEnumerator FadeInImageCoroutine()
    {
        Color color = fadeImage.color;
        float time = 0f;
        while (fadeImage.color.a > 0f)
        {
            time += Time.deltaTime;
            color.a = math.lerp(1f, 0f, time / (FadeTime - 1));
            fadeImage.color = color;
            yield return null;
        }
        yield return null;
    }

    // 인트로 스킵 버튼
    private void SkipIntro()
    {
        SceneFadeManager.Instance.FadeCheck(FadeTime, null);
        StartCoroutine(LoadGameScene());
    }
    private IEnumerator LoadGameScene()
    {
        yield return new WaitForSeconds(FadeTime);
        if (!SceneFadeManager.Instance._isFading)
        {
            SceneManager.LoadScene("GameScene");
            yield return null;
        }
    }
}
