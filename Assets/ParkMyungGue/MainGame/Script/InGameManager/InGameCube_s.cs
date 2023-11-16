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
    private Sprite _rotateTargetImage;
    [SerializeField] private SpriteRenderer _rotateSprite;
    [SerializeField] private Sprite _upImage;
    [SerializeField] private Sprite _downImage;
    [SerializeField] private Sprite _leftImage;
    [SerializeField] private Sprite _rightImage;
    [SerializeField] private float _rotateTime;
    private WaitForEndOfFrame _waitUpdate;
    private void Start()
    {
        _rotateQueue = new Queue<ERotatePosition>();
        _waitUpdate = new WaitForEndOfFrame();
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
        _rotateSprite.sprite = null;
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
                            _rotateTargetImage = _upImage;
                            break;
                        case ERotatePosition.DOWN:
                            _rotateTargetImage = _downImage;
                            break;
                        case ERotatePosition.LEFT:
                            _rotateTargetImage = _leftImage;
                            break;
                        case ERotatePosition.RIGHT:
                            _rotateTargetImage = _rightImage;
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
        while(InGameManager_s.Instance.curInGameStatus==EInGameStatus.CUBEROTATE)
        {
            yield return _waitUpdate;
        }
        CubeUI_s.Instance.ShowEffect(curFace);
        float startTime = InGameMusicManager_s.Instance.musicPosition;
        while (InGameMusicManager_s.Instance.musicPosition-startTime<=1f)
        {
            yield return _waitUpdate;
        }
        float rotateIncrease = 0;
        float curRotateValue = 0;
        while (rotateIncrease<=90)
        {
            curRotateValue = Mathf.Abs(rotateposition.x + rotateposition.y + rotateposition.z) / (1 / Time.deltaTime * _rotateTime);
            _gameCubeTsf.RotateAround(_gameCubeTsf.position, rotateposition, curRotateValue);
            rotateIncrease += curRotateValue;
            yield return _waitUpdate;
        }
        _gameCubeTsf.RotateAround(_gameCubeTsf.position, rotateposition, Mathf.Abs(rotateposition.x + rotateposition.y + rotateposition.z) - rotateIncrease);
        _gameCubeTsf.localEulerAngles = new Vector3(MathF.Round(_gameCubeTsf.localEulerAngles.x), Mathf.Round(_gameCubeTsf.localEulerAngles.y), Mathf.Round(_gameCubeTsf.localEulerAngles.z));
    }
    IEnumerator ShowRotateImage()
    {
        _rotateSprite.sprite = _rotateTargetImage;
        float waitTime = InGameManager_s.Instance.beatFreezeCount * InGameMusicManager_s.Instance.secPerBeat;
        StartCoroutine(ObjectAction.ImageFade(_rotateSprite, waitTime,false,1));
        float startTime = InGameMusicManager_s.Instance.musicPosition;
        while (InGameMusicManager_s.Instance.musicPosition - startTime <= waitTime)
        {
            yield return _waitUpdate;
        }
        _rotateSprite.sprite = null;
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
        float startTime = InGameMusicManager_s.Instance.musicPosition;
        float rotateIncrease = 0;
        float curRotateValue = 0;
        while (InGameMusicManager_s.Instance.musicPosition-startTime<=rotateTime)
        {
            curRotateValue = Mathf.Abs(rotateTarget.x + rotateTarget.y + rotateTarget.z) / (1 / Time.deltaTime * rotateTime);
            _gameCubeTsf.RotateAround(_gameCubeTsf.position, rotateTarget, curRotateValue);
            rotateIncrease += curRotateValue;
            yield return _waitUpdate;
        }
        _gameCubeTsf.RotateAround(_gameCubeTsf.position, rotateTarget, Mathf.Abs(rotateTarget.x + rotateTarget.y + rotateTarget.z) - rotateIncrease);
        _gameCubeTsf.localEulerAngles = new Vector3(MathF.Round(_gameCubeTsf.localEulerAngles.x), Mathf.Round(_gameCubeTsf.localEulerAngles.y), Mathf.Round(_gameCubeTsf.localEulerAngles.z));

        rotateTarget = rotateTarget * -1;
        startTime = InGameMusicManager_s.Instance.musicPosition;
        rotateIncrease = 0;
        curRotateValue = 0;
        while (InGameMusicManager_s.Instance.musicPosition - startTime <= rotateTime)
        {
            curRotateValue = Mathf.Abs(rotateTarget.x + rotateTarget.y + rotateTarget.z) / (1 / Time.deltaTime * rotateTime);
            _gameCubeTsf.RotateAround(_gameCubeTsf.position, rotateTarget, curRotateValue);
            rotateIncrease += curRotateValue;
            yield return _waitUpdate;
        }
        _gameCubeTsf.RotateAround(_gameCubeTsf.position, rotateTarget, Mathf.Abs(rotateTarget.x + rotateTarget.y + rotateTarget.z) - rotateIncrease);
        _gameCubeTsf.localEulerAngles = new Vector3(MathF.Round(_gameCubeTsf.localEulerAngles.x), Mathf.Round(_gameCubeTsf.localEulerAngles.y), Mathf.Round(_gameCubeTsf.localEulerAngles.z));
    }
}