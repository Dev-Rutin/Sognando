using System.Collections;
using UnityEngine;
using TMPro;

public class TypingManager : Singleton<TypingManager>
{
    [SerializeField] private float timeForCharacter;
    [SerializeField] private float timeForCharacter_Fast; 
    private float characterTime;

    private string[] dialogsSave;
    TextMeshProUGUI tmpSave;

    private static bool isDialogEnd;

    private bool isTypingEnd = false;
    private int dialogNumber = 0;
    private float timer; // 출력 타이머

    private void Awake()
    {
        timer = timeForCharacter;
        characterTime = timeForCharacter;
    }

    public void Typing(string[] dialogs, TextMeshProUGUI textObj)
    {
        isDialogEnd = false;
        dialogsSave = dialogs;
        tmpSave = textObj;
        if (dialogNumber < dialogs.Length)
        {
            //받아온 다이얼 로그를 char로 변환.
            char[] chars = dialogs[dialogNumber].ToCharArray();
            StartCoroutine(Typer(chars, textObj));
        }
        else
        {
            tmpSave.text = "";
            isDialogEnd = true;
            dialogsSave = null;
            tmpSave = null;
            dialogNumber = 0;
        }
    }

    // 입력에 따른 빠른 문장 넘기기
    public void GetInputDown()
    {
        if (dialogsSave != null)
        {
            if (isTypingEnd)
            {
                tmpSave.text = "";
                Typing(dialogsSave, tmpSave);
            }
            else
            {
                characterTime = timeForCharacter_Fast;
            }
        }
    }
    public void GetInputUp()
    {
        if (dialogsSave != null)
        {
            characterTime = timeForCharacter;
        }
    }

    // 타이핑 코루틴
    private IEnumerator Typer(char[] chars, TextMeshProUGUI textObj)
    {
        int currentChar = 0;
        int charLength = chars.Length;
        isTypingEnd = false;
        if(textObj.text.Length != 0)
        {
            textObj.text = "";
        }
        while (currentChar < charLength)
        {
            if(chars[currentChar] == '<')
            {
                characterTime = 0f;
            }
            if(chars[currentChar] == '>')
            {
                characterTime = timeForCharacter;
            }
            if (timer >= 0)
            {
                if(characterTime != 0)
                {
                    yield return null;
                }
                timer -= Time.deltaTime;
            }
            else
            {
                textObj.text += chars[currentChar].ToString();
                currentChar++;
                timer = characterTime;
            }
        }
        if (currentChar >= charLength)
        {
            isTypingEnd = true;
            dialogNumber++;
            yield break;
        }
    }
}