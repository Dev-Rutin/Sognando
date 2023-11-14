using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SystemUI_s : Singleton<SystemUI_s>
{
    [Header("main game ui canvas")]
    [SerializeField] private TextMeshProUGUI _comboText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private SpriteRenderer _beatImage_UI;
    [SerializeField] private Sprite _missSprite;
    [SerializeField] private Sprite _goodSprite;
    [SerializeField] private Sprite _perfectSprite;
    [Header("main game canvas")]
    [SerializeField] private Image _beatImage;
    [SerializeField] private Sprite _beatPerfectImg;
    [SerializeField] private Sprite _beatGoodImg;
    [SerializeField] private Sprite _beatMissImg;
    [SerializeField] private Sprite _beatDefaultImg;
    public void UpdateCombo(int value)
    {
        _comboText.text = value.ToString();
    }
    public void UpdateScore(int value)
    {
        _scoreText.text = value.ToString();
    }
    public void Miss()
    {
        _beatImage.sprite = _beatMissImg;
        _beatImage_UI.sprite = _missSprite;
    }
    public void Good()
    {
        _beatImage.sprite = _beatGoodImg;
        _beatImage_UI.sprite = _goodSprite;
    }
    public void Perfect()
    {
        _beatImage.sprite = _beatPerfectImg;
        _beatImage_UI.sprite = _perfectSprite;
    }
    public void DefaultShow()
    {
        _beatImage.sprite = _beatDefaultImg;
        _beatImage_UI.sprite = null;
    }

}
