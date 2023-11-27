using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    public EEnemyPhase curEnemyPhase { get; private set; }
    public ECubeFace curPumpTarget { get; private set; }
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
        EnemyModBind(ref _enemyModBinds, EEnemyMode.CROSSATTACK, () => CrossAttackPattern.Instance.Action(InGameManager_s.Instance.curInGameStatus, EnemyPhaseEndCheck(), (EnemyPatternEndCheck() && _curEnemyATKGauge == _enemyAttackGaugeMax) || !EnemyPatternEndCheck()));

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
        EnemyUI_s.Instance.EnemyHPInitialize(_enemyMaxHP);
        _curEnemyHP = _enemyMaxHP;
        _curEnemyATKGauge = 0;
        _curEnemyATKGaugeCount = 0;
        curEnemyPhase = EEnemyPhase.None;
    }
    public void GamePlay()
    {
        EnemyUI_s.Instance.EnemyHPUpdate(_curEnemyHP);
        EnemyUI_s.Instance.MutipleEnemyAnimation(new List<string>() { "start", "idle" });
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
        curPumpTarget = curPumpTarget+1;
    }
    public void ChangeInGameStatus(EInGameStatus changeTarget) //change to changeTarget
    {
        switch (changeTarget)
        {
            case EInGameStatus.SHOWPATH:
                switch(curEnemyPhase)
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
                        EnemyPhaseChange(EEnemyPhase.Phase4);
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
        bool rValue = true;
        foreach(var data in _curEnemyMods)
        {
            switch(data)
            {
                case EEnemyMode.LINEATTACK:
                    if(LineAttackPattern.Instance.curLineAttackMod!=ELineAttackMode.NONE)
                    {
                        rValue = false;
                    }                       
                    break;
                case EEnemyMode.LINKLINEATTACK:
                    if (LinkLineAttackPattern.Instance.curLinkLineAttackMode!=ELinkLineAttackMode.NONE)
                    {
                        rValue = false;
                    }
                    break;
                case EEnemyMode.CROSSATTACK:
                    if (CrossAttackPattern.Instance.curCrossAttackMod != ELineAttackMode.NONE)
                    {
                        rValue = false;
                    }
                    break;
                default:
                    break;
            }
        }
        return rValue;
    }
    private void DoEnemyMode()
    {
        if(_curEnemyMods.Contains(EEnemyMode.NOISE))
        {
            _enemyModBinds[EEnemyMode.NOISE]();
        }
        if (_curEnemyMods.Contains(EEnemyMode.LINEATTACK) && _curEnemyMods.Contains(EEnemyMode.LINKLINEATTACK)&& _curEnemyMods.Contains(EEnemyMode.CROSSATTACK))
        {
            if(EnemyPatternEndCheck())
            {
                switch(UnityEngine.Random.Range(0, 3))
                {
                    case 0:
                        _enemyModBinds[EEnemyMode.LINEATTACK]();
                        break;
                    case 1:
                        _enemyModBinds[EEnemyMode.LINKLINEATTACK]();
                        break;
                    case 2:
                        _enemyModBinds[EEnemyMode.CROSSATTACK]();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (LineAttackPattern.Instance.curLineAttackMod != ELineAttackMode.NONE)
                {
                    _enemyModBinds[EEnemyMode.LINEATTACK]();
                }
                else if(LinkLineAttackPattern.Instance.curLinkLineAttackMode != ELinkLineAttackMode.NONE)
                {
                    _enemyModBinds[EEnemyMode.LINKLINEATTACK]();
                }
                if (CrossAttackPattern.Instance.curCrossAttackMod != ELineAttackMode.NONE)
                {
                    _enemyModBinds[EEnemyMode.CROSSATTACK]();
                }
            }
        }
        else
        {
            if (_curEnemyMods.Contains(EEnemyMode.LINEATTACK))
            {
                _enemyModBinds[EEnemyMode.LINEATTACK]();
            }
            if (_curEnemyMods.Contains(EEnemyMode.LINKLINEATTACK))
            {
                _enemyModBinds[EEnemyMode.LINKLINEATTACK]();
            }
            if (_curEnemyMods.Contains(EEnemyMode.CROSSATTACK))
            {
                _enemyModBinds[EEnemyMode.CROSSATTACK]();
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
                _curEnemyMods.Add(EEnemyMode.CROSSATTACK);
                break;
            case EEnemyPhase.Phase3:
                _curEnemyMods.Add(EEnemyMode.LINKLINEATTACK);
                _curEnemyMods.Add(EEnemyMode.LINEATTACK);
                break;
            default:
                break;
        }
        curEnemyPhase = target;
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
        EnemyUI_s.Instance.EnemyHPDown();
        EnemyUI_s.Instance.EnemyHPUpdate(_curEnemyHP);
        if (_curEnemyHP <= 0)
        {
            EnemyUI_s.Instance.SingleEnemyAnimation("die", false);
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
        if(_curEnemyMods.Contains(EEnemyMode.CROSSATTACK))
        {
            if (InGameSideData_s.Instance.sideDatas[playerPos.x, playerPos.y].crossAttack != null && CrossAttackPattern.Instance.curCrossAttackMod == ELineAttackMode.ATTACK)
            {
                InGamePlayer_s.Instance.UpdatePlayerHP(-1);
            }
        }
    }
    public void SetObj(string dataName, GameObject instTarget, bool isStack, Transform parent,Vector2Int position)
    {
        int x = position.x;
        int y = position.y;
        if (new Vector2(x, y) == InGamePlayer_s.Instance.playerPos || !InGameSideData_s.Instance.sideDatas[x, y].isCanMakeCheck(isStack, dataName))
        {
            return;
        }
        else
        {
            GameObject instObj = Instantiate(instTarget, parent);
            instObj.transform.localPosition = InGameSideData_s.Instance.sideDatas[x, y].transform;
            TypedReference tr = __makeref(InGameSideData_s.Instance.sideDatas[x, y]);
            InGameSideData_s.Instance.sideDatas[x, y].GetType().GetField(dataName).SetValueDirect(tr, instObj);
        }
    }
    public void GetRandomeObjs(string dataName, GameObject instTarget, bool isStack, Transform parent, int makeCount)
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

