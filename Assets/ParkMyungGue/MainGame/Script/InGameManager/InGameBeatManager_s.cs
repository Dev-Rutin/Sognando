using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class BeatStruct
{
    public float value;
}
public partial class InGameBeatManager_s : Singleton<InGameBeatManager_s>,IInGame
{
    [Header("data")]
    [SerializeField] private GameObject _beatPrefab;
    [SerializeField] private Transform _beatTsf;
    [SerializeField] private Vector3 _startScale;
    [SerializeField] private Vector3 _endScale;

    [SerializeField] private float m_beatJudgeMax;
    public float beatJudgeMax { get => m_beatJudgeMax; }

    [SerializeField] private float m_beatJudgeMin;
    public float beatJudgeMin { get => m_beatJudgeMin; }

    private List<GameObject> _beatObjList;
    private WaitForEndOfFrame _waitUpdate;

    BeatStruct _beatShowLerpValue1;
    BeatStruct _beatShowLerpValue2;
    [Header("auto")]
    private bool _isAutoCalibrationOn;
    private float _autoCalibrationValue;
    void Start()
    {
        _beatObjList = new List<GameObject>();
        _beatObjList.Add(Instantiate(_beatPrefab, _beatTsf));
        _beatObjList.Add(Instantiate(_beatPrefab, _beatTsf));
        _waitUpdate = new WaitForEndOfFrame();
        _beatShowLerpValue1 = new BeatStruct();
        _beatShowLerpValue2 = new BeatStruct();
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
        _beatObjList[0].transform.localScale = _startScale;
        _beatObjList[1].transform.localScale = _startScale-((_startScale-_endScale)/2);
        _beatSelect = true;
    }
    public void GamePlay()
    {
        StartCoroutine(BeatShow(_beatObjList[0],_beatShowLerpValue1));
        StartCoroutine(BeatShow(_beatObjList[1], _beatShowLerpValue2));
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
            case EInGameStatus.TIMEWAIT:
                for(int i=0;i<_beatObjList.Count;i++)
                {
                    _beatObjList[i].SetActive(false);
                }
                break;
            case EInGameStatus.SHOWPATH:
                _isAutoCalibrationOn = false;
                _autoCalibrationValue = 0f;
                for (int i = 0; i < _beatObjList.Count; i++)
                {
                    _beatObjList[i].SetActive(true);
                }
                break;
            default:
                break;
        }
    }
    private bool _beatSelect;
    public void NextBit()
    {
        if (_beatSelect)
        {
            _beatShowLerpValue1.value = 0;
        }
        else
        {
            _beatShowLerpValue2.value = 0;
        }
        if (GhostPattern.Instance.isGhostPlay)
        {
            GhostPattern.Instance.Action(InGameManager_s.Instance.curInGameStatus);
        }
        _beatSelect = !_beatSelect;
    }

    private IEnumerator BeatShow(GameObject beatObj,BeatStruct data)
    {
        yield return new WaitUntil(() => InGameMusicManager_s.Instance.completedLoops < 1);
        float lastPos = InGameMusicManager_s.Instance.musicPosition;
        data.value = (_startScale.x - beatObj.transform.localScale.x) * 1f / (_startScale.x - _endScale.x);;
        while (InGameManager_s.Instance.curGameStatus == EGameStatus.PLAYING)
        {
            data.value += (InGameMusicManager_s.Instance.musicPosition - lastPos) / (InGameMusicManager_s.Instance.secPerBeat * 2);
            beatObj.transform.localScale = Vector3.Lerp(_startScale, _endScale, data.value);
            lastPos = InGameMusicManager_s.Instance.musicPosition;
            yield return _waitUpdate;
        }
    }
    public bool BeatJudgement()
    {
        StageDataController.Instance.totalValue += 2;
        if (InGameMusicManager_s.Instance.loopPositionInBeats <= beatJudgeMax|| InGameMusicManager_s.Instance.loopPositionInBeats >= beatJudgeMin)
        {
            AutoCalibration(true, false);
            BeatScoreCount(InGameMusicManager_s.Instance.loopPositionInBeats);
            return true;
        }
        else
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
        }
    }
    private void AutoCalibration(bool check, bool isAuto)
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
    }
    private void BeatScoreCount(float postionValue)
    {
        Debug.Log(postionValue);
        if (postionValue <= beatJudgeMax - 0.2f || postionValue >= beatJudgeMin + 0.2f)
        {
            InGameManager_s.Instance.PerfectScroe();
        }
        else
        {
            InGameManager_s.Instance.GoodScroe();
        }
    }
}