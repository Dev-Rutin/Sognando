using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using UnityEngine.UI;

interface IUI
{
    public void UIOpen();
    public void UIClose();
    public void UIPause();

    public void UpdateCombo(int changevalue);
    public void UpdateScore(int changevalue);
    public void UpdatePlayerHP(int changevalue);
    public void UpdateEnemyHP(int changevalue);
}

public partial class DivideCube_s : MonoBehaviour, IUI  //Display
{
    //main system
    private Dictionary<KeyCode, Action> _otherKeyBinds;
    private bool _isBeatMusicChecked;
    private List<EEnemyMode> _curEnemyMods;
    private Dictionary<EEnemyMode, Action> _enemyModBinds;
    public TimeSpan curMainGameTime;
    private bool _isHPDown;
    //cube
    private Dictionary<KeyCode, Action> _cubeKeyBinds;
    private CubeData[,] _cubeDatas; //x,y
    private Queue<ERotatePosition> _rotateQueue;
    private Dictionary<ECurCubeFace, bool> _curCubeSideStatus; 
    //player
    private Dictionary<KeyCode, Action> _playerKeyBinds;
    private Dictionary<KeyCode, Vector2> _playerMoveData;
    private Queue<Vector2> _playerMovePathQueue;
    private List<Vector2> _movePathCheckList;
    private Queue<GameObject> _curMovePathShowObj;
    private Dictionary<GameObject, ELineAttackMode> _curLinkLineAttackDic;
    private Queue<GameObject> _curLinkLIneAttackQueue;
    //time !!!!!TIME : defulat was seconds!!!!!
    private WaitForSeconds _GameWaitWFS;
    public int _speedLoader;
    private float _beatCount;

    [Header("Required Value")]
    //main system
    //[SerializeField] private Transform _buttonsTsf;
    [SerializeField] private AudioManager_s _audioManager;
    [SerializeField] private Transform _bgmSlider;
    [SerializeField] private Transform _ShowTextTsf;
    [SerializeField] private Transform _playerHPOffTsf;
    [SerializeField] private Transform _enemyHPTsf;
    [SerializeField] private Transform _enemyATKOnTsf;
    [SerializeField] private float _beatEndScaleX;
    [SerializeField] private float _beatEndScaleY;
    [SerializeField] private Transform _scoreTsf;
    [SerializeField] private Transform _comboTsf;
    [SerializeField] private EStage _curStage;
    [SerializeField] public AudioClip beatClip;
    [SerializeField] private GameObject _audioManagerObj;
    [SerializeField] private GameObject _enemyImageObj;
    [SerializeField] private GameObject _playerImageObj;
    [SerializeField] private GameObject _cubeEffectObj;
    [SerializeField] private GameObject _enemyHitEffectObj;
    [SerializeField] private GameObject _playerAttackEffectObj;
    //cube
    [SerializeField] private GameObject _gameCubeObj;
    [SerializeField] private Transform _cubeSizeTsf;
    [SerializeField] private Transform _rotateImageTsf;
    [SerializeField] private ECubeMode _curCubeMode;
    [SerializeField] private Material _cubeMaterial;
    [SerializeField] private Material _cubeSliceMaterial;
    //player
    [SerializeField] private GameObject _playerObj;
    //time
    [SerializeField] private Transform _beatTsf;
    [Header("Required Value(enemy attack)")]
    //enemy attack
    [SerializeField] private Transform _movePathTsf;
    [SerializeField] private GameObject _pathSampleObj;
    [SerializeField] private Transform _coinsTsf;
    [SerializeField] private GameObject _coinSampleObj;
    [SerializeField] private Transform _fireTsf;
    [SerializeField] private GameObject _fireSampleObj;
    [SerializeField] private Transform _blockTsf;
    [SerializeField] private GameObject _blockSampleObj;
    [SerializeField] private Transform _lineAttackTsf;
    [SerializeField] private GameObject _lineAttackSampleObj;
    [SerializeField] private Transform _linkLineAttackTsf;
    [Header("Only Display")]
    //main system
    [SerializeField] public EDivideGameStatus curGameStatus;
    [SerializeField] private bool _IsInput;
    [SerializeField] private int _curPlayerHP;
    [SerializeField] private int _curEnemyHP;
    [SerializeField] private int _curEnemyATKGauge;
    [SerializeField] private int _EnemyATKGaugeCount;
    [SerializeField] private EInGameStatus _curInGameStatus;
    [SerializeField] private int _beatTimeCount;
    [SerializeField] private float _curAnimationTime;
    [SerializeField] private bool _IsPlayerMoveEnd;
    [SerializeField] private int _score;
    [SerializeField] private int _combo;
    [SerializeField] public AudioClip curMusicClip;
    [SerializeField] private bool _isBeatChecked;
    //cube
    [SerializeField] private ECurCubeFace _curSideName;
    [SerializeField] private ERotatePosition _rotateTarget;
    //player
    [SerializeField] private Vector2 _playerPos;
    //enemy attack
    [SerializeField] private ELineAttackMode _curLineAttackMod;

    [Header("Changeable Value")]
    //main system
    [SerializeField] private int _playerMaxHP;
    [SerializeField] private int _enemyMaxHP;
    [SerializeField] private float _beatJudgeMax;
    [SerializeField] private float _beatJudgeMin;
    [SerializeField] private int _enemyAttackGaugeMax;
    //cube
    [SerializeField] private float _defaultMovingTime;
    [SerializeField] private int _arrX;
    [SerializeField] private int _arrY;
    //time
    [SerializeField] private float _gameWait;
    [SerializeField] private float _bpm;
}
public partial class DivideCube_s : MonoBehaviour, IUI //main system
{
    private void Awake()
    {
        //main system
        _otherKeyBinds = new Dictionary<KeyCode, Action>();
        _curEnemyMods = new List<EEnemyMode>();
        _enemyModBinds = new Dictionary<EEnemyMode, Action>();
        //cube
        _cubeKeyBinds = new Dictionary<KeyCode, Action>();
        _cubeDatas = new CubeData[_arrX, _arrY];
        _rotateQueue = new Queue<ERotatePosition>();
        _curCubeSideStatus = new Dictionary<ECurCubeFace, bool>();
        //player
        _playerKeyBinds = new Dictionary<KeyCode, Action>();
        _playerMoveData = new Dictionary<KeyCode, Vector2>();
        _playerMovePathQueue = new Queue<Vector2>();
        _movePathCheckList = new List<Vector2>();
        _curMovePathShowObj = new Queue<GameObject>();
        _curLinkLineAttackDic = new Dictionary<GameObject, ELineAttackMode>();
        _curLinkLIneAttackQueue = new Queue<GameObject>();
        DefaultDataSetting();
        DefaultValueSetting();
        TimeSetting();
        BindSetting();
        curGameStatus = EDivideGameStatus.STARTWAITTING;
        _playerImageObj.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "ready", true);
        //ButtonBind();
        //only Test
        //UIOpen();
    }
    private void DefaultDataSetting()
    {
    }
    private void DefaultValueSetting()
    {
        _beatJudgeMax = _beatJudgeMax == 0 ? 0.99f : _beatJudgeMax;
        _beatJudgeMin = _beatJudgeMin == 0 ? 0.50f : _beatJudgeMin;
        _defaultMovingTime = _defaultMovingTime == 0 ? 0.1f : _defaultMovingTime;
        _gameWait = _gameWait == 0 ? 0.05f : _gameWait;
        _speedLoader = _speedLoader == 0 ? 1 : _speedLoader;
        _bpm = _bpm == 0 ? 0.6f : _bpm;
        _playerMaxHP = _playerMaxHP == 0 ? 6 : _playerMaxHP;
        _enemyMaxHP = _enemyMaxHP == 0 ? 6 : _enemyMaxHP;
        _arrX = _arrX == 0 ? 4 : _arrX;
        _arrY = _arrY == 0 ? 4 : _arrY;
        _enemyAttackGaugeMax = _enemyAttackGaugeMax== 0 ? 3 : _enemyAttackGaugeMax;
        float xChange = _cubeSizeTsf.GetComponent<RectTransform>().rect.width / _arrX;
        float yChange = _cubeSizeTsf.GetComponent<RectTransform>().rect.height / _arrY;
        for (int i=0;i<_arrX;i++)
        {
            for(int j=0;j<_arrY;j++)
            {
                _cubeDatas[i, j] = new CubeData(new Vector2(i,j),new Vector2(-1*(xChange+_playerObj.GetComponent<RectTransform>().rect.width)-6 + xChange * i, yChange + _playerObj.GetComponent<RectTransform>().rect.height+6 - yChange * j));
            }
        }
    }
    private void BindSetting()
    {
        KeyBind(ref _cubeKeyBinds, KeyCode.LeftArrow, () => RotateCube(new Vector3(0, -90, 0)));
        KeyBind(ref _cubeKeyBinds, KeyCode.RightArrow, () => RotateCube(new Vector3(0, 90, 0)));
        KeyBind(ref _cubeKeyBinds, KeyCode.UpArrow, () => RotateCube(new Vector3(-90, 0, 0)));
        KeyBind(ref _cubeKeyBinds, KeyCode.DownArrow, () => RotateCube(new Vector3(90, 0, 0)));

        KeyBind(ref _playerKeyBinds, KeyCode.LeftArrow, () => MovePlayer(Vector2.left));
        _playerMoveData.Add(KeyCode.LeftArrow, Vector2.left);
        KeyBind(ref _playerKeyBinds, KeyCode.RightArrow, () => MovePlayer(Vector2.right));
        _playerMoveData.Add(KeyCode.RightArrow, Vector2.right);
        KeyBind(ref _playerKeyBinds, KeyCode.UpArrow, () => MovePlayer(new Vector2(0, -1)));
        _playerMoveData.Add(KeyCode.UpArrow, new Vector2(0, -1));
        KeyBind(ref _playerKeyBinds, KeyCode.DownArrow, () => MovePlayer(new Vector2(0, 1)));
        _playerMoveData.Add(KeyCode.DownArrow, new Vector2(0, 1));

        KeyBind(ref _otherKeyBinds, KeyCode.Space, () => UIPause());

        KeyBind(ref _enemyModBinds, EEnemyMode.PATH, PathAction);
        KeyBind(ref _enemyModBinds, EEnemyMode.COIN, CoinAction);
        KeyBind(ref _enemyModBinds, EEnemyMode.BLOCK, BlockAction);
        KeyBind(ref _enemyModBinds, EEnemyMode.LINEATTACK, LineAttackAction);
        KeyBind(ref _enemyModBinds, EEnemyMode.LINKLINEATTACK, LinkLineAttackAction);
    }
    private void KeyBind(ref Dictionary<KeyCode, Action> binddic, KeyCode keycode, Action action)
    {
        if (!_otherKeyBinds.ContainsKey(keycode) && !binddic.ContainsKey(keycode))
        {
            binddic.Add(keycode, action);
        }
    }
    private void KeyBind<T>(ref Dictionary<T, Action> binddic, T mod, Action action)
    {
        if (!binddic.ContainsKey(mod))
        {
            binddic.Add(mod, action);
        }
    }
    /*private void ButtonBind()
    {
        foreach (Transform child in _buttonsTsf)
        {
            child.GetComponent<Button>().onClick.AddListener((UnityAction)Delegate.CreateDelegate(typeof(UnityAction), this, child.name));
        }
    }*/
    public void MusicSetting(string name)
    {

        if (curGameStatus == EDivideGameStatus.STARTWAITTING)
        {
            curMusicClip = Resources.Load<AudioClip>("ParkMyungGue\\Music\\" + name);
        }
    }
    public void UIOpen()
    {
        curGameStatus =EDivideGameStatus.STARTWAITTING;
        _playerImageObj.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "ready", true);
    }
    public void UIPause()
    {
        if (curGameStatus == EDivideGameStatus.PAUSE)
        {
            curGameStatus = EDivideGameStatus.PLAYING;
            _audioManager.AudioUnPause(curMainGameTime);
        }
        else
        {
            if (curGameStatus == EDivideGameStatus.PLAYING)
            {
                curGameStatus = EDivideGameStatus.PAUSE;
                _audioManager.AudioPause();
            }
        }
    }
    public void UIClose()
    {
        curGameStatus = EDivideGameStatus.NONE;
    }
    float lastbeat;
    float beatIncrease;
    private void Update()
    {
        if (curMusicClip != null)
        {
            if (_audioManager.curMainMusicPosition <= curMusicClip.length && curGameStatus == EDivideGameStatus.PLAYING || curGameStatus == EDivideGameStatus.PAUSE)
            {
                _beatCount = (float)(_audioManager.curMainMusicPosition/_bpm) - (int)(_audioManager.curMainMusicPosition/_bpm);
                _bgmSlider.GetComponent<Slider>().value = (float)_audioManager.curMainMusicPosition / curMusicClip.length;
                BeatShow();
                /*if(_beatCount<=0.95f&&_beatCount>=0.92f&&_isBeatMusicChecked)
                {
                    //_audioManager.AudioPlay(beatClip);
                    _isBeatMusicChecked = false;
                }*/
                if (_beatCount <= 0.99f && _beatCount >= 0.70f && _isBeatChecked)
                {
                    _isBeatChecked = false;
                }
                if (_beatCount >= 0.05f && _beatCount <= 0.2f && !_isBeatChecked)
                {
                    MoveNextBeat();
                    _isBeatChecked = true;
                    //_isBeatMusicChecked = true;
                }
            }
            if (IsPlayerMove)
            {
                _playerObj.transform.localPosition = Vector2.Lerp(playerMoveStartPos, playerMoveEndPos, (_audioManager.curMainMusicPosition - playerMoveStartTime)*1/_defaultMovingTime);
                if (_curEnemyMods.Contains(EEnemyMode.PATH) && _curMovePathShowObj.Count != 0)
                {
                    _movePathTsf.GetComponent<LineRenderer>().SetPosition(0, _playerObj.transform.position);
                }
            }
            if (IsCubeRotate)
            {
                float curRotateValue = Mathf.Abs(rotatedivide.x + rotatedivide.y + rotatedivide.z) / (1 / Time.deltaTime * _defaultMovingTime);
                    _gameCubeObj.transform.RotateAround(_gameCubeObj.transform.position, rotatedivide, curRotateValue);
                rotateIncrease += curRotateValue;
          
            }
        }
        if (curGameStatus == EDivideGameStatus.PLAYING)
        {
            if (Input.anyKeyDown)
            {
                foreach (var dic in _otherKeyBinds)
                {
                    if (Input.GetKey(dic.Key))
                    {
                        dic.Value();
                    }
                }
                if (!_IsInput && _curInGameStatus != EInGameStatus.TIMEWAIT)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        return;
                    }
                    if ( _beatCount>=_beatJudgeMin &&_beatCount <= _beatJudgeMax)
                    {
                        if (_curCubeMode == ECubeMode.ROTATE && _rotateTarget != ERotatePosition.NONE)
                        {
                            foreach (var dic in _cubeKeyBinds)
                            {
                                if (Input.GetKey(dic.Key)&&!_IsInput)
                                {                                  
              
                                    _cubeKeyBinds[dic.Key]();
                                    _IsInput = true;
                                    BeatJudgement();
                                }
                            }
                        }
                        else if (_curCubeMode == ECubeMode.MAINTAIN && _curInGameStatus == EInGameStatus.PLAYERMOVE)
                        {
                            foreach (var dic in _playerKeyBinds)
                            {
                                if (Input.GetKey(dic.Key)&&!_IsInput)
                                {
                          
                                    _IsInput = true;
                                    _playerKeyBinds[dic.Key]();
                                }
                            }
                        }
                    }
                    else if (_curInGameStatus == EInGameStatus.PLAYERMOVE)
                    {
                        PlayerHPDown("beat miss");
                        _IsInput = true;
                    }
                }
            }
        }
        else
        {
            if (Input.anyKeyDown)
            {
                foreach (var dic in _otherKeyBinds)
                {
                    if (Input.GetKey(dic.Key))
                    {
                        dic.Value();
                    }
                }
            }
        }
    }
    public void BeatJudgement()
    {
        if (_beatCount <= _beatJudgeMax - 0.1f && _beatCount >= _beatJudgeMin + 0.1f)
        {
            //perfect
            UpdateCombo(1);
            UpdateScore(100);
            StageDataController.Instance.perfectCount++;
        }
        else
        {
            UpdateCombo(1);
            UpdateScore(50);
            //good
            StageDataController.Instance.goodCount++;
        }
    }
    private void GameStart()
    {
        if (curGameStatus == EDivideGameStatus.STARTWAITTING)
        {
            //stage data controller
            StageDataController.Instance.isClear = false;
            StageDataController.Instance.maxCombo = 0;
            StageDataController.Instance.score = 0;
            StageDataController.Instance.perfectCount = 0;
            StageDataController.Instance.goodCount = 0;
            StageDataController.Instance.missCount = 0;
            //main system
            _cubeEffectObj.GetComponent<ParticleSystem>().Stop();
            _playerObj.transform.Find("Effect").GetComponent<ParticleSystem>().Stop();
            _enemyHitEffectObj.GetComponent<ParticleSystem>().Stop();
            _playerAttackEffectObj.GetComponent<ParticleSystem>().Stop();
            _gameCubeObj.GetComponent<MeshRenderer>().material = _cubeSliceMaterial;
            //_buttonsTsf.Find("GameStart").gameObject.SetActive(false);
            MusicSetting(_curStage.ToString());
            curMainGameTime = new TimeSpan(0, 0, 0);
            curGameStatus = EDivideGameStatus.PLAYING;
            _curInGameStatus = EInGameStatus.SHOWPATH;
            _curPlayerHP = 0;
            UpdatePlayerHP(_playerMaxHP);
            _curEnemyHP = 0;
            UpdateEnemyHP(_enemyMaxHP);
            _curEnemyATKGauge = 0;
            UpdateEnemyATKGauge(0);
            _EnemyATKGaugeCount = 0;
            _IsInput = false;
            _beatTimeCount = 0;
            _isBeatChecked = false;
            _isBeatMusicChecked = false;
            _score = 0;
            _combo = 0;
            //cube
            _gameCubeObj.transform.localEulerAngles = Vector3.zero;
            GetCubeImage();
            _curCubeMode = ECubeMode.MAINTAIN;
            //player
            _playerPos = Vector2.zero;
            _playerObj.transform.localPosition = _cubeDatas[(int)_playerPos.x, (int)_playerPos.y].transform;
            //test
            _curEnemyMods.Add(EEnemyMode.COIN);
            _curEnemyMods.Add(EEnemyMode.LINKLINEATTACK);
            _curEnemyMods.Add(EEnemyMode.LINEATTACK);
            //_curEnemyMods.Add(EEnemyMode.BLOCK);
            //_curEnemyMods.Add(EEnemyMode.PATH);
            _curAnimationTime = 3f;
            _rotateQueue.Enqueue(ERotatePosition.RIGHT);
            _rotateQueue.Enqueue(ERotatePosition.UP);
            _rotateQueue.Enqueue(ERotatePosition.UP);
            _rotateQueue.Enqueue(ERotatePosition.UP) ;
            _rotateQueue.Enqueue(ERotatePosition.RIGHT);
            _curCubeSideStatus.Add(ECurCubeFace.ONE, true);
            _curCubeSideStatus.Add(ECurCubeFace.TWO, true);
            _curCubeSideStatus.Add(ECurCubeFace.THREE, true);
            _curCubeSideStatus.Add(ECurCubeFace.FOUR, true);
            _curCubeSideStatus.Add(ECurCubeFace.FIVE, true);
            _curCubeSideStatus.Add(ECurCubeFace.SIX, true);
            InitializeEnemyHP();
            lastbeat = 0;
            beatIncrease = 60/_bpm;
            _audioManager.AudioPlay(curMusicClip);
            _enemyImageObj.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0,"start",false);
            _enemyImageObj.GetComponent<SkeletonAnimation>().AnimationState.AddAnimation(0, "idle", true,0f);
            _playerImageObj.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "start", false);
            _playerImageObj.GetComponent<SkeletonAnimation>().AnimationState.AddAnimation(0, "idle", true, 0f);
        }
    }
    private void MoveNextBeat()
    {
        ShowText("");
        _isHPDown = false;
        _playerObj.GetComponent<Image>().color = new Vector4(0, 0, 0, 1f);
        _playerObj.transform.Find("Effect").GetComponent<ParticleSystem>().Stop();
        switch (_curInGameStatus)
        {
            case EInGameStatus.SHOWPATH:
                if (_curCubeMode==ECubeMode.MAINTAIN)
                {
                    if (PlayerMoveEndCheck())
                    {
                        DoEnemyMode();
                        _beatTimeCount = 2;
                    }
                    _beatTimeCount--;
                    if (_beatTimeCount == 0)
                    {
                        TransparentObjs();
                        _IsPlayerMoveEnd = PlayerMoveEndCheck();
                        _curInGameStatus = EInGameStatus.PLAYERMOVE;
                    }
                }
                break;
            case EInGameStatus.PLAYERMOVE:
                _EnemyATKGaugeCount++;
                if (_EnemyATKGaugeCount == 2)
                {
                    UpdateEnemyATKGauge(1);
                    _EnemyATKGaugeCount = 0;
                }
                _IsPlayerMoveEnd = PlayerMoveEndCheck();
                DoEnemyMode();
                if (_IsPlayerMoveEnd&&_rotateTarget==ERotatePosition.NONE)
                {
                    if (_rotateQueue.Count != 0)
                    {
                        _rotateTarget = _rotateQueue.Dequeue();
                    }
                    else
                    {
                        GetRandomeRotate();
                    }
                    StartCoroutine(RotateMode());
                }
                if (_curEnemyATKGauge >= _enemyAttackGaugeMax)
                {
                    UpdateEnemyATKGauge(_curEnemyATKGauge * -1);
                }
                if (!_IsInput)
                {
                    PlayerPositionCheck(false);
                    UpdateCombo(_combo * -1);
                }
                break;
            case EInGameStatus.TIMEWAIT:
                break;
            default:
                break;
        }
        _IsInput = false;
    }
    IEnumerator WaitTime(float time)
    {
        _gameCubeObj.GetComponent<MeshRenderer>().material = _cubeMaterial;
        MovePlayer(new Vector2(_playerPos.x * -1, _playerPos.y * -1));
        _curInGameStatus = EInGameStatus.TIMEWAIT;
        yield return new WaitForSeconds(time - 0.2f);
        _cubeEffectObj.GetComponent<ParticleSystem>().Stop();
        _cubeEffectObj.transform.Find(_curSideName.ToString()).gameObject.SetActive(false);
        EnemyHPDown();
        UpdateEnemyHP(-1);
   
        _enemyHitEffectObj.GetComponent<ParticleSystem>().Play();
        if (_curEnemyHP > 0)
        {
            StartCoroutine(RotateTimeLock(GetERotatePositionToVec3(_rotateTarget), _gameCubeObj));
            yield return new WaitForSeconds(0.2f);
            UpdatePlayerHP(1);
            _curInGameStatus = EInGameStatus.SHOWPATH;
            _gameCubeObj.GetComponent<MeshRenderer>().material = _cubeSliceMaterial;
        }
    
    }
    private void GameEnd()
    {
        if (curGameStatus != EDivideGameStatus.END)
        {
            curGameStatus = EDivideGameStatus.END;
        }
        _audioManager.AudioStop(curMusicClip);
            StopAllCoroutines();    
            //_buttonsTsf.Find("GameStart").gameObject.SetActive(true);
            _curInGameStatus = EInGameStatus.PLAYERMOVE;
            _IsPlayerMoveEnd = true;
            DoEnemyMode();
            _curEnemyMods.Clear();
            _curCubeSideStatus.Clear();
            _curInGameStatus = EInGameStatus.SHOWPATH;
            _gameCubeObj.transform.localEulerAngles= Vector3.zero;
            curGameStatus = EDivideGameStatus.STARTWAITTING;
            if (_rotateTarget != ERotatePosition.NONE)
            {
                _rotateImageTsf.Find(_rotateTarget.ToString()).gameObject.SetActive(false);
            }
            _rotateTarget = ERotatePosition.NONE;
            _rotateQueue.Clear();
            _curCubeMode = ECubeMode.MAINTAIN;
        _cubeEffectObj.GetComponent<ParticleSystem>().Stop();
        _playerObj.transform.Find("Effect").GetComponent<ParticleSystem>().Stop();
        _enemyHitEffectObj.GetComponent<ParticleSystem>().Stop();
        _playerAttackEffectObj.GetComponent<ParticleSystem>().Stop();
        _playerImageObj.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "ready", true);
    }
    IEnumerator GameOver()
    {
        curGameStatus = EDivideGameStatus.END;
        yield return new WaitForSeconds(3f);
        GameEnd();
    }
    public void TimeSetting()
    {
        _GameWaitWFS = new WaitForSeconds(_gameWait / _speedLoader);
    }
    private ERotatePosition GetVec3ToERotatePosition(Vector3 position)
    {
        if(position== new Vector3(0, -90, 0))
        {
            return ERotatePosition.LEFT;
        }
        else if(position == new Vector3(0, 90, 0))
        {
            return ERotatePosition.RIGHT;
        }
        else if (position == new Vector3(-90,0, 0))
        {
            return ERotatePosition.UP;
        }
        else if (position == new Vector3(90,0, 0))
        {
            return ERotatePosition.DOWN;
        }
        return ERotatePosition.NONE;
    }
    private Vector3 GetERotatePositionToVec3(ERotatePosition position)
    {
        switch(position)
        {
            case ERotatePosition.LEFT:
                return new Vector3(0, -90, 0);
            case ERotatePosition.RIGHT:
                return new Vector3(0, 90, 0);
            case ERotatePosition.UP:
                return new Vector3(-90, 0, 0);
            case ERotatePosition.DOWN:
                return new Vector3(90, 0, 0);
            default:
                break;
        }
        return new Vector3(0, 0, 0);
    }
    private ERotatePosition GetVec2ToERotatePosition(Vector2 pos)
    {
        if(pos == new Vector2(-1, -1 * (_arrY - 1)))
        {
            return ERotatePosition.LEFT;
        }
        else if(pos == new Vector2(1,-1 * (_arrY - 1)))
        {
            return ERotatePosition.RIGHT;
        }
        else if(pos == new Vector2(-1 * (_arrX - 1),-1))
        {
            return ERotatePosition.UP;
        }
        else if(pos == new Vector2(-1 * (_arrX - 1),1))
        {
            return ERotatePosition.DOWN;
        }
        return ERotatePosition.NONE;
    }
    private Vector2 GetERotatePositionToVec2(ERotatePosition position)
    {
        switch (position)
        {
            case ERotatePosition.LEFT:
                return new Vector2(-1, -1 * (_arrY - 1));
            case ERotatePosition.RIGHT:
                return new Vector2(1, -1 * (_arrY - 1));
            case ERotatePosition.UP:
                return new Vector2(-1 * (_arrX - 1), -1);
            case ERotatePosition.DOWN:
                return new Vector2(-1 * (_arrX - 1), 1);
            default:
                break;
        }
        return new Vector2(0,0);
    }
    
}
public partial class DivideCube_s : MonoBehaviour, IUI //UI
{
    private void ShowText(string data)
    {
        _ShowTextTsf.GetComponent<TextMeshProUGUI>().text = data;
    }
    private void BeatShow()
    {
        if (_curInGameStatus == EInGameStatus.PLAYERMOVE)
        {
            _beatTsf.GetComponent<RectTransform>().localScale = Vector2.Lerp(new Vector2(1.176471f, 1.162791f),new Vector2(_beatEndScaleX, _beatEndScaleY),_beatCount);
            if (_beatCount > 0.8f)
            {
                _beatTsf.GetComponent<Image>().color = new Vector4(0.36f, 0, 1f, 1f);
            }
            else
            {
                _beatTsf.GetComponent<Image>().color = new Vector4(1f, 1f, 1f, 1f);
            }
        }
    }
    public void UpdateCombo(int changevalue)
    {
        _combo += changevalue;
        _comboTsf.GetComponent<TextMeshProUGUI>().text = _combo.ToString();
        if (StageDataController.Instance.maxCombo<_combo)
        {
            StageDataController.Instance.maxCombo = _combo;
        }
    }
    public void UpdateScore(int changevalue)
    {
        _score += changevalue;
        _scoreTsf.GetComponent<TextMeshProUGUI>().text = _score.ToString();
        StageDataController.Instance.score = _score;
    }
    public void UpdatePlayerHP(int changevalue)
    {
        _curPlayerHP += changevalue;
        _curPlayerHP = _curPlayerHP > _playerMaxHP ? _playerMaxHP : _curPlayerHP;
        int count = 0;
        foreach (Transform data in _playerHPOffTsf)
        {
            if (count < _curPlayerHP)
            {
                data.gameObject.SetActive(false);
            }
            else
            {
                data.gameObject.SetActive(true);
            }
            count++;
        }

    }
    private void PlayerHPDown(string message)
    {
        if (!_isHPDown)
        {
            _playerImageObj.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "hit", false);
            _playerImageObj.GetComponent<SkeletonAnimation>().AnimationState.AddAnimation(0, "idle", true, 0f);
            ShowText(message);
            UpdatePlayerHP(-1);
            UpdateCombo(_combo * -1);
            if (_curPlayerHP == 0)
            {
                StartCoroutine(GameOver());
            }
            _isHPDown = true;
            _playerObj.GetComponent<Image>().color = new Vector4(0, 0, 0, 0.7f);
        }
    }
    private void InitializeEnemyHP()
    {
        foreach (var data in _curCubeSideStatus)
        {
            _enemyHPTsf.Find(data.Key.ToString()).GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
            _enemyHPTsf.Find(data.Key.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>("ParkMyungGue\\CubeSideImage\\ON\\" + data.Key.ToString());
        }
    }
    public void UpdateEnemyHP(int changevalue)
    {
        _playerAttackEffectObj.GetComponent<ParticleSystem>().Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        _enemyImageObj.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "hit", false);
        _curEnemyHP += changevalue;
        foreach (var data in _curCubeSideStatus)
        {
            if (!data.Value)
            {
                _enemyHPTsf.Find(data.Key.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>("ParkMyungGue\\CubeSideImage\\OFF\\" + data.Key.ToString());
            }
        }
        if (_curEnemyHP <= 0)
        {
            _enemyImageObj.GetComponent<SkeletonAnimation>().AnimationState.AddAnimation(0, "death", false,0f);
            StageDataController.Instance.isClear = true;
            StartCoroutine(GameOver());
        }
        else
        {
            _enemyImageObj.GetComponent<SkeletonAnimation>().AnimationState.AddAnimation(0, "idle", true, 0f);
        }
    }
    private void EnemyHPDown()
    {
        _curCubeSideStatus[_curSideName] = false;
        StageDataController.Instance.missCount++;
    }
    public void UpdateEnemyATKGauge(int changevalue)
    {
        _curEnemyATKGauge += changevalue;
        int count = 0;
        foreach (Transform data in _enemyATKOnTsf)
        {
            if(count==0)
            {
                count++;
                continue;
                
            }
            if (count < _curEnemyATKGauge+1)
            {
                data.gameObject.SetActive(true);
            }
            else
            {
                data.gameObject.SetActive(false);
            }
            count++;
        }
        /*if(_curEnemyATKGauge>=_enemyAttackGaugeMax-1)
        {
            _enemyATKOnTsf.Find("BackGround").gameObject.SetActive(true);
        }
        else
        {
            _enemyATKOnTsf.Find("BackGround").gameObject.SetActive(false);
        }*/
    }
}
public partial class DivideCube_s : MonoBehaviour, IUI // cube
{
    private void GetRandomeRotate()
    {
        _rotateTarget = (ERotatePosition)Enum.GetValues(typeof(ERotatePosition)).GetValue(UnityEngine.Random.Range(1, Enum.GetValues(typeof(ERotatePosition)).Length));
    }
    IEnumerator RotateMode()
    {
        if (curGameStatus == EDivideGameStatus.PLAYING)
        {
            _curCubeMode = ECubeMode.ROTATE;
            _rotateImageTsf.Find(_rotateTarget.ToString()).gameObject.SetActive(true);
            yield return new WaitForSeconds(_bpm - 0.02f);
            if (!_IsInput)
            {
                PlayerHPDown("Don't Rotate");
            }
            _rotateImageTsf.Find(_rotateTarget.ToString()).gameObject.SetActive(false);
            _playerImageObj.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "attack", false);
            _playerImageObj.GetComponent<SkeletonAnimation>().AnimationState.AddAnimation(0, "idle", true, 0f);
            _playerAttackEffectObj.GetComponent<ParticleSystem>().Play();
            _cubeEffectObj.transform.Find(_curSideName.ToString()).gameObject.SetActive(true);
            _cubeEffectObj.GetComponent<ParticleSystem>().Play();
            StartCoroutine(WaitTime(_curAnimationTime));
        }
    }
    private void RotateCube(Vector3 rotateposition)
    {
        /*switch(_rotateTarget)
        {
            case ERotatePosition.LEFT:
                MovePlayer(new Vector2(GetERotatePositionToVec2(_rotateTarget).x* GetERotatePositionToVec2(_rotateTarget).y,0));
                break;
            case ERotatePosition.RIGHT:
                MovePlayer(new Vector2(GetERotatePositionToVec2(_rotateTarget).x * GetERotatePositionToVec2(_rotateTarget).y, 0));
                break;
            case ERotatePosition.UP:
                MovePlayer(new Vector2(0, GetERotatePositionToVec2(_rotateTarget).x * GetERotatePositionToVec2(_rotateTarget).y));
                break;
            case ERotatePosition.DOWN:
                MovePlayer(new Vector2(0,GetERotatePositionToVec2(_rotateTarget).x * GetERotatePositionToVec2(_rotateTarget).y));
                break;
            default:
                break;
        }*/
        if (_rotateTarget != GetVec3ToERotatePosition(rotateposition))
        {
            PlayerHPDown("RotateMiss");
        }
    }
    private bool IsCubeRotate;
    private Vector3 rotatedivide;
    private Vector3 endRotatePos;
    private float rotateIncrease;
    IEnumerator RotateTimeLock(Vector3 rotateposition, GameObject targetobj)
    {
        if (_rotateTarget == GetVec3ToERotatePosition(rotateposition))
        {
            _rotateTarget = ERotatePosition.NONE;
            rotatedivide =  rotateposition;
            rotateIncrease = 0;
            IsCubeRotate = true;
            yield return new WaitForSeconds(_defaultMovingTime); 
            IsCubeRotate = false;
            targetobj.transform.RotateAround(_gameCubeObj.transform.position, rotatedivide, Mathf.Abs(rotatedivide.x + rotatedivide.y + rotatedivide.z)-rotateIncrease);
            targetobj.transform.localEulerAngles = new Vector3(MathF.Round(targetobj.transform.localEulerAngles.x), Mathf.Round(targetobj.transform.localEulerAngles.y), Mathf.Round(targetobj.transform.localEulerAngles.z));
            GetCubeImage();
            _curCubeMode = ECubeMode.MAINTAIN;
            _enemyHitEffectObj.GetComponent<ParticleSystem>().Stop();
        }
    }
    void GetCubeImage()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(1920 / 2, 1080/2, 0));
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        _curSideName =(ECurCubeFace)Enum.Parse(typeof(ECurCubeFace), hit.transform.name);
    }
}
public partial class DivideCube_s : MonoBehaviour, IUI //player
{
    private void MovePlayer(Vector2 pos)
    {
        if (PlayerMoveCheck(pos))
        {
            Vector2 previouspos = _playerPos;
            _playerPos += pos;
            _playerPos.x = _playerPos.x < 0 ? 0 : _playerPos.x;
            _playerPos.x = _playerPos.x > _arrX - 1 ? _arrX - 1 : _playerPos.x;
            _playerPos.y = _playerPos.y < 0 ? 0 : _playerPos.y;
            _playerPos.y = _playerPos.y > _arrY - 1 ? _arrY - 1 : _playerPos.y;
            if (_playerPos != previouspos)
            {
                StartCoroutine(MoveTimeLock(_cubeDatas[(int)_playerPos.x, (int)_playerPos.y].transform, _playerObj));
                if (_curCubeMode == ECubeMode.MAINTAIN)
                {
                    BeatJudgement();
                }
            }
            PlayerPositionCheck(true);
        }
    }
    private float playerMoveStartTime;
    private Vector2 playerMoveStartPos;
    private Vector2 playerMoveEndPos;
    private bool IsPlayerMove;
    IEnumerator MoveTimeLock(Vector2 moveposition, GameObject targetobj)
    {
        playerMoveStartTime = _audioManager.curMainMusicPosition;
        playerMoveStartPos = targetobj.transform.localPosition;
        playerMoveEndPos = moveposition;
        IsPlayerMove = true;    
        yield return new WaitForSeconds(_defaultMovingTime);
        IsPlayerMove = false;
        _playerObj.transform.localPosition = playerMoveEndPos;
        _playerObj.transform.Find("Effect").GetComponent<ParticleSystem>().Play();
    }
    private bool PlayerMoveCheck(Vector2 pos)
    {
        if (_curEnemyMods.Contains(EEnemyMode.PATH))
        {
            if (_playerMovePathQueue.Count != 0)
            {
                if (_playerMovePathQueue.Peek() != _playerPos + pos)
                {
                    PlayerHPDown("Move Miss!!");
                    return false;
                }
            }
        }
        if (_curEnemyMods.Contains(EEnemyMode.BLOCK))
        {
            if (_cubeDatas[(int)(_playerPos.x + pos.x), (int)(_playerPos.y + pos.y)].wall != null)
            {
                return false;
            }
        }
        return true;
    }
    private void PlayerPositionCheck(bool isMoving)
    {
        if (_curEnemyMods.Contains(EEnemyMode.COIN))
        {
            if (_cubeDatas[(int)_playerPos.x, (int)_playerPos.y].coin != null&&isMoving)
            {
                _cubeDatas[(int)_playerPos.x, (int)_playerPos.y].coincount--;
                if (_cubeDatas[(int)_playerPos.x, (int)_playerPos.y].coincount == 0)
                {
                    RemoveTargetObj("coin", (int)_playerPos.x, (int)_playerPos.y);
                }
                else
                {
                    _cubeDatas[(int)_playerPos.x, (int)_playerPos.y].coin.GetComponent<Image>().color = new Vector4(_cubeDatas[(int)_playerPos.x, (int)_playerPos.y].coin.GetComponent<Image>().color.r, _cubeDatas[(int)_playerPos.x, (int)_playerPos.y].coin.GetComponent<Image>().color.g, _cubeDatas[(int)_playerPos.x, (int)_playerPos.y].coin.GetComponent<Image>().color.b, 0.7f);
                }
            }
            if (_cubeDatas[(int)_playerPos.x, (int)_playerPos.y].fire != null)
            {
                PlayerHPDown("Fire!!!");
            }
        }
        if (_curEnemyMods.Contains(EEnemyMode.PATH))
        {
            UpdatePath();
        }
        if (_curEnemyMods.Contains(EEnemyMode.LINEATTACK))
        {
            if (_cubeDatas[(int)_playerPos.x, (int)_playerPos.y].lineAttack != null && _curLineAttackMod == ELineAttackMode.ATTACK)
            {
                PlayerHPDown("Line Attack!!!");
            }
        }
        if (_curEnemyMods.Contains(EEnemyMode.LINKLINEATTACK))
        {
            if (_cubeDatas[(int)_playerPos.x, (int)_playerPos.y].linkLineAttack != null && _curLinkLineAttackDic[_cubeDatas[(int)_playerPos.x, (int)_playerPos.y].linkLineAttack] == ELineAttackMode.ATTACK)
            {
                PlayerHPDown("Link Line Attack!!!");
            }
        }
    }
}
public partial class DivideCube_s : MonoBehaviour, IUI //ingame pattern
{
    private Queue<T> ReverseQueue<T>(Queue<T> queue)
    {
        List<T> prim = new List<T>();
        Queue<T> temp = new Queue<T>();
        foreach (var data in queue)
        {
            prim.Add(data);
        }
        for (int i = prim.Count - 1; i >= 0; i--)
        {
            temp.Enqueue(prim[i]);
        }
        return temp;
    }
    private bool PlayerMoveEndCheck()
    {
        foreach (var data in _curEnemyMods)
        {
            switch (data)
            {
                case EEnemyMode.COIN:
                    if (_coinsTsf.childCount == 0)
                    {
                        return true;
                    }
                    break;
                case EEnemyMode.PATH:
                    if (_movePathTsf.childCount == 0)
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }
        }
        return false;
    }
    private void DoEnemyMode()
    {
        var temp = new List<EEnemyMode>(_curEnemyMods);
        foreach (var data in temp)
        {
            _enemyModBinds[data]();
        }
    }
    private void TransparentObjs()
    {
        if (_curEnemyMods.Contains(EEnemyMode.COIN))
        {
            foreach (Transform data in _coinsTsf)
            {
                data.GetComponent<Image>().color = new Vector4(data.GetComponent<Image>().color.r, data.GetComponent<Image>().color.g, data.GetComponent<Image>().color.b, 0.9f);
            }
        }
        if (_curEnemyMods.Contains(EEnemyMode.PATH))
        {
            foreach (Transform data in _movePathTsf)
            {
                data.GetComponent<Image>().color = new Vector4(data.GetComponent<Image>().color.r, data.GetComponent<Image>().color.g, data.GetComponent<Image>().color.b, 0.5f);
            }
        }
    }
    private void GetRandomeObjs(string dataName, GameObject instTarget, bool isStack, Transform parent, int makeCount)
    {
        for (int i = 0; i < makeCount; i++)
        {
            bool isCreate = false;
            int block = 0;
            while (!isCreate)
            {
                if (block == 50)
                {
                    break;
                }
                int x = UnityEngine.Random.Range(0, _arrX);
                int y = UnityEngine.Random.Range(0, _arrY);
                if (new Vector2(x, y) == _playerPos || !_cubeDatas[x, y].isCanMakeCheck(isStack, dataName))
                {
                    continue;
                }
                else
                {
                    GameObject instObj = Instantiate(instTarget, parent);
                    instObj.transform.localPosition = _cubeDatas[x, y].transform;
                    TypedReference tr = __makeref(_cubeDatas[x, y]);
                    _cubeDatas[x, y].GetType().GetField(dataName).SetValueDirect(tr, instObj);
                    if(dataName=="coin")
                    {
                        _cubeDatas[x, y].coincount = 2;
                    }
                    isCreate = true;
                }
                block++;
            }
        }
    }
    private void RemoveTargetObj(string dataName, int xpos, int ypos)
    {
        TypedReference tr = __makeref(_cubeDatas[xpos, ypos]);
        object target = _cubeDatas[xpos, ypos].GetType().GetField(dataName).GetValueDirect(tr);
        if (target != null)
        {
            if (target.GetType().Equals(typeof(GameObject)))
            {
                Destroy((GameObject)target);
                _cubeDatas[xpos, ypos].GetType().GetField(dataName).SetValueDirect(tr, null);
                if (dataName == "coin")
                {
                    _cubeDatas[xpos, ypos].coincount = 0;
                }
            }
        }
    }
    private void RemoveAllTargetObj(string dataName)
    {
        for (int i = 0; i < _arrX; i++)
        {
            for (int j = 0; j < _arrY; j++)
            {
                RemoveTargetObj(dataName, i, j);
            }
        }
    }
    private void GetRandomePath()//only test
    {
        Vector2 movedPlayerPos = _playerPos;
        for (int i = 0; i < 10; i++)
        {
            _movePathCheckList.Clear();
            if (movedPlayerPos.x - 1 >= 0)
            {
                _movePathCheckList.Add(new Vector2(movedPlayerPos.x - 1, movedPlayerPos.y));
            }
            if (movedPlayerPos.x + 1 <= _arrX - 1)
            {
                _movePathCheckList.Add(new Vector2(movedPlayerPos.x + 1, movedPlayerPos.y));
            }
            if (movedPlayerPos.y - 1 >= 0)
            {
                _movePathCheckList.Add(new Vector2(movedPlayerPos.x, movedPlayerPos.y - 1));
            }
            if (movedPlayerPos.y + 1 <= _arrY - 1)
            {
                _movePathCheckList.Add(new Vector2(movedPlayerPos.x, movedPlayerPos.y + 1));
            }
            _movePathCheckList.Remove(_playerPos);
            foreach (var data in _playerMovePathQueue)
            {
                _movePathCheckList.Remove(data);
            }
            if (_movePathCheckList.Count != 0)
            {
                Vector2 enqueuedata = _movePathCheckList[UnityEngine.Random.Range(0, _movePathCheckList.Count)];
                _playerMovePathQueue.Enqueue(enqueuedata);
                movedPlayerPos = enqueuedata;
            }
            else
            {
                break;
            }
        }
    }
    private void ShowPath()
    {
        if (_playerMovePathQueue.Count != 0)
        {
            int count = 0;
            _movePathTsf.GetComponent<LineRenderer>().positionCount = _playerMovePathQueue.Count >= 5 ? 5 : _playerMovePathQueue.Count;
            _movePathTsf.GetComponent<LineRenderer>().SetPosition(count, _playerObj.transform.position);
            count++;
            foreach (var data in _playerMovePathQueue)
            {
                if (data.x != -1 * (_arrX - 1) && data.y != -1 * (_arrY - 1))
                {
                    GameObject instPath = Instantiate(_pathSampleObj, _movePathTsf);
                    instPath.transform.localPosition = _cubeDatas[(int)data.x, (int)data.y].transform;
                    if (count < 5)
                    {
                        instPath.SetActive(true);
                        _movePathTsf.GetComponent<LineRenderer>().SetPosition(count, instPath.transform.position);
                        _curMovePathShowObj.Enqueue(instPath);
                    }
                    else
                    {
                        instPath.SetActive(false);
                    }
                    count++;
                }
            }
        }
    }
    private void UpdatePath()
    {
        if (_playerMovePathQueue.Count == 0)
        {
            if (_movePathTsf.childCount != 0)
            {
                for (int i = 0; i < _movePathTsf.childCount; i++)
                {
                    Destroy(_movePathTsf.GetChild(i).gameObject);
                }
            }
            _movePathTsf.GetComponent<LineRenderer>().positionCount = 1;
        }
        else if (_curInGameStatus == EInGameStatus.PLAYERMOVE)
        {
            if (_playerPos == _playerMovePathQueue.Peek())
            {
                _playerMovePathQueue.Dequeue();
                Destroy(_curMovePathShowObj.Dequeue());
            }
            if (_curMovePathShowObj.Count == 1)
            {
                foreach (Transform data in _movePathTsf)
                {
                    if (!data.gameObject.activeSelf)
                    {
                        data.gameObject.SetActive(true);
                        _curMovePathShowObj.Enqueue(data.gameObject);
                        break;
                    }
                }
            }
            int count = 0;
            _movePathTsf.GetComponent<LineRenderer>().positionCount = _curMovePathShowObj.Count + 1;
            _movePathTsf.GetComponent<LineRenderer>().SetPosition(count, _playerObj.transform.position);
            foreach (var data in _curMovePathShowObj)
            {
                count++;
                _movePathTsf.GetComponent<LineRenderer>().SetPosition(count, data.transform.position);
            }
        }
    }
    private void PathAction()
    {
        if (_curInGameStatus == EInGameStatus.SHOWPATH)
        {
            GetRandomePath();
            ShowPath();
        }
        else
        {
            if (_IsPlayerMoveEnd)
            {
                _rotateTarget = ERotatePosition.NONE;
                _playerMovePathQueue.Clear();
                _curMovePathShowObj.Clear();
                UpdatePath();
            }
        }
    }
    private void GetRandomeCoin()
    {
        GetRandomeObjs("coin", _coinSampleObj, true, _coinsTsf, 5);
    }
    private void EndCoin()
    {
        RemoveAllTargetObj("coin");
    }
    private void GetRandomeFire()
    {
        GetRandomeObjs("fire", _fireSampleObj, false, _fireTsf, 5);
    }
    private void EndFire()
    {
        RemoveAllTargetObj("fire");
    }
    private void CoinAction()
    {
        if (_curInGameStatus == EInGameStatus.SHOWPATH)
        {
            GetRandomeCoin();
            //GetRandomeFire();
        }
        else
        {
            if (_IsPlayerMoveEnd)
            {
                EndCoin();
                //EndFire();
            }
        }
    }
    private void GetRandomeBlock()
    {
        GetRandomeObjs("wall", _blockSampleObj, false, _blockTsf, 3);
    }
    private void EndBlock()
    {
        RemoveAllTargetObj("wall");
    }
    private void BlockAction()
    {
        if (_curInGameStatus == EInGameStatus.SHOWPATH)
        {
            GetRandomeBlock();
        }
        else
        {
            if (_IsPlayerMoveEnd)
            {
                EndBlock();
            }
        }
    }
    private void GetRandomeLineAttack()
    {
        int count = UnityEngine.Random.Range(0, 2);
        if (count == 0) // row attack
        {
            int y = (int)_playerPos.y;
            for (int i = 0; i < _arrX; i++)
            {
                if (_cubeDatas[i, y].isCanMakeCheck(true, "lineAttack"))
                {
                    GameObject instObj = Instantiate(_lineAttackSampleObj, _lineAttackTsf);
                    instObj.transform.Find("Attack").gameObject.SetActive(false);
                    instObj.transform.localPosition = _cubeDatas[i, y].transform;
                    _cubeDatas[i, y].lineAttack = instObj;
                }
            }
        }
        else // columns attack
        {
            int x = (int)_playerPos.x;
            for (int i = 0; i < _arrY; i++)
            {
                if (_cubeDatas[x, i].isCanMakeCheck(true, "lineAttack"))
                {
                    GameObject instObj = Instantiate(_lineAttackSampleObj, _lineAttackTsf);
                    instObj.transform.Find("Attack").gameObject.SetActive(false);
                    instObj.transform.localPosition = _cubeDatas[x, i].transform;
                    _cubeDatas[x, i].lineAttack = instObj;
                }
            }
        }
        _curLineAttackMod = ELineAttackMode.SHOW;
    }
    private void EndRandomeLineAttack()
    {
        RemoveAllTargetObj("lineAttack");
        _curLineAttackMod = ELineAttackMode.NONE;
    }
    private void LineAttackAction()
    {
        if (_curInGameStatus == EInGameStatus.SHOWPATH)
        {
            //GetRandomeLineAttack();
        }
        else
        {
            if (_IsPlayerMoveEnd)
            {
                EndRandomeLineAttack();
            }
            else
            {
                if (_curEnemyATKGauge >= _enemyAttackGaugeMax)
                {
                    GetRandomeLineAttack();
                }
                else if (_curLineAttackMod == ELineAttackMode.SHOW)
                {
                    _curLineAttackMod = ELineAttackMode.ATTACK;
                    foreach (Transform data in _lineAttackTsf)
                    {
                        data.Find("Attack").gameObject.SetActive(true);

                    }
                    if (_IsInput)
                    {
                        PlayerPositionCheck(false);
                    }
                }
                else if (_curLineAttackMod == ELineAttackMode.ATTACK)
                {
                    EndRandomeLineAttack();
                }
            }
        }
    }
    private void GetRandomeLinkLineAttack()
    {
        int count = UnityEngine.Random.Range(0, 2);
        int secondcount = UnityEngine.Random.Range(0, 2);
        if (count == 0) // row attack
        {
            int y = (int)_playerPos.y;
            if (secondcount == 0)
            {
                for (int i = 0; i < _arrX; i++)
                {
                    if (_cubeDatas[i, y].isCanMakeCheck(true, "linkLineAttack"))
                    {
                        GameObject instObj = Instantiate(_lineAttackSampleObj, _linkLineAttackTsf);
                        instObj.transform.Find("Attack").gameObject.SetActive(false);
                        instObj.transform.localPosition = _cubeDatas[i, y].transform;
                        _cubeDatas[i, y].linkLineAttack = instObj;
                        _curLinkLineAttackDic.Add(instObj, ELineAttackMode.NONE);
                        instObj.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = _arrX - 1; i >= 0; i--)
                {
                    if (_cubeDatas[i, y].isCanMakeCheck(true, "linkLineAttack"))
                    {
                        GameObject instObj = Instantiate(_lineAttackSampleObj, _linkLineAttackTsf);
                        instObj.transform.Find("Attack").gameObject.SetActive(false);
                        instObj.transform.localPosition = _cubeDatas[i, y].transform;
                        _cubeDatas[i, y].linkLineAttack = instObj;
                        _curLinkLineAttackDic.Add(instObj, ELineAttackMode.NONE);
                        instObj.gameObject.SetActive(false);
                    }
                }
            }
        }
        else // columns attack
        {
            int x = (int)_playerPos.x;
            if (secondcount == 0)
            {
                for (int i = 0; i < _arrY; i++)
                {

                    if (_cubeDatas[x, i].isCanMakeCheck(true, "linkLineAttack"))
                    {
                        GameObject instObj = Instantiate(_lineAttackSampleObj, _linkLineAttackTsf);
                        instObj.transform.Find("Attack").gameObject.SetActive(false);
                        instObj.transform.localPosition = _cubeDatas[x, i].transform;
                        _cubeDatas[x, i].linkLineAttack = instObj;
                        _curLinkLineAttackDic.Add(instObj, ELineAttackMode.NONE);
                        instObj.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = _arrY - 1; i >= 0; i--)
                {

                    if (_cubeDatas[x, i].isCanMakeCheck(true, "linkLineAttack"))
                    {
                        GameObject instObj = Instantiate(_lineAttackSampleObj, _linkLineAttackTsf);
                        instObj.transform.Find("Attack").gameObject.SetActive(false);
                        instObj.transform.localPosition = _cubeDatas[x, i].transform;
                        _cubeDatas[x, i].linkLineAttack = instObj;
                        _curLinkLineAttackDic.Add(instObj, ELineAttackMode.NONE);
                        instObj.gameObject.SetActive(false);
                    }
                }

            }
        }
    }
    private void EndRandomeLinkLineAttack()
    {
        RemoveAllTargetObj("linkLineAttack");
        _curLinkLineAttackDic.Clear();
        _curLinkLIneAttackQueue.Clear();
    }
    private void LinkLineAttackAction()
    {
        if (_curInGameStatus == EInGameStatus.SHOWPATH)
        {
            //GetRandomeLineAttack();
        }
        else
        {
            if (_IsPlayerMoveEnd)
            {
                EndRandomeLinkLineAttack();
            }
            else
            {
                if (_curEnemyATKGauge >= _enemyAttackGaugeMax && _curLinkLineAttackDic.Count == 0)
                {
                    GetRandomeLinkLineAttack();
                }
                else if (_curLinkLineAttackDic.Count != 0)
                {
                    for (int i = 0; i < _arrX; i++)
                    {
                        for (int j = 0; j < _arrY; j++)
                        {
                            if (_cubeDatas[i, j].linkLineAttack != null)
                            {
                                if (_curLinkLineAttackDic[_cubeDatas[i, j].linkLineAttack] == ELineAttackMode.ATTACK)
                                {
                                    GameObject removeTarget = _cubeDatas[i, j].linkLineAttack;
                                    _cubeDatas[i, j].linkLineAttack = null;
                                    _curLinkLIneAttackQueue.Dequeue();
                                    _curLinkLineAttackDic.Remove(removeTarget);
                                    Destroy(removeTarget);
                                }
                            }
                        }
                    }
                    foreach (Transform data in _linkLineAttackTsf)
                    {
                        if (_curLinkLIneAttackQueue.Count >= 2)
                        {
                            break;
                        }
                        if (_curLinkLineAttackDic.ContainsKey(data.gameObject))
                        {
                            switch (_curLinkLineAttackDic[data.gameObject])
                            {
                                case ELineAttackMode.NONE:
                                    data.gameObject.SetActive(true);
                                    _curLinkLineAttackDic[data.gameObject] = ELineAttackMode.SHOW;
                                    if (_curLinkLIneAttackQueue.Count == 0)
                                    {
                                        _curLinkLIneAttackQueue.Enqueue(data.gameObject);
                                        return;
                                    }
                                    else
                                    {
                                        _curLinkLIneAttackQueue.Enqueue(data.gameObject);
                                    }
                                    break;
                                case ELineAttackMode.SHOW:
                                    data.gameObject.transform.Find("Attack").gameObject.SetActive(true);
                                    _curLinkLineAttackDic[data.gameObject] = ELineAttackMode.ATTACK;
                                    break;
                                case ELineAttackMode.ATTACK:
                                    break;
                            }
                        }
                    }

                }
            }
        }
    }

    private void GetRandomeNeedleAttack()
    {
        int count = UnityEngine.Random.Range(0, 2);
        int secondcount = UnityEngine.Random.Range(0, 2);
        if (count == 0) // row attack
        {
            int y = UnityEngine.Random.Range(0,_arrY);
            if (secondcount == 0)
            {
                for (int i = 0; i < _arrX; i++)
                {
                    if (_cubeDatas[i, y].isCanMakeCheck(true, "linkLineAttack"))
                    {
                        GameObject instObj = Instantiate(_lineAttackSampleObj, _linkLineAttackTsf);
                        instObj.transform.Find("Attack").gameObject.SetActive(false);
                        instObj.transform.localPosition = _cubeDatas[i, y].transform;
                        _cubeDatas[i, y].linkLineAttack = instObj;
                        _curLinkLineAttackDic.Add(instObj, ELineAttackMode.NONE);
                        instObj.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = _arrX - 1; i >= 0; i--)
                {
                    if (_cubeDatas[i, y].isCanMakeCheck(true, "linkLineAttack"))
                    {
                        GameObject instObj = Instantiate(_lineAttackSampleObj, _linkLineAttackTsf);
                        instObj.transform.Find("Attack").gameObject.SetActive(false);
                        instObj.transform.localPosition = _cubeDatas[i, y].transform;
                        _cubeDatas[i, y].linkLineAttack = instObj;
                        _curLinkLineAttackDic.Add(instObj, ELineAttackMode.NONE);
                        instObj.gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            int x = UnityEngine.Random.Range(0, _arrX);
            for (int i = 0; i < _arrY; i++)
            {
                if (_cubeDatas[x, i].isCanMakeCheck(true, "lineAttack"))
                {
                    GameObject instObj = Instantiate(_lineAttackSampleObj, _lineAttackTsf);
                    instObj.transform.Find("Attack").gameObject.SetActive(false);
                    instObj.transform.localPosition = _cubeDatas[x, i].transform;
                    _cubeDatas[x, i].lineAttack = instObj;
                }
            }
        }
    }
    private void EndRandomeNeedleAttack()
    {

    }
}