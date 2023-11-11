using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public partial class InGameEnemy_s : MonoBehaviour,IInGame//data
{
    [Header("script")]
    private ScriptManager_s _scripts;
    [SerializeField]private EnemyUI_s _enemyUI_s;

    [Header("data")]
    private List<EEnemyMode> _curEnemyMods;
    private Dictionary<EEnemyMode, Action> _enemyModBinds;
    [SerializeField] private int _enemyMaxHP;
    [SerializeField] private int _curEnemyHP;
    [SerializeField] private int _enemyAttackGaugeMax;
    private int _curEnemyATKGauge;
    private int _curEnemyATKGaugeCount;
    [SerializeField]private EEnemyPhase _curEnemyPhase;

    [Header("pattern")]
    [SerializeField] private GhostPattern _ghostPattern_s;
    [SerializeField] private NoisePattern_s _noisePattern_s;
    [SerializeField] private LineAttackPattern _lineAttackPattern_s;
    [SerializeField] private LinkLineAttackPattern _linkLineAttackPattern_s;
    public void ScriptBind(ScriptManager_s script)
    {
        _scripts = script;
    }
    private void Start()
    {
        _curEnemyMods = new List<EEnemyMode>();
        _enemyModBinds = new Dictionary<EEnemyMode, Action>();
        PatternBindSetting();
    }
    private void PatternBindSetting()
    {
        EnemyModBind(ref _enemyModBinds, EEnemyMode.GHOST, null);
        EnemyModBind(ref _enemyModBinds, EEnemyMode.NOISE,()=> _noisePattern_s.Action(_scripts._inGameManager_s.curInGameStatus));
        EnemyModBind(ref _enemyModBinds, EEnemyMode.LINEATTACK, ()=>_lineAttackPattern_s.Action(_scripts._inGameManager_s.curInGameStatus,EnemyPhaseEndCheck(),(EnemyPatternEndCheck()&&_curEnemyATKGauge==_enemyAttackGaugeMax)||!EnemyPatternEndCheck()));
        EnemyModBind(ref _enemyModBinds, EEnemyMode.LINKLINEATTACK, ()=>_linkLineAttackPattern_s.Action(_scripts._inGameManager_s.curInGameStatus, EnemyPhaseEndCheck(), EnemyPatternEndCheck() && _curEnemyATKGauge == _enemyAttackGaugeMax || !EnemyPatternEndCheck()));
    }
    public void EnemyModBind<T>(ref Dictionary<T, Action> binddic, T mod, Action action)
    {
        if (!binddic.ContainsKey(mod))
        {
            binddic.Add(mod, action);
        }
    } 
}
public partial class InGameEnemy_s //game system
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
        _curEnemyMods.Clear();
        _curEnemyHP = _enemyMaxHP;
        _curEnemyATKGauge = 0;
        _curEnemyATKGaugeCount = 0;
        _curEnemyPhase = EEnemyPhase.None;
    }
    public void GamePlay()
    {
        _enemyUI_s.HPReset();
    }
    public void GameEnd()
    {
        DoEnemyMode();
        if(_ghostPattern_s.isGhostPlay)
        {
            _ghostPattern_s.EndGhost();
        }
        _noisePattern_s.EndNoisePattern();
    }
    public void MoveNextBit(EInGameStatus curInGameStatus)
    {
        switch (curInGameStatus)
        {
            case EInGameStatus.SHOWPATH:
                if (EnemyPhaseEndCheck())
                {
                    DoEnemyMode();
                }
                break;
            case EInGameStatus.PLAYERMOVE:
                    DoEnemyMode();
                    EnemyPositionCheck(_scripts._inGamePlayer_s.playerPos);
                    if(EnemyPatternEndCheck())
                    {
                        _curEnemyATKGaugeCount++;
                        if (_curEnemyATKGaugeCount == 2)
                        {
                            _curEnemyATKGauge++;
                            _curEnemyATKGaugeCount = 0;
                        }
                    }
                break;
            case EInGameStatus.TIMEWAIT:
                break;
            default:
                break;
        }
    }
    public void ChangeInGameStatus(EInGameStatus changeTarget) //change to changeTarget
    {
        switch (changeTarget)
        {
            case EInGameStatus.SHOWPATH:
                switch(_curEnemyPhase)
                {
                    case EEnemyPhase.None:
                        EnemyPhaseChange(EEnemyPhase.Ghost);
                        break;
                    case EEnemyPhase.Phase1:
                        EnemyPhaseChange(EEnemyPhase.Phase2);
                        break;
                    case EEnemyPhase.Phase2:
                        EnemyPhaseChange(EEnemyPhase.Phase3);
                        break;
                    case EEnemyPhase.Phase3:
                        EnemyPhaseChange(EEnemyPhase.Phase2);
                        break;
                }
                break;
            case EInGameStatus.CUBEROTATE:
                DoEnemyMode();
                break;
            default:
                break;
        }
    }
    public bool EnemyPhaseEndCheck()
    {
        if(_scripts._inGameManager_s.curGameStatus==EGameStatus.END)
        {
            return true;
        }
        if(_curEnemyMods.Contains(EEnemyMode.NOISE))
        {
           return _noisePattern_s.isPatternEnd();
        }
        return false;
    }
    public bool EnemyPatternEndCheck()
    {
        foreach(var data in _curEnemyMods)
        {
            switch(data)
            {
                case EEnemyMode.LINEATTACK:
                    if(_lineAttackPattern_s.curLineAttackMod==ELineAttackMode.NONE)
                    {
                        return true;
                    }                       
                    break;
                case EEnemyMode.LINKLINEATTACK:
                    if (_linkLineAttackPattern_s.curLinkLineAttackMode==ELinkLineAttackMode.NONE)
                    {
                        return true;
                    }
                    break;
            }
        }
        return false;
    }
    private void DoEnemyMode()
    {
        foreach (var data in _curEnemyMods)
        {
            if (_enemyModBinds[data]!= null)
            {
                _enemyModBinds[data]();
            }
        }
        if (_curEnemyATKGauge >= _enemyAttackGaugeMax)
        {
            _curEnemyATKGauge = 0;
        }
    }
    public void EnemyPhaseChange(EEnemyPhase target)
    {
        switch(target)
        {
            case EEnemyPhase.Ghost:
                if(!_curEnemyMods.Contains(EEnemyMode.GHOST))
                {            
                    _curEnemyMods.Add(EEnemyMode.GHOST);
                }
                if (!_curEnemyMods.Contains(EEnemyMode.NOISE))
                {
                    _curEnemyMods.Add(EEnemyMode.NOISE);
                }
                _ghostPattern_s.SetGhost();
                break;
            case EEnemyPhase.Phase1:
                _ghostPattern_s.EndGhost();
                _curEnemyMods.Remove(EEnemyMode.GHOST);
                break;
            case EEnemyPhase.Phase2:
                if (_curEnemyMods.Contains(EEnemyMode.LINEATTACK))
                {
                    _curEnemyMods.Remove(EEnemyMode.LINEATTACK);
                }
                _curEnemyMods.Add(EEnemyMode.LINKLINEATTACK);
                break;
            case EEnemyPhase.Phase3:
                if (_curEnemyMods.Contains(EEnemyMode.LINKLINEATTACK))
                {
                    _curEnemyMods.Remove(EEnemyMode.LINKLINEATTACK);
                }
                _curEnemyMods.Add(EEnemyMode.LINEATTACK);
                break;
        }
        _curEnemyPhase = target;
    }
}
public partial class InGameEnemy_s //data change
{
    public void UpdateEnemyHP(int changevalue)
    {
        _curEnemyHP += changevalue;
        if (changevalue < 0)
        {
            EnemyHPDown();
        }
    }
    private void EnemyHPDown()
    {
        _enemyUI_s.EnemyHPDown(_scripts._inGameCube_s.curFace);
        if (_curEnemyHP <= 0)
        {
            _scripts._inGameManager_s.GameOverByEnemy();
        }
    }
}
public partial class InGameEnemy_s //pattern
{
    public void EnemyPositionCheck(Vector2Int playerPos)
    {
        if (_curEnemyMods.Contains(EEnemyMode.LINEATTACK))
        {
            if (_scripts._inGameSideData_s.sideDatas[playerPos.x, playerPos.y].lineAttack != null && _lineAttackPattern_s.curLineAttackMod == ELineAttackMode.ATTACK)
            {
                _scripts._inGamePlayer_s.UpdatePlayerHP(-1);
            }
        }
        if (_curEnemyMods.Contains(EEnemyMode.LINKLINEATTACK))
        {
            if (_scripts._inGameSideData_s.sideDatas[playerPos.x, playerPos.y].linkLineAttack != null && _linkLineAttackPattern_s.curLinkLineAttackMode == ELinkLineAttackMode.ATTACK)
            {
                _scripts._inGamePlayer_s.UpdatePlayerHP(-1);
            }
        }
    }
    public void GetRandomeObjs(string dataName, GameObject instTarget, bool isStack, Transform parent, int makeCount)
    {
        for (int i = 0; i < makeCount; i++)
        {
            if (_curEnemyPhase == EEnemyPhase.Ghost && i == 0 && dataName == "noise")
            {
                int x = 2;
                int y = 1;
                GameObject instObj = Instantiate(instTarget, parent);
                instObj.transform.localPosition = _scripts._inGameSideData_s.sideDatas[x, y].transform;
                TypedReference tr = __makeref(_scripts._inGameSideData_s.sideDatas[x, y]);
                _scripts._inGameSideData_s.sideDatas[x, y].GetType().GetField(dataName).SetValueDirect(tr, instObj);
                continue;
            }
            bool isCreate = false;
            int block = 0;
            while (!isCreate)
            {
                if (block == 50)
                {
                    break;
                }
                int x = UnityEngine.Random.Range(0, _scripts._inGameSideData_s.divideSize.x);
                int y = UnityEngine.Random.Range(0, _scripts._inGameSideData_s.divideSize.y);
                if (new Vector2(x, y) == _scripts._inGamePlayer_s.playerPos || !_scripts._inGameSideData_s.sideDatas[x, y].isCanMakeCheck(isStack, dataName))
                {
                    continue;
                }
                else
                {
                    GameObject instObj = Instantiate(instTarget, parent);
                    instObj.transform.localPosition = _scripts._inGameSideData_s.sideDatas[x, y].transform;
                    TypedReference tr = __makeref(_scripts._inGameSideData_s.sideDatas[x, y]);
                    _scripts._inGameSideData_s.sideDatas[x, y].GetType().GetField(dataName).SetValueDirect(tr, instObj);
                    isCreate = true;
                }
                block++;
            }
        }
    }
    public void RemoveTargetObj(string dataName, int xpos, int ypos,bool isDestroy)
    {
        TypedReference tr = __makeref(_scripts._inGameSideData_s.sideDatas[xpos, ypos]);
        object target = _scripts._inGameSideData_s.sideDatas[xpos, ypos].GetType().GetField(dataName).GetValueDirect(tr);
        if (target != null)
        {
            if (target.GetType().Equals(typeof(GameObject)))
            {
                if (isDestroy)
                {
                    Destroy((GameObject)target);
                }
                else
                {
                    ((GameObject)target).SetActive(false);
                }
                _scripts._inGameSideData_s.sideDatas[xpos, ypos].GetType().GetField(dataName).SetValueDirect(tr, null);
            }
        }
    }
    public void RemoveAllTargetObj(string dataName, bool isDestroy)
    {
        for (int i = 0; i < _scripts._inGameSideData_s.divideSize.x; i++)
        {
            for (int j = 0; j < _scripts._inGameSideData_s.divideSize.y; j++)
            {
                RemoveTargetObj(dataName, i, j,isDestroy);
            }
        }
    }
}

