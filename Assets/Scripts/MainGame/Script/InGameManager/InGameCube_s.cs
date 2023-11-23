using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class InGameCube_s : Singleton<InGameCube_s>, IInGame //data
{
    [Header("cube data")]
    [SerializeField] private Transform _gameCubeTsf;
    [SerializeField] private GameObject _cubeSideUI;
    public ECubeFace curFace { get; private set; }
    private Queue<ERotatePosition> _rotateQueue;
    private ERotatePosition _rotateTarget;
    [SerializeField] private Transform _rotateSpriteRendererTsf;
    private SpriteRenderer _rotateSpriteRenderer;
    [SerializeField] private Sprite _rotateSprite;
    [SerializeField] private float _rotateTime;
    private WaitForEndOfFrame _waitUpdate;
    private void Start()
    {
        _rotateQueue = new Queue<ERotatePosition>();
        _waitUpdate = new WaitForEndOfFrame();
        _rotateSpriteRenderer = _rotateSpriteRendererTsf.GetComponent<SpriteRenderer>();
    }
}
public partial class InGameCube_s//game system
{
    public void InGameBind()
    {
        InGameFunBind_s.Instance.EgameStart += GameStart;
        InGameFunBind_s.Instance.EgamePlay += GamePlay;
        InGameFunBind_s.Instance.EgameEnd += GameEnd;
        InGameFunBind_s.Instance.EmoveNextBit += MoveNextBit;
        InGameFunBind_s.Instance.EchangeInGameState += ChangeInGameStatus;
    }
    public void GameStart()
    {
        _rotateQueue.Clear();
        _rotateQueue.Enqueue(ERotatePosition.RIGHT); //need fun
        _rotateQueue.Enqueue(ERotatePosition.UP);
        _rotateQueue.Enqueue(ERotatePosition.UP);
        _rotateQueue.Enqueue(ERotatePosition.UP);
        _rotateQueue.Enqueue(ERotatePosition.RIGHT);
        _rotateTarget = ERotatePosition.NONE;
    }
    public void GamePlay()
    {
        _gameCubeTsf.localEulerAngles = Vector3.zero;
        _cubeSideUI.SetActive(true);
        _rotateSpriteRenderer.sprite = null;
    }
    public void GameEnd()
    {
        StopAllCoroutines();
    }
    public void MoveNextBit(EInGameStatus curInGameStatus)
    {

    }
    public void ChangeInGameStatus(EInGameStatus changeTarget) //change to changeTarget
    {
        switch (changeTarget)
        {
            case EInGameStatus.SHOWPATH:
                _rotateTarget = ERotatePosition.NONE;
                _cubeSideUI.SetActive(true);
                break;
            case EInGameStatus.PLAYERMOVE:
                GetFace();
                break;
            case EInGameStatus.CUBEROTATE:
                _cubeSideUI.SetActive(false);
                if (_rotateQueue.Count != 0)
                {
                    _rotateTarget = _rotateQueue.Dequeue();
                    switch (_rotateTarget)
                    {
                        case ERotatePosition.UP:
                            _rotateSpriteRendererTsf.localEulerAngles = new Vector3(0,0,-90);
                            break;
                        case ERotatePosition.DOWN:
                            _rotateSpriteRendererTsf.localEulerAngles = new Vector3(0, 0, 90);
                            break;
                        case ERotatePosition.LEFT:
                            break;
                        case ERotatePosition.RIGHT:
                            _rotateSpriteRendererTsf.localEulerAngles = new Vector3(0, 0, 180);
                            break;
                    }
                }
                else
                {
                    GetRandomeRotate();
                }
                StartCoroutine(ShowRotateImage());
                StartCoroutine(RotateTimeLock(DataConverter.GetERotatePositionToVec3(_rotateTarget)));
                break;
            case EInGameStatus.TIMEWAIT:
                break;
            default:
                break;
        }
    }
}
public partial class InGameCube_s //rotate
{
    public bool RotateCube(Vector3 rotateposition)
    {
        if (DataConverter.GetERotatePositionToVec3(_rotateTarget)==rotateposition)
        {
            return true;
        }
        return false;
    }
    private IEnumerator RotateTimeLock(Vector3 rotateposition)
    {
        DoremiUI_s.Instance.SingleDoremiAnimation("ready",true);
        while (InGameManager_s.Instance.curInGameStatus == EInGameStatus.CUBEROTATE)
        {
            yield return _waitUpdate;
        }
        CubeUI_s.Instance.ShowEffect(curFace);
        float startTime = InGameMusicManager_s.Instance.musicPosition;
        float waitTime = InGameManager_s.Instance.beatFreezeCount * InGameMusicManager_s.Instance.secPerBeat;
        while (InGameMusicManager_s.Instance.musicPosition-startTime<=waitTime)
        {
            yield return _waitUpdate;
        }
        StartCoroutine(ObjectAction.RotateObj(_gameCubeTsf.gameObject, rotateposition, _rotateTime));
    }
    IEnumerator ShowRotateImage()
    {
        _rotateSpriteRenderer.sprite = _rotateSprite;
        float waitTime = InGameManager_s.Instance.beatFreezeCount * InGameMusicManager_s.Instance.secPerBeat;
        StartCoroutine(ObjectAction.ImageFade(_rotateSpriteRenderer, InGameMusicManager_s.Instance.secPerBeat, false,1,0, InGameManager_s.Instance.beatFreezeCount));
        float startTime = InGameMusicManager_s.Instance.musicPosition;
        while (InGameMusicManager_s.Instance.musicPosition - startTime <= waitTime)
        {
            yield return _waitUpdate;
        }
        _rotateSpriteRenderer.sprite = null;
    }
    private void GetRandomeRotate()
    {
        _rotateTarget = (ERotatePosition)Enum.GetValues(typeof(ERotatePosition)).GetValue(UnityEngine.Random.Range(1, Enum.GetValues(typeof(ERotatePosition)).Length));
    }
    private void GetFace()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(1920 / 2, 1080 / 2, 0));
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        curFace = (ECubeFace)Enum.Parse(typeof(ECubeFace), hit.transform.name);
    }
    public IEnumerator QuakeCube(float rotateTime)
    {
        Vector3 rotateTarget = new Vector3(UnityEngine.Random.Range(-150, 150) * 0.01f, UnityEngine.Random.Range(-150, 150) * 0.01f, UnityEngine.Random.Range(-150, 150) * 0.01f);
        StartCoroutine(ObjectAction.RotateObj(_gameCubeTsf.gameObject, rotateTarget, rotateTime));
       float startTime = InGameMusicManager_s.Instance.musicPosition;
        while (InGameMusicManager_s.Instance.musicPosition-startTime<=rotateTime)
        {
            yield return _waitUpdate;
        }
        rotateTarget = rotateTarget * -1;
        StartCoroutine(ObjectAction.RotateObj(_gameCubeTsf.gameObject, rotateTarget, rotateTime));
    }
}