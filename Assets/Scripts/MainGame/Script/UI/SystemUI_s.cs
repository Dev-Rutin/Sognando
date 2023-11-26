using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SystemUI_s : Singleton<SystemUI_s>
{
    [Header("main game ui canvas")]
    [SerializeField] private Transform _comboTsf;
    [SerializeField] private TextMeshProUGUI _comboText;
    [SerializeField] private Transform _scoreTsf;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private GameObject _beatImageUIObj;
    private SpriteRenderer _beatImageUI;
    [SerializeField] private Sprite _missSprite;
    [SerializeField] private Sprite _goodSprite;
    [SerializeField] private Sprite _perfectSprite;
    [SerializeField] private Animator _songNoteAnimator;
    [Header("main game canvas")]
    [SerializeField] private Transform _beatJudgeTsf;
    [SerializeField] private Image _beatImage;
    [SerializeField] private Sprite _beatPerfectSprite;
    [SerializeField] private Sprite _beatGoodSprite;
    [SerializeField] private Sprite _beatMissSprite;
    [SerializeField] private Sprite _beatDefaultSprite;

    [Header("camera")]
    WaitForEndOfFrame waitUpdate;
    [SerializeField] private int _cameraShakeCount;
    private void Start()
    {
        waitUpdate = new WaitForEndOfFrame();
        _beatImageUI = _beatImageUIObj.GetComponent<SpriteRenderer>();
        _beatImageUIObj.SetActive(false);
    }
    public void UpdateCombo(int value)
    {
        if (value !=0)
        {
            StartCoroutine(ObjectAction.ObjectScalePump(_comboTsf, new Vector3(1.2f, 1.2f, 1.2f), 0.08f));
        }
        _comboText.text = value.ToString();
    }
    public void UpdateScore(int value)
    {
        _scoreText.text = value.ToString();
    }
    public void Miss()
    {
        _beatImage.sprite = _beatMissSprite;
        _beatImageUI.sprite = _missSprite;
        StartCoroutine(BeatUIShow());
    }
    public void Good()
    {
        _beatImage.sprite = _beatGoodSprite;
        _beatImageUI.sprite = _goodSprite;
        StartCoroutine(BeatUIShow());
    }
    public void Perfect()
    {
        _beatImage.sprite = _beatPerfectSprite;
        _beatImageUI.sprite = _perfectSprite;
        StartCoroutine(BeatUIShow());
    }
    private IEnumerator BeatUIShow()
    {
        _beatImageUIObj.SetActive(true);
        float startTime = InGameMusicManager_s.Instance.musicPosition;
        while(InGameMusicManager_s.Instance.musicPosition-startTime<=InGameMusicManager_s.Instance.secPerBeat/2)
        {
            yield return waitUpdate;
        }
        _beatImageUIObj.SetActive(false);
    }
    public IEnumerator CameraShake()
    {
        float startTime;
        bool lastShakeIsLeft = false;
        bool lastShakeIsDown = false;
        Vector2 shakeTarget = Vector2.zero;
        for (int i=0;i<_cameraShakeCount;i++)
        {
            shakeTarget.x = Random.Range(0, lastShakeIsLeft ? -0.2f : 0.2f);
            shakeTarget.y = Random.Range(0, lastShakeIsDown ? -0.2f : 0.2f);
            startTime = InGameMusicManager_s.Instance.musicPosition;
            Debug.Log(shakeTarget);
            StartCoroutine(ObjectAction.MovingObj(Camera.main.gameObject, shakeTarget, 0.03f));
            while (InGameMusicManager_s.Instance.musicPosition - startTime <= 0.1f)
            {
                yield return waitUpdate;
            }
        }
        StartCoroutine(ObjectAction.MovingObj(Camera.main.gameObject, Vector2.zero, 0.03f));
    }
}
