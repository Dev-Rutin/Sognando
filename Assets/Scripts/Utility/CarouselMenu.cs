using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarouselMenu : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollBar;
    [SerializeField] private GameObject Stage2;
    [SerializeField] private GameObject[] Stage2Childs;
    [SerializeField] private GameObject rest4;
    [SerializeField] private GameObject rest16;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private Sprite stageBG;

    [SerializeField] private Button easyButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Sprite[] easyButtons;
    [SerializeField] private Sprite[] hardButtons;

    private float _scrollPos;
    private bool _isArrowPressed;
    private float[] _indexes;
    public int Index { get; private set; }
    private float _distacne = 0.25f;

    private void Awake()
    {
        Index = 0;
        if (PlayerPrefs.HasKey(PlayerPrefsKeyNames.STAGE1CLEARCHECK))
        {
            scoreText.enabled = true;
            comboText.enabled = true;
            rest16.SetActive(false);
            rest4.SetActive(false);
            scoreText.text = PlayerPrefs.GetInt(PlayerPrefsKeyNames.STAGE1HIGHSCORE).ToString();
            comboText.text = PlayerPrefs.GetInt(PlayerPrefsKeyNames.STAGE1MAXCOMBO).ToString();
        }

        if (PlayerPrefs.HasKey(PlayerPrefsKeyNames.STAGE2CLEARCHECK))
        {
            Stage2.GetComponent<Image>().sprite = stageBG;
            Stage2Childs[0].SetActive(true);
            Stage2Childs[1].SetActive(true);
        }
        StageDataController.Instance.isHardModeOn = false;
        easyButton.GetComponent<Image>().sprite = easyButtons[1];
        
        easyButton.onClick.AddListener(PressEasy);
        hardButton.onClick.AddListener(PressHard);
    }

    void Update()
    {
        _indexes = new float[transform.childCount];
        float distacne = 1f / (_indexes.Length - 1);
        for(int i = 0; i< _indexes.Length; i++)
        {
            _indexes[i] = distacne * i;
        }

        if (Input.GetMouseButton(0))
        {
            _scrollPos = scrollBar.value;
        }
        else
        {
            if (!_isArrowPressed)
            {
                for (int i = 0; i < _indexes.Length; i++)
                {
                    if (_scrollPos < _indexes[i] + (distacne / 2) && _scrollPos > _indexes[i] - (distacne / 2))
                    {
                        scrollBar.value = Mathf.Lerp(scrollBar.value, _indexes[i], 0.1f);
                    }
                }
            }
        }

        for (int i = 0; i < _indexes.Length; i++)
        {
            if (_scrollPos < _indexes[i] + (distacne / 2) && _scrollPos > _indexes[i] - (distacne / 2))
            {
                transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1f, 1f), 0.1f);
                for(int j = 0; j < _indexes.Length; j++)
                {
                    if(j != i)
                    {
                        transform.GetChild(j).localScale = Vector2.Lerp(transform.GetChild(j).localScale, new Vector2(0.8f, 0.8f), 0.1f);
                    }
                }
            }
        }    
    }

    public void LeftArrow()
    {
        if (--Index < 0)
        {
            Index = 0;
        }

        float distacne = 1f / (_indexes.Length - 1);
        
        StartCoroutine(selectBtn(Index * _distacne));
    }
    
    public void RightArrow()
    {
        if (++Index > 4)
        {
            Index = 4;
        }

        float distacne = 1f / (_indexes.Length - 1);
        
        StartCoroutine(selectBtn(Index * _distacne));
    }

    IEnumerator selectBtn(float targetValue)
    {
        _isArrowPressed = true;
        while (true)
        {
            yield return null;
            scrollBar.value = Mathf.Lerp(scrollBar.value, targetValue, 0.1f);
            if (Mathf.Abs(scrollBar.value - targetValue) <= 0.1f)
            {
                _scrollPos = scrollBar.value;
                _isArrowPressed = false;
                break;
            }
        }

        switch (Index)
        {
            case 0:
                if (PlayerPrefs.HasKey(PlayerPrefsKeyNames.STAGE1CLEARCHECK))
                {
                    scoreText.enabled = true;
                    comboText.enabled = true;
                    rest16.SetActive(false);
                    rest4.SetActive(false);
                    scoreText.text = PlayerPrefs.GetInt(PlayerPrefsKeyNames.STAGE1HIGHSCORE).ToString();
                    comboText.text = PlayerPrefs.GetInt(PlayerPrefsKeyNames.STAGE1MAXCOMBO).ToString();
                }
                break;
            case 1:
                if (PlayerPrefs.HasKey(PlayerPrefsKeyNames.STAGE2CLEARCHECK))
                {
                    scoreText.enabled = true;
                    comboText.enabled = true;
                    rest16.SetActive(false);
                    rest4.SetActive(false);
                    scoreText.text = PlayerPrefs.GetInt(PlayerPrefsKeyNames.STAGE2HIGHSCORE).ToString();
                    comboText.text = PlayerPrefs.GetInt(PlayerPrefsKeyNames.STAGE2MAXCOMBO).ToString();
                }
                else
                {
                    scoreText.enabled = false;
                    comboText.enabled = false;
                    rest16.SetActive(true);
                    rest4.SetActive(true);
                }
                break;
            case 2:
            case 3:
            case 4:
                scoreText.enabled = false;
                comboText.enabled = false;
                rest16.SetActive(true);
                rest4.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void PressEasy()
    {
        StageDataController.Instance.isHardModeOn = false;
        StageDataController.Instance.stage = Index + 1;
        easyButton.GetComponent<Image>().sprite = easyButtons[1];
        hardButton.GetComponent<Image>().sprite = hardButtons[0];
    }
    
    private void PressHard()
    {
        StageDataController.Instance.isHardModeOn = true;
        StageDataController.Instance.stage = Index + 1;
        easyButton.GetComponent<Image>().sprite = easyButtons[0];
        hardButton.GetComponent<Image>().sprite = hardButtons[1];
    }
}
