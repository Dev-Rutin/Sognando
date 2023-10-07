using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypingUtility : Singleton<TypingUtility>
{
    [SerializeField] private float _timeForCharacter;
    [SerializeField] private float _timeForCharacter_Fast; 
    private float _characterTime;

    private string _dialogsSave;
    private TextMeshProUGUI _tmpSave;

    private static bool _isDialogEnd;

    private bool _isTypingEnd = false;
    private int _dialogNumber = 0;
    private float _timer; // 출력 타이머

    private void Awake()
    {
        _timer = _timeForCharacter;
        _characterTime = _timeForCharacter;
    }

    public void Typing(string dialogs, TextMeshProUGUI textObj)
    {
        StorySceneManager.Instance.IsTyping = true;
        _isDialogEnd = false;
        _dialogsSave = dialogs;
        _tmpSave = textObj;
        if (_dialogNumber < dialogs.Length)
        {
            //받아온 다이얼 로그를 char로 변환.
            char[] chars = dialogs.ToCharArray();
            StartCoroutine(Typer(chars, textObj));
        }
        else
        {
            _tmpSave.text = "";
            _isDialogEnd = true;
            _dialogsSave = null;
            _tmpSave = null;
            _dialogNumber = 0;
        }
    }

    // 입력에 따른 빠른 문장 넘기기
    public void GetInputDown()
    {
        if (_dialogsSave != null)
        {
            if (_isTypingEnd)
            {
                _tmpSave.text = "";
                Typing(_dialogsSave, _tmpSave);
            }
            else
            {
                _characterTime = _timeForCharacter_Fast;
            }
        }
    }
    public void GetInputUp()
    {
        if (_dialogsSave != null)
        {
            _characterTime = _timeForCharacter;
        }
    }

    // 타이핑 코루틴
    private IEnumerator Typer(char[] chars, TextMeshProUGUI textObj)
    {
        int currentChar = 0;
        int charLength = chars.Length;
        _isTypingEnd = false;
        if(textObj.text.Length != 0)
        {
            textObj.text = "";
        }
        while (currentChar < charLength)
        {
            if(chars[currentChar] == '<')
            {
                _characterTime = 0f;
            }
            if(chars[currentChar] == '>')
            {
                _characterTime = _timeForCharacter;
            }
            if (_timer >= 0)
            {
                if(_characterTime != 0)
                {
                    yield return null;
                }
                _timer -= Time.deltaTime;
            }
            else
            {
                textObj.text += chars[currentChar].ToString();
                currentChar++;
                _timer = _characterTime;
            }
        }
        if (currentChar >= charLength)
        {
            _isTypingEnd = true;
            _dialogNumber++;
            StorySceneManager.Instance.IsTyping = false;
            yield break;
        }
    }
}