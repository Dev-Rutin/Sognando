using System;
using System.Collections;
using System.Collections.Generic;
using FMODPlus;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ResultManager : MonoBehaviour
{
    [Header("Score")] 
    [SerializeField] private float _textFadeTime;
    [SerializeField] private TextMeshProUGUI _clearText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _maxComboText;
    [SerializeField] private TextMeshProUGUI _perfectText;
    [SerializeField] private TextMeshProUGUI _goodText;
    [SerializeField] private TextMeshProUGUI _missText;
    [SerializeField] private CommandSender _scoreSoundSender;
    
    [Header("SceneFade")]
    [SerializeField] private float _fadeTime;
    [SerializeField] private GameObject _fadePenal;

    [Header("RankParameter")] 
    [SerializeField] private Image _rankImage;
    [SerializeField] private float _scaleLerpTime;
    [SerializeField] private Sprite[] _ranks;
    [SerializeField] private CommandSender _rankSoundSender;

    [SerializeField] private GameObject _restartText;

    [SerializeField] private int _maxCountSpeed;
    [SerializeField] private int _minCountSpeed;

    private bool _isPenalFading;

    private bool _isTextFading;

    // Start is called before the first frame update
    void Start()
    {
        /*if (!StageDataController.Instance.isClear)
        {
            _clearText.text = "Fail...";
        }

        _scoreText.text = StageDataController.Instance.score.ToString();
        _maxComboText.text = StageDataController.Instance.maxCombo.ToString();
        _perfectText.text = StageDataController.Instance.perfectCount.ToString();
        _goodText.text = StageDataController.Instance.goodCount.ToString();
        _missText.text = StageDataController.Instance.missCount.ToString();
        _isPenalFading = true;*/
        
        CalcRank();
        StartCoroutine(StartCount());
    }

    // Update is called once per frame
    void Update()
    {
        /*if (_isPenalFading && !_isTextFading && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("GameScene");
        }*/
    }

    private IEnumerator StartCount()
    {
        FadeUtlity.Instance.CallFade(_fadeTime, _fadePenal, EGameObjectType.UI, EFadeType.FadeIn);

        yield return new WaitForSeconds(0.5f);
        
        //Score
        int Scoredata = 0;
        int ETCData = 0;
        int maxscore = Int32.Parse(_scoreText.text) + 1;
        int maxCombo = Int32.Parse(_maxComboText.text);
        int maxPerfect = Int32.Parse(_perfectText.text);
        int maxGood = Int32.Parse(_goodText.text);
        int maxMiss = Int32.Parse(_missText.text);
        _scoreSoundSender.SendCommand();
        while (Scoredata < maxscore)
        {
            Scoredata += Random.Range(_maxCountSpeed, _minCountSpeed);
            ETCData++;
            if (Scoredata >= maxscore)
            {
                Scoredata = maxscore;
            }

            if (ETCData <= maxCombo)
            {
                _maxComboText.text = ETCData.ToString();
            }

            if (ETCData <= maxPerfect)
            {
                _perfectText.text = ETCData.ToString();
            }
            
            if (ETCData <= maxGood)
            {
                _goodText.text = ETCData.ToString();
            }
            
            if (ETCData <= maxMiss)
            {
                _missText.text = ETCData.ToString();
            }
            _scoreText.text = Scoredata++.ToString();
            yield return null;
        }
        SoundUtility.Instance.SFXAudioSource.Stop();
        yield return null;

        _rankImage.enabled = true;
        
        Vector3 imageScale = _rankImage.rectTransform.localScale;
        Vector3 targetScale = new Vector3(1, 1, 1);
        Vector3 startVector = imageScale;
        float time = 0;
        _rankSoundSender.SendCommand();
        while (imageScale != targetScale)
        {
            time += Time.deltaTime;
            imageScale = Vector3.Lerp(startVector, targetScale, time / _scaleLerpTime);
            _rankImage.rectTransform.localScale = imageScale;
            yield return null;;
        }
        
        FadeUtlity.Instance.BlinkUI(_textFadeTime, _restartText);
    }

    private void CalcRank()
    {
        float totalValue = Int32.Parse(_perfectText.text) * 2 + Int32.Parse(_goodText.text);
        float minusPer = Int32.Parse(_missText.text) * 2;
        int minusValue = Mathf.RoundToInt(totalValue / 100 * minusPer);
        int rankValue = Mathf.RoundToInt((totalValue - minusValue) / totalValue * 100);
        if (rankValue >= (int)ERankValue.S)
        {
            _rankImage.sprite = _ranks[0];
        }
        else if (rankValue > (int)ERankValue.A)
        {
            _rankImage.sprite = _ranks[1];
        }
        else if (rankValue > (int)ERankValue.B)
        {
            _rankImage.sprite = _ranks[2];
        }
        else if (rankValue > (int)ERankValue.C)
        {
            _rankImage.sprite = _ranks[3];
        }
        else if (rankValue > (int)ERankValue.D)
        {
            _rankImage.sprite = _ranks[4];
        }
        else
        {
            _rankImage.sprite = _ranks[5];
        }
    }
    
}
