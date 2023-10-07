using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class StorySceneManager : Singleton<StorySceneManager>
{
    [SerializeField] private GameObject _fadePenal;
    [SerializeField] private float _fadeTime;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _npc;
    [SerializeField] private GameObject _creature;
    [SerializeField] private Sprite[] _playerSprites;
    [SerializeField] private Sprite[] _npcSprites;
    [SerializeField] private Sprite[] _creatureSprites;
    private Queue<string> _dialogQueue;
    private Queue<string> _characterQueue;
    private Queue<string> _stateQueue;
    private Color _dialogCharacterColor;
    private Color _grayColor;
    

    [Header("Dialog")]
    [SerializeField] private TextMeshProUGUI _textOutput;
    [SerializeField] private float _textDelayTime;
    private int _typingCount = 0;
    public bool IsTyping { get; set; }

    private bool _isStoryStart;
    // Start is called before the first frame update
    void Start()
    {
        IsTyping = false;
        List<Dictionary<string, object>> csvRead = CSVReader.Read("Stage1Story");
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
            if (!IsTyping && Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log($"character : {_characterQueue.Peek()}, state : {_stateQueue.Peek()}, dialog : {_dialogQueue.Peek()}");
                Debug.Log($"dialogCount : {_dialogQueue.Count}, charCount : {_characterQueue.Count}, stateCount : {_stateQueue.Count}");
                yield return StartCoroutine(ChangeCharacter(_characterQueue.Dequeue(), _stateQueue.Dequeue()));
                TypingUtility.Instance.Typing(_dialogQueue.Dequeue(), _textOutput);
                _typingCount++;
                //yield return new WaitForSeconds(_textDelayTime);
            }
            yield return null;
        }
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
                playerSprite.color = _dialogCharacterColor;
                //playerSprite.sprite = _playerSprites[0];
                npcSprite.color = _grayColor;
                creatureSprite.color = _grayColor;
                break;
            case "NPC":
                if (!_npc.activeSelf)
                {
                    _npc.SetActive(true);
                    FadeUtlity.Instance.CallFade(0.5f, _npc, EGameObjectType.UI, EFadeType.FadeOut);
                }
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
                playerSprite.color = _grayColor;
                npcSprite.color = _grayColor;
                creatureSprite.color = _dialogCharacterColor;
                //creatureSprite.sprite = _creatureSprites[0];
                break;
            case "Narrator":
                
                playerSprite.color = _grayColor;
                npcSprite.color = _grayColor;
                creatureSprite.color = _grayColor;
                break;
            default:
                break;
        }
        yield return null;
    }
}
