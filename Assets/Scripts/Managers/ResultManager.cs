using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    [Header("Score")] 
    [SerializeField] private float _textFadeTime;
    [SerializeField] private TextMeshPro _clearText;
    [SerializeField] private TextMeshPro _rankText;
    [SerializeField] private TextMeshPro _scoreText;
    [SerializeField] private TextMeshPro _maxComboText;
    [SerializeField] private TextMeshPro _perfectText;
    [SerializeField] private TextMeshPro _goodText;
    [SerializeField] private TextMeshPro _missText;
    
    [Header("SceneFade")]
    [SerializeField] private float _fadeTime;
    [SerializeField] private GameObject _fadePenal;

    private bool _isPenalFading;

    private bool _isTextFading;
    // Start is called before the first frame update
    void Start()
    {
        if (!StageDataController.Instance.isClear)
        {
            _clearText.text = "Fail...";
        }

        _scoreText.text = StageDataController.Instance.score.ToString();
        _maxComboText.text = StageDataController.Instance.maxCombo.ToString();
        _perfectText.text = StageDataController.Instance.perfectCount.ToString();
        _goodText.text = StageDataController.Instance.goodCount.ToString();
        _missText.text = StageDataController.Instance.missCount.ToString();
        
        FadeUtlity.Instance.CallFade(_fadeTime, _fadePenal, EGameObjectType.UI, EFadeType.FadeIn);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPenalFading && !_isTextFading && Input.GetKeyDown(KeyCode.Return))
        {
            
        }
    }
}
