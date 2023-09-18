using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI; 

public class IntroManager : MonoBehaviour
{
    [Header("PlayerState")]
    [SerializeField] private GameObject _playerObject;
    [SerializeField] private float _fadeTime;
    [SerializeField] private float _sceneSpeed;
    [Header("FadeScreen")]
    [Header("TextDialogs")]
    [TextArea]
    [SerializeField] private string[] _dialogStrings;
    [SerializeField] private TextMeshProUGUI _textOutput;
    [SerializeField] private float _textDelayTime;
    private int _typingCount = 0;
    
    private SpriteRenderer _playerColor;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerColor = _playerObject.GetComponent<SpriteRenderer>();
        StartCoroutine(FadeOutPlayer());
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
        Color color = _playerColor.color;
        
        float time = 0f;

        while (_playerColor.color.a > 0f)
        {
            time += Time.deltaTime;
            color.a = math.lerp(1f, 0f, time / _fadeTime);
            _playerColor.color = color;
            yield return null;
        }
    }
    
    private IEnumerator FadeOutPlayer()
    {
        Color color = _playerColor.color;
        
        float time = 0f;

        while (_playerColor.color.a > 0f)
        {
            time += Time.deltaTime;
            color.a = math.lerp(1f, 0f, time / _fadeTime);
            _playerColor.color = color;
            yield return null;
        }
        yield return null;
    }
}
