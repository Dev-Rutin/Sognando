using System.Collections;
using Spine.Unity;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

public class IntroManager : MonoBehaviour
{
    [Header("PlayerState")]
    [SerializeField] private GameObject _playerObject;
    [SerializeField] private float _fadeTime;
    [SerializeField] private float _sceneSpeed;
    private bool _isPlayerFading;
    private Spine.Skeleton _playerColor;

    [Header("FadeScreen")] 
    [SerializeField] private GameObject _fadePanel;
    [Header("TextDialogs")]
    [TextArea]
    [SerializeField] private string[] _dialogStrings;
    [SerializeField] private TextMeshProUGUI _textOutput;
    [SerializeField] private float _textDelayTime;
    private int _typingCount = 0;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        SceneSoundManager.Instance.PlaySound(ESoundTypes.Bgm, SceneSoundNames.INTRO_BGM);
        _playerColor = _playerObject.GetComponent<SkeletonAnimation>().skeleton;
        _playerColor.A = 0;
        StartCoroutine(FadeInPlayer());
        StartCoroutine(TypingCoroutine());
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    // 문자 타이핑 코루틴
    private IEnumerator TypingCoroutine()
    {
        yield return new WaitForSeconds(_fadeTime/5);
        while(_typingCount < _dialogStrings.Length)
        {
            TypingManager.Instance.Typing(_dialogStrings, _textOutput);
            _typingCount++;
            yield return new WaitForSeconds(_textDelayTime);
        }
        yield return null;
    }

    private IEnumerator WalkPlayer()
    {
        yield return null;
    }
    
    private IEnumerator OpenDoor()
    {
        yield return null;
    }

    private IEnumerator FadeInPlayer()
    {
        float color = _playerColor.A;
        
        float time = 0f;

        while (_playerColor.A < 1f)
        {
            time += Time.deltaTime;
            color = math.lerp(0f, 1f, time / _fadeTime);
            _playerColor.A = color;
            yield return null;
        }
        _isPlayerFading = false;
    }
    
    private IEnumerator FadeOutPlayer()
    {
        float color = _playerColor.A;
        
        float time = 0f;

        while (_playerColor.A > 0f)
        {
            time += Time.deltaTime;
            color = math.lerp(1f, 0f, time / _fadeTime);
            _playerColor.A = color;
            yield return null;
        }
        yield return null;
    }
}
