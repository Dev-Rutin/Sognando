using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public partial class InGameEnemy_s : Singleton<InGameEnemy_s>, IInGame//data
{
    [Header("data")]
    private List<EEnemyMode> _curEnemyMods;
    private Dictionary<EEnemyMode, Action> _enemyModBinds;
    [SerializeField] private int _enemyMaxHP;
    [SerializeField] private int _curEnemyHP;
    [SerializeField] private int _enemyAttackGaugeMax;
    private int _curEnemyATKGauge;
    private int _curEnemyATKGaugeCount;
    [SerializeField]private EEnemyPhase _curEnemyPhase;
    private void Start()
    {
        _curEnemyMods = new List<EEnemyMode>();
        _enemyModBinds = new Dictionary<EEnemyMode, Action>();
        PatternBindSetting();
    }
    private void PatternBindSetting()
    {
        EnemyModBind(ref _enemyModBinds, EEnemyMode.GHOST, null);
        EnemyModBind(ref _enemyModBinds, EEnemyMode.NOISE,()=> NoisePattern_s.Instance.Action(InGameManager_s.Instance.curInGameStatus));
        EnemyModBind(ref _enemyModBinds, EEnemyMode.LINEATTACK, ()=>LineAttackPattern.Instance.Action(InGameManager_s.Instance.curInGameStatus,EnemyPhaseEndCheck(),(EnemyPatternEndCheck()&&_curEnemyATKGauge==_enemyAttackGaugeMax)||!EnemyPatternEndCheck()));
        EnemyModBind(ref _enemyModBinds, EEnemyMode.LINKLINEATTACK, ()=>LinkLineAttackPattern.Instance.Action(InGameManager_s.Instance.curInGameStatus, EnemyPhaseEndCheck(), EnemyPatternEndCheck() && _curEnemyATKGauge == _enemyAttackGaugeMax || !EnemyPatternEndCheck()));
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
        InGameFunBind_s.Instance.EgameStart += GameStart;
        InGameFunBind_s.Instance.EgamePlay += GamePlay;
        InGameFunBind_s.Instance.EgameEnd += GameEnd;
        InGameFunBind_s.Instance.EmoveNextBit += MoveNextBit;
        InGameFunBind_s.Instance.EchangeInGameState += ChangeInGameStatus;
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
        EnemyUI_s.Instance.HPReset();
    }
    public void GameEnd()
    {
        DoEnemyMode();
        if(GhostPattern.Instance.isGhostPlay)
        {
            GhostPattern.Instance.EndGhost();
        }
       NoisePattern_s.Instance.EndNoisePattern();
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
                    EnemyPositionCheck(InGamePlayer_s.Instance.playerPos);
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
        if(InGameManager_s.Instance.curGameStatus==EGameStatus.END)
        {
            return true;
        }
        if(_curEnemyMods.Contains(EEnemyMode.NOISE))
        {
           return NoisePattern_s.Instance.isPatternEnd();
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
                    if(LineAttackPattern.Instance.curLineAttackMod==ELineAttackMode.NONE)
                    {
                        return true;
                    }                       
                    break;
                case EEnemyMode.LINKLINEATTACK:
                    if (LinkLineAttackPattern.Instance.curLinkLineAttackMode==ELinkLineAttackMode.NONE)
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
                GhostPattern.Instance.SetGhost();
                break;
            case EEnemyPhase.Phase1:
                GhostPattern.Instance.EndGhost();
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
        EnemyUI_s.Instance.EnemyHPDown(InGameCube_s.Instance.curFace);
        if (_curEnemyHP <= 0)
        {
            InGameManager_s.Instance.GameOverByEnemy();
        }
    }
}
public partial class InGameEnemy_s //pattern
{
    public void EnemyPositionCheck(Vector2Int playerPos)
    {
        if (_curEnemyMods.Contains(EEnemyMode.LINEATTACK))
        {
            if (InGameSideData_s.Instance.sideDatas[playerPos.x, playerPos.y].lineAttack != null && LineAttackPattern.Instance.curLineAttackMod == ELineAttackMode.ATTACK)
            {
                InGamePlayer_s.Instance.UpdatePlayerHP(-1);
            }
        }
        if (_curEnemyMods.Contains(EEnemyMode.LINKLINEATTACK))
        {
            if (InGameSideData_s.Instance.sideDatas[playerPos.x, playerPos.y].linkLineAttack != null && LinkLineAttackPattern.Instance.curLinkLineAttackMode == ELinkLineAttackMode.ATTACK)
            {
                InGamePlayer_s.Instance.UpdatePlayerHP(-1);
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
                instObj.transform.localPosition = InGameSideData_s.Instance.sideDatas[x, y].transform;
                TypedReference tr = __makeref(InGameSideData_s.Instance.sideDatas[x, y]);
                InGameSideData_s.Instance.sideDatas[x, y].GetType().GetField(dataName).SetValueDirect(tr, instObj);
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
                int x = UnityEngine.Random.Range(0, InGameSideData_s.Instance.divideSize.x);
                int y = UnityEngine.Random.Range(0, InGameSideData_s.Instance.divideSize.y);
                if (new Vector2(x, y) == InGamePlayer_s.Instance.playerPos || !InGameSideData_s.Instance.sideDatas[x, y].isCanMakeCheck(isStack, dataName))
                {
                    continue;
                }
                else
                {
                    GameObject instObj = Instantiate(instTarget, parent);
                    instObj.transform.localPosition = InGameSideData_s.Instance.sideDatas[x, y].transform;
                    TypedReference tr = __makeref(InGameSideData_s.Instance.sideDatas[x, y]);
                    InGameSideData_s.Instance.sideDatas[x, y].GetType().GetField(dataName).SetValueDirect(tr, instObj);
                    isCreate = true;
                }
                block++;
            }
        }
    }
    public void RemoveTargetObj(string dataName, int xpos, int ypos,bool isDestroy)
    {
        TypedReference tr = __makeref(InGameSideData_s.Instance.sideDatas[xpos, ypos]);
        object target = InGameSideData_s.Instance.sideDatas[xpos, ypos].GetType().GetField(dataName).GetValueDirect(tr);
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
                InGameSideData_s.Instance.sideDatas[xpos, ypos].GetType().GetField(dataName).SetValueDirect(tr, null);
            }
        }
    }
    public void RemoveAllTargetObj(string dataName, bool isDestroy)
    {
        for (int i = 0; i < InGameSideData_s.Instance.divideSize.x; i++)
        {
            for (int j = 0; j < InGameSideData_s.Instance.divideSize.y; j++)
            {
                RemoveTargetObj(dataName, i, j,isDestroy);
            }
        }
    }
}

