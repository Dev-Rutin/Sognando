using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public partial class InGameBeatManager_s : MonoBehaviour,IInGame,IScript
{
    [Header("script")]
    private ScriptManager_s _scripts;
    [SerializeField] private GhostPattern _ghostPattern_s;

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
    [Header("auto")]
    private bool _isAutoCalibrationOn;
    private float _autoCalibrationValue;
    public void ScriptBind(ScriptManager_s script)
    {
        _scripts = script;
    }
    void Start()
    {
        _beatObjList = new List<GameObject>();
        _beatObjList.Add(Instantiate(_beatPrefab, _beatTsf));
        _beatObjList.Add(Instantiate(_beatPrefab, _beatTsf));
        _waitUpdate = new WaitForEndOfFrame();
    }
  
}
public partial class InGameBeatManager_s
{
    public void InGameBind()
    {
        _scripts._inGamefunBind_s.EgameStart += GameStart;
        _scripts._inGamefunBind_s.EgamePlay += GamePlay;
        _scripts._inGamefunBind_s.EgameEnd += GameEnd;
        _scripts._inGamefunBind_s.EmoveNextBit += MoveNextBit;
        _scripts._inGamefunBind_s.EchangeInGameState += ChangeInGameStatus;
    }
    public void GameStart()
    {
        _beatObjList[0].transform.localScale = _startScale;
        _beatObjList[1].transform.localScale = _startScale-((_startScale-_endScale)/2);
        _beatSelect = true;
    }
    public void GamePlay()
    {
        StartCoroutine(BeatShow1(_beatObjList[0]));
        StartCoroutine(BeatShow2(_beatObjList[1]));
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
        if(_beatSelect)
        {
            BeatShow1MoveNext();
        }
        else
        {
            BeatShow2MoveNext();
        }
        _beatSelect = !_beatSelect;
    }

    private float _beatShow1LerpValue;
    private IEnumerator BeatShow1(GameObject beatObj)
    {
        yield return new WaitUntil(() => _scripts._inGameMusicManager_s.completedLoops < 1);
        float lastPos = _scripts._inGameMusicManager_s.musicPosition;
        _beatShow1LerpValue = (_startScale.x - beatObj.transform.localScale.x) * 1f / (_startScale.x - _endScale.x);;
        while (_scripts._inGameManager_s.curGameStatus == EGameStatus.PLAYING)
        {
            _beatShow1LerpValue += (_scripts._inGameMusicManager_s.musicPosition - lastPos) / (_scripts._inGameMusicManager_s.secPerBeat * 2);
            beatObj.transform.localScale = Vector3.Lerp(_startScale, _endScale, _beatShow1LerpValue);
            lastPos = _scripts._inGameMusicManager_s.musicPosition;
            yield return _waitUpdate;
        }
    }
    private void BeatShow1MoveNext()
    {
        //Debug.Log("beat1"+_scripts._inGameMusicManager_s.musicPosition);
        _beatShow1LerpValue = 0;
        if (_ghostPattern_s.isGhostPlay)
        {
            _ghostPattern_s.Action(_scripts._inGameManager_s.curInGameStatus);
        }
    }
    private float _beatShow2LerpValue;
    private IEnumerator BeatShow2(GameObject beatObj)
    {
        yield return new WaitUntil(() => _scripts._inGameMusicManager_s.completedLoops < 1);
        float lastPos = _scripts._inGameMusicManager_s.musicPosition;
        _beatShow2LerpValue = (_startScale.x - beatObj.transform.localScale.x) * 1f / (_startScale.x - _endScale.x);
        int lastCompeletedLoop = _scripts._inGameMusicManager_s.completedLoops;
        while (_scripts._inGameManager_s.curGameStatus == EGameStatus.PLAYING)
        {
            _beatShow2LerpValue += ( _scripts._inGameMusicManager_s.musicPosition - lastPos) / (_scripts._inGameMusicManager_s.secPerBeat*2);
            beatObj.transform.localScale = Vector3.Lerp(_startScale, _endScale, _beatShow2LerpValue);
            lastPos = _scripts._inGameMusicManager_s.musicPosition;
            yield return _waitUpdate;
        }
    }
    private void BeatShow2MoveNext()
    {
        //Debug.Log("beat2"+_scripts._inGameMusicManager_s.musicPosition);
        _beatShow2LerpValue = 0;
        if (_ghostPattern_s.isGhostPlay)
        {
            _ghostPattern_s.Action(_scripts._inGameManager_s.curInGameStatus);
        }
    }
    public bool BeatJudgement()
    {
        StageDataController.Instance.totalValue += 2;
        if (_scripts._inGameMusicManager_s.loopPositionInBeats <= beatJudgeMax|| _scripts._inGameMusicManager_s.loopPositionInBeats >= beatJudgeMin)
        {
            AutoCalibration(true, false);
            BeatScoreCount(_scripts._inGameMusicManager_s.loopPositionInBeats);
            return true;
        }
        else
        {
            if (_scripts._inGameMusicManager_s.loopPositionInBeats + _autoCalibrationValue <= beatJudgeMax || _scripts._inGameMusicManager_s.loopPositionInBeats + _autoCalibrationValue >= beatJudgeMin)
            {
                AutoCalibration(true, true);
                BeatScoreCount(_scripts._inGameMusicManager_s.loopPositionInBeats + _autoCalibrationValue);
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
        float distance = _scripts._inGameMusicManager_s.loopPositionInBeats > beatJudgeMax + 0.1f ? beatJudgeMin - _scripts._inGameMusicManager_s.loopPositionInBeats : beatJudgeMax - _scripts._inGameMusicManager_s.loopPositionInBeats;

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
        if (postionValue <= beatJudgeMax - 0.05f || postionValue >= beatJudgeMin + 0.05f)
        {
            _scripts._inGameManager_s.PerfectScroe();
        }
        else
        {
            _scripts._inGameManager_s.GoodScroe();
        }
    }
}