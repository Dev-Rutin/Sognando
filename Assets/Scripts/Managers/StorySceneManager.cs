using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StorySceneManager : Singleton<StorySceneManager>
{
    [SerializeField] private GameObject _fadePenal;
    [SerializeField] private float _fadeTime;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _npc;
    [SerializeField] private GameObject _creature;
    [SerializeField] private GameObject _background;
    [SerializeField] private GameObject _backgroundWithCharacter;
    [SerializeField] private GameObject _backgroundZoomIn;
    [SerializeField] private GameObject _storyDataObject;
    private Queue<string> _dialogQueue;
    private Queue<string> _characterQueue;
    private Queue<string> _stateQueue;
    private Color _dialogCharacterColor;
    private Color _grayColor;
    

    [Header("Dialog")]
    [SerializeField] private TextMeshProUGUI _textOutput;
    [SerializeField] private float _textDelayTime;
    [SerializeField] private GameObject _icon;
    [SerializeField] private float _backgroundFadeTime;
    private int _typingCount = 0;
    public bool IsTyping { get; set; }

    private bool _isStoryStart;

    private bool _isFading;
    // Start is called before the first frame update
    void Start()
    {
        IsTyping = false;
        List<Dictionary<string, object>> csvRead = CSVReader.Read("StoryStage1");
        _dialogQueue = new Queue<string>();
        _characterQueue = new Queue<string>();
        _stateQueue = new Queue<string>();

        _dialogCharacterColor = new Color(1f, 1f, 1f);
        _grayColor = new Color(0.5f, 0.5f, 0.5f);

        foreach (var data in csvRead)
        {
            _dialogQueue.Enqueue(data["Dialog"].ToString());
            _characterQueue.Enqueue(data["Character"].ToString());
            _stateQueue.Enqueue(data["State"].ToString());
        }

        StartCoroutine(StartScene());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
        }
    }

    private IEnumerator StartScene()
    {
        FadeUtlity.Instance.CallFade(_fadeTime, _fadePenal, EGameObjectType.UI, EFadeType.FadeIn);
        yield return new WaitWhile(() => _fadePenal.GetComponent<CanvasGroup>().alpha != 0);
        StartCoroutine(TypingCoroutine());
    }

    private IEnumerator TypingCoroutine()
    {
        Debug.Log($"character : {_characterQueue.Peek()}, state : {_stateQueue.Peek()}, dialog : {_dialogQueue.Peek()}");
        yield return StartCoroutine(ChangeCharacter(_characterQueue.Dequeue(), _stateQueue.Dequeue()));
        TypingUtility.Instance.Typing(_dialogQueue.Dequeue(), _textOutput);
        _typingCount++;
        while (_dialogQueue.Count != 0)
        {
            if (!IsTyping)
            {
                _icon.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _icon.SetActive(false);
                    _textOutput.text = "";
                    yield return StartCoroutine(ChangeCharacter(_characterQueue.Dequeue(), _stateQueue.Dequeue()));
                    TypingUtility.Instance.Typing(_dialogQueue.Dequeue(), _textOutput);
                    _typingCount++;
                    switch (_typingCount)
                    {
                        case 3:
                            FadeUtlity.Instance.CallFade(_backgroundFadeTime, _backgroundWithCharacter, EGameObjectType.UI, EFadeType.FadeOut);
                            yield return new WaitWhile(() => Math.Abs(_backgroundWithCharacter.GetComponent<CanvasGroup>().alpha - 1) > 0);
                            break;
                        case 5:
                            FadeUtlity.Instance.CallFade(_backgroundFadeTime, _backgroundZoomIn, EGameObjectType.UI, EFadeType.FadeOut);
                            yield return new WaitWhile(() => Math.Abs(_backgroundZoomIn.GetComponent<CanvasGroup>().alpha - 1) > 0);
                            break;
                        case 8:
                            FadeUtlity.Instance.CallFade(_backgroundFadeTime, _backgroundZoomIn, EGameObjectType.UI, EFadeType.FadeIn);
                            yield return new WaitWhile(() => _backgroundZoomIn.GetComponent<CanvasGroup>().alpha != 0);
                            break;
                        case 10:
                            FadeUtlity.Instance.CallFade(_backgroundFadeTime, _backgroundWithCharacter, EGameObjectType.UI, EFadeType.FadeIn);
                            yield return new WaitWhile(() => _backgroundWithCharacter.GetComponent<CanvasGroup>().alpha != 0);
                            break;
                    }
                }
            }
            yield return null;
        }

        StartCoroutine(ChangeScene());
    }

    private IEnumerator ChangeCharacter(string characterType, string characterState)
    {
        Image playerSprite = _player.GetComponent<Image>();
        Image npcSprite = _npc.GetComponent<Image>();
        Image creatureSprite = _creature.GetComponent<Image>();
        
        switch (characterType)
        {
            case "Player":
                if (!_player.activeSelf)
                {
                    _player.SetActive(true);
                    FadeUtlity.Instance.CallFade(0.5f, _player, EGameObjectType.UI, EFadeType.FadeOut);
                }
                _textOutput.horizontalAlignment = HorizontalAlignmentOptions.Left;
                _textOutput.verticalAlignment = VerticalAlignmentOptions.Top;
                playerSprite.color = _dialogCharacterColor;
                //playerSprite.sprite = _playerSprites[ChangeState(characterState)];
                npcSprite.color = _grayColor;
                creatureSprite.color = _grayColor;
                break;
            case "NPC":
                if (!_npc.activeSelf)
                {
                    _npc.SetActive(true);
                    FadeUtlity.Instance.CallFade(0.5f, _npc, EGameObjectType.UI, EFadeType.FadeOut);
                }
                _textOutput.horizontalAlignment = HorizontalAlignmentOptions.Left;
                _textOutput.verticalAlignment = VerticalAlignmentOptions.Top;
                playerSprite.color = _grayColor;
                npcSprite.color = _dialogCharacterColor;
                //npcSprite.sprite = _npcSprites[0];
                creatureSprite.color = _grayColor;
                break;
            case "Creature":
                if (!_creature.activeSelf)
                {
                    _creature.SetActive(true);
                    FadeUtlity.Instance.CallFade(0.5f, _creature, EGameObjectType.UI, EFadeType.FadeOut);
                }
                _textOutput.horizontalAlignment = HorizontalAlignmentOptions.Right;
                _textOutput.verticalAlignment = VerticalAlignmentOptions.Top;
                playerSprite.color = _grayColor;
                npcSprite.color = _grayColor;
                creatureSprite.color = _dialogCharacterColor;
                //creatureSprite.sprite = _creatureSprites[0];
                break;
            case "Narrator":
                _textOutput.horizontalAlignment = HorizontalAlignmentOptions.Center;
                _textOutput.verticalAlignment = VerticalAlignmentOptions.Middle;
                playerSprite.color = _grayColor;
                npcSprite.color = _grayColor;
                creatureSprite.color = _grayColor;
                break;
            default:
                break;
        }
        yield return null;
    }

    private int ChangeState(string state)
    {
        switch (state)
        {
            case "IDLE":
                return 0;
            case "SAD":
                return 1;
            case "SMILE":
                return 2;
            case "PAIN":
                return 3;
        }

        return -1;
    }

    private IEnumerator ChangeScene()
    {
        while (true)
        {
            if (!IsTyping)
            {
                _icon.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SceneManager.LoadScene("GameScene");
                }
            }
            yield return null;
        }
    }
}
