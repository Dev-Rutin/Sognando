using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Rendering;

class BeatStruct
{
    public float value;
}
public partial class InGameBeatManager_s : Singleton<InGameBeatManager_s>,IInGame
{
    [SerializeField] private CameraShake _cameraShake;
    [Header("data")]
    [SerializeField] private GameObject _beatPrefabVertical;
    [SerializeField] private GameObject _beatPrefabHorizontal;
    [SerializeField] private Transform _beatTsf;
    [SerializeField] private GameObject _beatPrefabVertical2;
    [SerializeField] private GameObject _beatPrefabHorizontal2;
    [SerializeField] private Transform _playerBeatTsf;

    [SerializeField] private float m_beatJudgeMax;
    public float beatJudgeMax { get => m_beatJudgeMax; }

    [SerializeField] private float m_beatJudgeMin;
    public float beatJudgeMin { get => m_beatJudgeMin; }

    private List<GameObject> _beatObjList;
    private WaitForEndOfFrame _waitUpdate;

    BeatStruct _beatShowLerpValue1;
    BeatStruct _beatShowLerpValue2;

    [SerializeField] private ParticleSystem _moveEffectBad;
    [Header("auto")]
    private bool _isAutoCalibrationOn;
    private float _autoCalibrationValue;
    void Start()
    {
        _beatObjList = new List<GameObject>();
        _beatObjList.Add(Instantiate(_beatPrefabVertical,_beatTsf));
        _beatObjList.Add(Instantiate(_beatPrefabHorizontal, _beatTsf));
        _waitUpdate = new WaitForEndOfFrame();
        _beatShowLerpValue1 = new BeatStruct();
        _beatShowLerpValue2 = new BeatStruct();
        _beatObjList[0].SetActive(false);
        _beatObjList[1].SetActive(false);
    }
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
        _beatSelect = true;
    }
    public void GamePlay()
    {
        StartCoroutine(BeatShowVetrical(_beatObjList[0],_beatShowLerpValue1));
        StartCoroutine(BeatShowHorizontal(_beatObjList[1], _beatShowLerpValue2));
        _beatObjList[0].SetActive(false);
    }
    public void GameEnd()
    {
        StopAllCoroutines();
    }
    public void MoveNextBit(EInGameStatus curInGameStatus)
    {

    }
    public void ChangeInGameStatus(EInGameStatus changeTarget)
    {
        switch (changeTarget)
        {
            case EInGameStatus.SHOWPATH:
                _isAutoCalibrationOn = false;
                _autoCalibrationValue = 0f;
                break;
            case EInGameStatus.TIMEWAIT:
                for (int i = 0; i < _beatObjList.Count; i++)
                {
                    _beatObjList[i].SetActive(false);
                }
                break;
            default:
                break;
        }
    }
    [SerializeField] private bool _beatSelect;
    public void UnshowBeat()
    {
        _beatObjList[1].SetActive(false);
        _beatObjList[0].SetActive(false);
    }
    public void NextBeat()
    {
        _beatSelect = UnityEngine.Random.Range(0, 2) == 1 ? true : false;//
        _beatSelect = true;

        if (_beatSelect)
        {
            _beatShowLerpValue1.value = 0;
            if (InGameManager_s.Instance.curInGameStatus != EInGameStatus.TIMEWAIT)
            {
                _beatObjList[1].SetActive(false);
                _beatObjList[0].SetActive(true);
            }
        }
        else
        {
            _beatShowLerpValue2.value = 0;
            if (InGameManager_s.Instance.curInGameStatus != EInGameStatus.TIMEWAIT)
            {
                _beatObjList[0].SetActive(false);
                _beatObjList[1].SetActive(true);
            }
        }
        if (GhostPattern.Instance.isGhostPlay)
        {
            GhostPattern.Instance.Action(InGameManager_s.Instance.curInGameStatus);
        }
    }
    private IEnumerator BeatShowVetrical(GameObject beatObj,BeatStruct data)
    {
        yield return new WaitUntil(() => InGameMusicManager_s.Instance.completedLoops < 1);
        double lastPos = InGameMusicManager_s.Instance.musicPosition;
        data.value = 0;
        Transform leftBeat = beatObj.transform.GetChild(0);
        Transform rightBeat = beatObj.transform.GetChild(1);
        Vector2 leftStartPos = leftBeat.localPosition;
        Vector2 rightStartPos = rightBeat.localPosition;
        while (InGameManager_s.Instance.curGameStatus == EGameStatus.PLAYING)
        {
            data.value += (float)((InGameMusicManager_s.Instance.musicPosition - lastPos) / InGameMusicManager_s.Instance.secPerBeat);
            leftBeat.transform.localPosition = leftStartPos + new Vector2(-1*Mathf.Lerp(0, 120, data.value), 0);
            rightBeat.transform.localPosition = rightStartPos + new Vector2(Mathf.Lerp(0, 120, data.value), 0);
            lastPos = InGameMusicManager_s.Instance.musicPosition;
            yield return _waitUpdate;
        }
    }
    private IEnumerator BeatShowHorizontal(GameObject beatObj, BeatStruct data)
    {
        yield return new WaitUntil(() => InGameMusicManager_s.Instance.completedLoops < 1);
        double lastPos = InGameMusicManager_s.Instance.musicPosition;
        data.value = 0;
        Transform upBeat = beatObj.transform.GetChild(0);
        Transform downBeat = beatObj.transform.GetChild(1);
        Vector2 upStartPos = upBeat.localPosition;
        Vector2 downStartPos = downBeat.localPosition;
        while (InGameManager_s.Instance.curGameStatus == EGameStatus.PLAYING)
        {
            data.value += (float)((InGameMusicManager_s.Instance.musicPosition - lastPos) / InGameMusicManager_s.Instance.secPerBeat);
            upBeat.transform.localPosition = upStartPos + new Vector2(0, -1 * Mathf.Lerp(0, 120, data.value));
            downBeat.transform.localPosition = downStartPos + new Vector2(0, Mathf.Lerp(0, 120, data.value));
            lastPos = InGameMusicManager_s.Instance.musicPosition;
            yield return _waitUpdate;
        }
    }
    public bool BeatJudgement()
    {
        StageDataController.Instance.totalValue += 2;
        if (InGameMusicManager_s.Instance.loopPositionInBeats <= beatJudgeMax|| InGameMusicManager_s.Instance.loopPositionInBeats >= beatJudgeMin)
        {
            //AutoCalibration(true, false);
            BeatScoreCount(InGameMusicManager_s.Instance.loopPositionInBeats);
            return true;
        }
        else
        {
            _moveEffectBad.Play();
            _cameraShake.ShakeStart();
            return false;
        }
        /*else
        {
            if (InGameMusicManager_s.Instance.loopPositionInBeats + _autoCalibrationValue <= beatJudgeMax || InGameMusicManager_s.Instance.loopPositionInBeats + _autoCalibrationValue >= beatJudgeMin)
            {
                AutoCalibration(true, true);
                BeatScoreCount(InGameMusicManager_s.Instance.loopPositionInBeats + _autoCalibrationValue);
                return true;
            }
            else
            {
                AutoCalibration(false, true);
                return false;
            }
        }*/
    }
    /*private void AutoCalibration(bool check, bool isAuto)
    {
        float distance = InGameMusicManager_s.Instance.loopPositionInBeats > beatJudgeMax + 0.1f ? beatJudgeMin - InGameMusicManager_s.Instance.loopPositionInBeats : beatJudgeMax - InGameMusicManager_s.Instance.loopPositionInBeats;

        if (!check) //miss
        {
            if (MathF.Abs(distance) <= 0.3f)
            {
                if (!_isAutoCalibrationOn)
                {
                    _isAutoCalibrationOn = true;
                    if (distance < 0)
                    {
                        _autoCalibrationValue = distance - 0.15f;
                    }
                    else
                    {
                        _autoCalibrationValue = distance + 0.15f;
                    }
                }
                else
                {
                    _isAutoCalibrationOn = false;
                    _autoCalibrationValue = 0f;
                }
            }
            else
            {
                _isAutoCalibrationOn = false;
                _autoCalibrationValue = 0f;
            }
        }
        else //not miss
        {
            if (isAuto)
            {
                if (_isAutoCalibrationOn)
                {
                    distance = Mathf.Lerp(_autoCalibrationValue, 0f, 1f - (distance / 0.2f));
                    if (MathF.Abs(distance) <= 0.01f)
                    {
                        _isAutoCalibrationOn = false;
                        _autoCalibrationValue = 0f;
                    }
                    else
                    {
                        if (distance < 0)
                        {
                            _autoCalibrationValue = distance - 0.08f;
                        }
                        else
                        {
                            _autoCalibrationValue = distance + 0.08f;
                        }
                    }
                }

            }
        }
    }*/
    private void BeatScoreCount(double postionValue)
    {
        if (postionValue <= beatJudgeMax - 0.15f || postionValue >= beatJudgeMin + 0.2f)
        {
            InGameManager_s.Instance.PerfectScroe();
        }
        else
        {
            InGameManager_s.Instance.GoodScroe();
        }
    }
}