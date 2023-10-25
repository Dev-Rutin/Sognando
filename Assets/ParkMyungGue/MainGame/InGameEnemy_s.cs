using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public partial class InGameEnemy_s : MonoBehaviour,IInGame,IDataSetting,IScript //data
{
    //script
    private InGameManager_s _inGameManager_s;
    private InGameData_s _inGameData_s;
    //data
    public List<EEnemyMode> curEnemyMods;
    [SerializeField] private int _curEnemyHP;
    [SerializeField] private Dictionary<ECubeFace, bool> _enemyHPImageStatus;
    [SerializeField] private int _curEnemyATKGauge;
    [SerializeField] private int _enemyATKGaugeCount;
    //attack pattern
    public bool isEnemyPhaseEnd;
    private Dictionary<EEnemyMode, Action> _enemyModBinds;
    private List<Vector2> _movePathCheckList;
    public Queue<Vector2> playerMovePathQueue;
    public Queue<GameObject> curMovePathShowObj;
    private Vector2Int _curLinkLineAttackPos;
    private Vector2Int _curLinkLineAttackEndPos;
    private Vector2 _curLinkLineAttackDirection;
    private int _curLinkLineAttackCount;
    private ELinkLineAttackMode _curLinkLineAttackMode;
    private ELineAttackMode _curLineAttackMod;
    public bool isEnemyPhase;
    public EEnemyPhase curEnemyPhase;
    private GameObject _curGhost;
    private int _curGhostPos;
    IEnumerator curGhostI;
    public void ScriptBind(InGameManager_s gameManager)
    {
        _inGameManager_s = gameManager;
        _inGameData_s = gameManager.GetInGameDataScript();
    }
    public void DefaultDataSetting()
    {
        //data
        curEnemyMods = new List<EEnemyMode>();
        _curEnemyHP = _inGameData_s.enemyMaxHP;
        _enemyHPImageStatus = new Dictionary<ECubeFace, bool>();
        for (int i = 0; i < Enum.GetValues(typeof(ECubeFace)).Length;i++)
        {
            _enemyHPImageStatus.Add((ECubeFace)Enum.GetValues(typeof(ECubeFace)).GetValue(i), true);
        }
        ShowEnemyHP();
        _curEnemyATKGauge = 0;
        _enemyATKGaugeCount = 0;
        //attack pattern
        isEnemyPhase = false;
        isEnemyPhaseEnd = false;
        _enemyModBinds = new Dictionary<EEnemyMode, Action>();
        BindSetting();
        _movePathCheckList = new List<Vector2>();
        playerMovePathQueue = new Queue<Vector2>();
        curMovePathShowObj = new Queue<GameObject>();
        _curLineAttackMod = ELineAttackMode.NONE;
        _curLinkLineAttackMode = ELinkLineAttackMode.NONE;
        curEnemyPhase = EEnemyPhase.None;
    }
    private void BindSetting()
    {
        EnemyModBind(ref _enemyModBinds, EEnemyMode.PATH, PathAction);
        EnemyModBind(ref _enemyModBinds, EEnemyMode.COIN, CoinAction);
        EnemyModBind(ref _enemyModBinds, EEnemyMode.BLOCK, BlockAction);
        EnemyModBind(ref _enemyModBinds, EEnemyMode.LINEATTACK, LineAttackAction);
        EnemyModBind(ref _enemyModBinds, EEnemyMode.LINKLINEATTACK, LinkLineAttackAction);
        EnemyModBind(ref _enemyModBinds, EEnemyMode.GHOST, GhostAction);
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
    public void GameStart()
    {
        _curEnemyHP = 0;
        UpdateEnemyHP(_inGameData_s.enemyMaxHP);
        _curEnemyATKGauge = 0;
        UpdateEnemyATKGauge(0);
        _enemyATKGaugeCount = 0;
        _movePathCheckList.Clear();
        playerMovePathQueue.Clear();
        curMovePathShowObj.Clear();
        _curLineAttackMod = ELineAttackMode.NONE;
        _curLinkLineAttackMode = ELinkLineAttackMode.NONE;
        isEnemyPhase = false;
        isEnemyPhaseEnd = false;
        curEnemyPhase = EEnemyPhase.None;
        //enemy pattern test
        curEnemyMods.Clear();
        //curEnemyMods.Add(EEnemyMode.COIN);
        //curEnemyMods.Add(EEnemyMode.LINKLINEATTACK);
        //curEnemyMods.Add(EEnemyMode.LINEATTACK);
        //_curEnemyMods.Add(EEnemyMode.BLOCK);
        //_curEnemyMods.Add(EEnemyMode.PATH);

    }
    public void GameEnd()
    {
        isEnemyPhaseEnd = true;
        DoEnemyMode();
        curEnemyMods.Clear();
    }
    public void MoveNextBit(EInGameStatus curInGameStatus)
    {
        switch (curInGameStatus)
        {
            case EInGameStatus.SHOWPATH:
                isEnemyPhaseEnd = EnemyPhaseEndCheck();
                if (isEnemyPhaseEnd)
                {
                    DoEnemyMode();
                    _inGameManager_s.beatFreezeCount = 2;
                }
                break;
            case EInGameStatus.PLAYERMOVE:
                isEnemyPhaseEnd = EnemyPhaseEndCheck();
                if (curEnemyPhase != EEnemyPhase.Ghost&&curEnemyPhase!=EEnemyPhase.Phase1)
                {
                    DoEnemyMode();
                    PlayerPositionCheck(_inGameManager_s.GetPlayerPos(), false);
                    isEnemyPhase = EnemyPhaseCheck();
                    if (!isEnemyPhaseEnd && !isEnemyPhase)
                    {
                        _enemyATKGaugeCount++;
                        if (_enemyATKGaugeCount == 2)
                        {
                            UpdateEnemyATKGauge(1);
                            _enemyATKGaugeCount = 0;
                        }
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

                break;
            case EInGameStatus.PLAYERMOVE:
                TransparentObjs();
                break;
            case EInGameStatus.TIMEWAIT:
                break;
            default:
                break;
        }
    }
    public bool EnemyPhaseCheck()
    {
        foreach(var data in curEnemyMods)
        {
            switch(data)
            {
                case EEnemyMode.LINEATTACK:
                    if (_inGameData_s.lineAttackTsf.childCount != 0)
                    {
                        return true;
                    }
                    break;
                case EEnemyMode.LINKLINEATTACK:
                    if (_inGameData_s.linkLineAttackTsf.childCount != 0)
                    {
                        return true;
                    }
                    break;
            }
        }
        return false;
    }
    public bool EnemyPhaseEndCheck()
    {
        foreach (var data in curEnemyMods)
        {
            switch (data)
            {
                case EEnemyMode.COIN:
                    if (_inGameData_s.coinsTsf.childCount == 0)
                    {
                        return true;
                    }
                    break;
                case EEnemyMode.PATH:
                    if (_inGameData_s.movePathTsf.childCount == 0)
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
        foreach (var data in curEnemyMods)
        {
            _enemyModBinds[data]();
        }
        if (_curEnemyATKGauge >= _inGameData_s.enemyAttackGaugeMax)
        {
            UpdateEnemyATKGauge(_curEnemyATKGauge * -1);
        }
    }
    public void EnemyPhaseChange(EEnemyPhase target)
    {
        switch(target)
        {
            case EEnemyPhase.Ghost:
                if(!curEnemyMods.Contains(EEnemyMode.GHOST))
                {            
                    curEnemyMods.Add(EEnemyMode.GHOST);
                }
                if (!curEnemyMods.Contains(EEnemyMode.COIN))
                {
                    curEnemyMods.Add(EEnemyMode.COIN);
                }
                break;
            case EEnemyPhase.Phase1:
                EndGhost();
                curEnemyMods.Remove(EEnemyMode.GHOST);
                break;
            case EEnemyPhase.Phase2:
                if (curEnemyMods.Contains(EEnemyMode.LINKLINEATTACK))
                {
                    curEnemyMods.Remove(EEnemyMode.LINKLINEATTACK);
                }
                curEnemyMods.Add(EEnemyMode.LINEATTACK);
                break;
            case EEnemyPhase.Phase3:
                curEnemyMods.Remove(EEnemyMode.LINEATTACK);
                curEnemyMods.Add(EEnemyMode.LINKLINEATTACK);
                break;
        }
        curEnemyPhase = target;
    }
}
public partial class InGameEnemy_s //ui,data change
{
    private void ShowEnemyHP()
    {
        foreach (var data in _enemyHPImageStatus)
        {
            if (data.Value)
            {
                _inGameData_s.enemyHPTsf.Find(data.Key.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>("ParkMyungGue\\CubeSideImage\\ON\\" + data.Key.ToString());
            }
            else
            {
                _inGameData_s.enemyHPTsf.Find(data.Key.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>("ParkMyungGue\\CubeSideImage\\OFF\\" + data.Key.ToString());
            }
        }
    }
    public void UpdateEnemyHP(int changevalue)
    {
        _curEnemyHP += changevalue;
        if (changevalue == _inGameData_s.enemyMaxHP)
        {
            EnemyHPReset();
        }
        else if (changevalue < 0)
        {
            EnemyHPDown();
        }
        ShowEnemyHP();
    }
    private void EnemyHPReset()
    {
        var temp = new Dictionary<ECubeFace,bool>(_enemyHPImageStatus);
        foreach (var data in temp)
        {
            _enemyHPImageStatus[data.Key] = true;
        }
    }
    private void EnemyHPDown()
    {
        _inGameData_s.playerAttackEffectObj.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _inGameData_s.enemyHitEffectObj.GetComponent<ParticleSystem>().Play();
        _inGameData_s.enemyImageObj.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "hit", false);
        _enemyHPImageStatus[_inGameManager_s.curFace] = false;
        if (_curEnemyHP <= 0)
        {
            _inGameData_s.enemyImageObj.GetComponent<SkeletonAnimation>().AnimationState.AddAnimation(0, "death", false, 0f);
            _inGameManager_s.GameOverByEnemy();
        }
        else
        {
            _inGameData_s.enemyImageObj.GetComponent<SkeletonAnimation>().AnimationState.AddAnimation(0, "idle", true, 0f);
        }
    }
    public void UpdateEnemyATKGauge(int changevalue) //need change!!!!!!!!!!!!!!!!!!!! to like ShowEnemyHP()
    {
        _curEnemyATKGauge += changevalue;
        int count = 0;
        foreach (Transform data in _inGameData_s.enemyATKOnTsf)
        {
            if (count < _curEnemyATKGauge)
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
public partial class InGameEnemy_s //pattern
{
    public void PlayerPositionCheck(Vector2Int playerPos,bool isMoved)
    {
        if (isMoved)
        {
            if (curEnemyMods.Contains(EEnemyMode.COIN))
            {
                if (_inGameData_s.sideDatas[playerPos.x, playerPos.y].coin != null)
                {
                    _inGameData_s.sideDatas[playerPos.x, playerPos.y].coincount--;
                    if (_inGameData_s.sideDatas[playerPos.x, playerPos.y].coincount == 0)
                    {
                        RemoveTargetObj("coin", playerPos.x, playerPos.y);
                    }
                    else
                    {
                        _inGameData_s.sideDatas[playerPos.x, playerPos.y].coin.GetComponent<Image>().color = new Vector4(_inGameData_s.sideDatas[playerPos.x, playerPos.y].coin.GetComponent<Image>().color.r, _inGameData_s.sideDatas[playerPos.x, playerPos.y].coin.GetComponent<Image>().color.g, _inGameData_s.sideDatas[playerPos.x, playerPos.y].coin.GetComponent<Image>().color.b, 0.7f);
                    }
                }
                if (_inGameData_s.sideDatas[playerPos.x, playerPos.y].fire != null)
                {
                    _inGameManager_s.PlayerHPDown(-1, "Fire!!");
                }
            }
        }
        else
        {
            if (curEnemyMods.Contains(EEnemyMode.PATH))
            {
                UpdatePath();
            }
            if (curEnemyMods.Contains(EEnemyMode.LINEATTACK))
            {
                if (_inGameData_s.sideDatas[playerPos.x, playerPos.y].lineAttack != null && _curLineAttackMod == ELineAttackMode.SHOW2)
                {
                    _inGameManager_s.PlayerHPDown(-1, "Line Attack!!!");
                }
            }
            if (curEnemyMods.Contains(EEnemyMode.LINKLINEATTACK))
            {
                if (_inGameData_s.sideDatas[playerPos.x, playerPos.y].linkLineAttack != null && _curLinkLineAttackMode==ELinkLineAttackMode.ATTACK)
                {
                    _inGameManager_s.PlayerHPDown(-1, "Link Line Attack!!!");
                }
            }
        }
    }
    private void TransparentObjs()
    {
        if (curEnemyMods.Contains(EEnemyMode.COIN))
        {
            foreach (Transform data in _inGameData_s.coinsTsf)
            {
                data.GetComponent<Image>().color = new Vector4(data.GetComponent<Image>().color.r, data.GetComponent<Image>().color.g, data.GetComponent<Image>().color.b, 0.9f);
            }
        }
        if (curEnemyMods.Contains(EEnemyMode.PATH))
        {
            foreach (Transform data in _inGameData_s.movePathTsf)
            {
                data.GetComponent<Image>().color = new Vector4(data.GetComponent<Image>().color.r, data.GetComponent<Image>().color.g, data.GetComponent<Image>().color.b, 0.5f);
            }
        }
    }
    private void GetRandomeObjs(string dataName, GameObject instTarget, bool isStack, Transform parent, int makeCount)
    {
        for (int i = 0; i < makeCount; i++)
        {
            if(curEnemyPhase==EEnemyPhase.Ghost&&i ==0&&dataName=="coin")
            {
                int x = 2;
                int y = 1;
                GameObject instObj = Instantiate(instTarget, parent);
                instObj.transform.localPosition = _inGameData_s.sideDatas[x, y].transform;
                TypedReference tr = __makeref(_inGameData_s.sideDatas[x, y]);
                _inGameData_s.sideDatas[x, y].GetType().GetField(dataName).SetValueDirect(tr, instObj);
                if (dataName == "coin")
                {
                    _inGameData_s.sideDatas[x, y].coincount = 2;
                }
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
                int x = UnityEngine.Random.Range(0, _inGameData_s.divideSize.x);
                int y = UnityEngine.Random.Range(0, _inGameData_s.divideSize.y);
                if (new Vector2(x, y) == _inGameManager_s.GetPlayerPos() || !_inGameData_s.sideDatas[x, y].isCanMakeCheck(isStack, dataName))
                {
                    continue;
                }
                else
                {
                    GameObject instObj = Instantiate(instTarget, parent);
                    instObj.transform.localPosition = _inGameData_s.sideDatas[x, y].transform;
                    TypedReference tr = __makeref(_inGameData_s.sideDatas[x, y]);
                    _inGameData_s.sideDatas[x, y].GetType().GetField(dataName).SetValueDirect(tr, instObj);
                    if (dataName == "coin")
                    {
                        _inGameData_s.sideDatas[x, y].coincount = 2;
                    }
                    isCreate = true;
                }
                block++;
            }
        }
    }
    private void RemoveTargetObj(string dataName, int xpos, int ypos)
    {
        TypedReference tr = __makeref(_inGameData_s.sideDatas[xpos, ypos]);
        object target = _inGameData_s.sideDatas[xpos, ypos].GetType().GetField(dataName).GetValueDirect(tr);
        if (target != null)
        {
            if (target.GetType().Equals(typeof(GameObject)))
            {
                Destroy((GameObject)target);
                _inGameData_s.sideDatas[xpos, ypos].GetType().GetField(dataName).SetValueDirect(tr, null);
                if (dataName == "coin")
                {
                    _inGameData_s.sideDatas[xpos, ypos].coincount = 0;
                }
            }
        }
    }
    private void RemoveAllTargetObj(string dataName)
    {
        for (int i = 0; i < _inGameData_s.divideSize.x; i++)
        {
            for (int j = 0; j < _inGameData_s.divideSize.y; j++)
            {
                RemoveTargetObj(dataName, i, j);
            }
        }
    }
    private void GetRandomePath()//only test
    {
        Vector2 movedPlayerPos = _inGameManager_s.GetPlayerPos();
        for (int i = 0; i < 10; i++)
        {
            _movePathCheckList.Clear();
            if (movedPlayerPos.x - 1 >= 0)
            {
                _movePathCheckList.Add(new Vector2(movedPlayerPos.x - 1, movedPlayerPos.y));
            }
            if (movedPlayerPos.x + 1 <= _inGameData_s.divideSize.x - 1)
            {
                _movePathCheckList.Add(new Vector2(movedPlayerPos.x + 1, movedPlayerPos.y));
            }
            if (movedPlayerPos.y - 1 >= 0)
            {
                _movePathCheckList.Add(new Vector2(movedPlayerPos.x, movedPlayerPos.y - 1));
            }
            if (movedPlayerPos.y + 1 <= _inGameData_s.divideSize.y - 1)
            {
                _movePathCheckList.Add(new Vector2(movedPlayerPos.x, movedPlayerPos.y + 1));
            }
            _movePathCheckList.Remove(_inGameManager_s.GetPlayerPos());
            foreach (var data in playerMovePathQueue)
            {
                _movePathCheckList.Remove(data);
            }
            if (_movePathCheckList.Count != 0)
            {
                Vector2 enqueuedata = _movePathCheckList[UnityEngine.Random.Range(0, _movePathCheckList.Count)];
                playerMovePathQueue.Enqueue(enqueuedata);
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
        if (playerMovePathQueue.Count != 0)
        {
            int count = 0;
            _inGameData_s.movePathTsf.GetComponent<LineRenderer>().positionCount = playerMovePathQueue.Count >= 5 ? 5 : playerMovePathQueue.Count;
            _inGameData_s.movePathTsf.GetComponent<LineRenderer>().SetPosition(count, _inGameManager_s.GetPlayerObj().transform.position);
            count++;
            foreach (var data in playerMovePathQueue)
            {
                if (data.x != -1 * (_inGameData_s.divideSize.x - 1) && data.y != -1 * (_inGameData_s.divideSize.y - 1))
                {
                    GameObject instPath = Instantiate(_inGameData_s.pathSampleObj,_inGameData_s.movePathTsf);
                    instPath.transform.localPosition = _inGameData_s.sideDatas[(int)data.x, (int)data.y].transform;
                    if (count < 5)
                    {
                        instPath.SetActive(true);
                        _inGameData_s.movePathTsf.GetComponent<LineRenderer>().SetPosition(count, instPath.transform.position);
                        curMovePathShowObj.Enqueue(instPath);
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
        if (playerMovePathQueue.Count == 0)
        {
            if (_inGameData_s.movePathTsf.childCount != 0)
            {
                for (int i = 0; i < _inGameData_s.movePathTsf.childCount; i++)
                {
                    Destroy(_inGameData_s.movePathTsf.GetChild(i).gameObject);
                }
            }
            _inGameData_s.movePathTsf.GetComponent<LineRenderer>().positionCount = 1;
        }
        else if (_inGameManager_s.curInGameStatus == EInGameStatus.PLAYERMOVE)
        {
            if (_inGameManager_s.GetPlayerPos() == playerMovePathQueue.Peek())
            {
                playerMovePathQueue.Dequeue();
                Destroy(curMovePathShowObj.Dequeue());
            }
            if (curMovePathShowObj.Count == 1)
            {
                foreach (Transform data in _inGameData_s.movePathTsf)
                {
                    if (!data.gameObject.activeSelf)
                    {
                        data.gameObject.SetActive(true);
                        curMovePathShowObj.Enqueue(data.gameObject);
                        break;
                    }
                }
            }
            int count = 0;
            _inGameData_s.movePathTsf.GetComponent<LineRenderer>().positionCount = curMovePathShowObj.Count + 1;
            _inGameData_s.movePathTsf.GetComponent<LineRenderer>().SetPosition(count, _inGameManager_s.GetPlayerObj().transform.position);
            foreach (var data in curMovePathShowObj)
            {
                count++;
                _inGameData_s. movePathTsf.GetComponent<LineRenderer>().SetPosition(count, data.transform.position);
            }
        }
    }
    private void PathAction()
    {
        if (_inGameManager_s.curInGameStatus == EInGameStatus.SHOWPATH)
        {
            GetRandomePath();
            ShowPath();
        }
        else
        {
            if (isEnemyPhaseEnd)
            {
                //_rotateTarget = ERotatePosition.NONE;
                playerMovePathQueue.Clear();
                curMovePathShowObj.Clear();
                UpdatePath();
            }
        }
    }
    private void GetRandomeCoin()
    {
        GetRandomeObjs("coin", _inGameData_s.coinSampleObj, true,_inGameData_s.coinsTsf, 5);
    }
    private void EndCoin()
    {
        RemoveAllTargetObj("coin");
    }
    private void GetRandomeFire()
    {
        GetRandomeObjs("fire", _inGameData_s.fireSampleObj, false, _inGameData_s.fireTsf, 5);
    }
    private void EndFire()
    {
        RemoveAllTargetObj("fire");
    }
    private void CoinAction()
    {
        if (_inGameManager_s.curInGameStatus == EInGameStatus.SHOWPATH)
        {
            GetRandomeCoin();
            //GetRandomeFire();
        }
        else
        {
            if (isEnemyPhaseEnd)
            {
                EndCoin();
                //EndFire();
            }
        }
    }
    private void GetRandomeBlock()
    {
        GetRandomeObjs("wall", _inGameData_s.blockSampleObj, false, _inGameData_s.blockTsf, 3);
    }
    private void EndBlock()
    {
        RemoveAllTargetObj("wall");
    }
    private void BlockAction()
    {
        if (_inGameManager_s.curInGameStatus == EInGameStatus.SHOWPATH)
        {
            GetRandomeBlock();
        }
        else
        {
            if (isEnemyPhaseEnd)
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
            int y = _inGameManager_s.GetPlayerPos().y;
            for (int i = 0; i < _inGameData_s.divideSize.x; i++)
            {
                if (_inGameData_s.sideDatas[i, y].isCanMakeCheck(true, "lineAttack"))
                {
                    GameObject instObj = Instantiate(_inGameData_s.lineAttackSampleObj, _inGameData_s.lineAttackTsf);
                    instObj.transform.Find("Attack").gameObject.SetActive(false);
                    instObj.transform.localPosition = _inGameData_s.sideDatas[i, y].transform;
                    _inGameData_s.sideDatas[i, y].lineAttack = instObj;
                }
            }
        }
        else // columns attack
        {
            int x = _inGameManager_s.GetPlayerPos().x;
            for (int i = 0; i < _inGameData_s.divideSize.y; i++)
            {
                if (_inGameData_s.sideDatas[x, i].isCanMakeCheck(true, "lineAttack"))
                {
                    GameObject instObj = Instantiate(_inGameData_s.lineAttackSampleObj, _inGameData_s.lineAttackTsf);
                    instObj.transform.Find("Attack").gameObject.SetActive(false);
                    instObj.transform.localPosition = _inGameData_s.sideDatas[x, i].transform;
                    _inGameData_s.sideDatas[x, i].lineAttack = instObj;
                }
            }
        }
        _curLineAttackMod = ELineAttackMode.SHOW1;
        foreach (Transform data in _inGameData_s.lineAttackTsf)
        {
            data.Find("WarningTile").gameObject.SetActive(true);
            data.Find("WarningTile").Find("Effect").GetComponent<ParticleSystem>().Play();
        }
    }
    private void EndRandomeLineAttack()
    {
        RemoveAllTargetObj("lineAttack");
        _curLineAttackMod = ELineAttackMode.NONE;
    }
    private void LineAttackAction()
    {
        if (_inGameManager_s.curInGameStatus == EInGameStatus.SHOWPATH)
        {
            //GetRandomeLineAttack();
        }
        else
        {
            if (isEnemyPhaseEnd)
            {
                EndRandomeLineAttack();
            }
            else
            {
                if (_curEnemyATKGauge >= _inGameData_s.enemyAttackGaugeMax)
                {
                    GetRandomeLineAttack();
                }
                else if(_curLineAttackMod == ELineAttackMode.SHOW1)
                {
                    _curLineAttackMod = ELineAttackMode.SHOW2;
                    foreach (Transform data in _inGameData_s.lineAttackTsf)
                    {
                        data.Find("1").gameObject.SetActive(false);
                    }
                    foreach (Transform data in _inGameData_s.lineAttackTsf)
                    {
                        data.Find("2").gameObject.SetActive(true);
                    }
                    foreach (Transform data in _inGameData_s.lineAttackTsf)
                    {
                        data.Find("WarningTile").Find("Effect").GetComponent<ParticleSystem>().Play();
                    }
                }
                else if (_curLineAttackMod == ELineAttackMode.SHOW2)
                {
                    _curLineAttackMod = ELineAttackMode.ATTACK;
                    foreach (Transform data in _inGameData_s.lineAttackTsf)
                    {
                        data.Find("2").gameObject.SetActive(false);
                    }
                    foreach (Transform data in _inGameData_s.lineAttackTsf)
                    {
                        data.Find("Attack").gameObject.SetActive(true);
                        StartCoroutine(LineAttackMove(data.Find("Attack").gameObject));
                    }
                    foreach (Transform data in _inGameData_s.lineAttackTsf)
                    {
                        data.Find("WarningTile").gameObject.SetActive(false);
                    }
                }
                else if (_curLineAttackMod == ELineAttackMode.ATTACK)
                {
                    EndRandomeLineAttack();
                }
            }
        }
    }
    IEnumerator LineAttackMove(GameObject target)
    {
        float lerpValue = 0;
        float NiddleMoveStartTime = _inGameManager_s.GetMusicPosition();
        Vector2 NiddleStartPos = new Vector2(0, 35); 
        Vector2 NiddleEndPos = new Vector2(0, -10);
        while (lerpValue <= 1)
        {
            lerpValue = (_inGameManager_s.GetMusicPosition() - NiddleMoveStartTime) * 1 / _inGameData_s.movingTime;
            target.transform.localPosition = Vector2.Lerp(NiddleStartPos, NiddleEndPos, lerpValue);
            yield return null;
        }
        target.transform.localPosition = NiddleEndPos;
        float alpha = 1f;
        Debug.Log(_inGameManager_s.GetMusicPosition() - NiddleMoveStartTime);
        Debug.Log((60f / _inGameData_s.bpm) - 0.1f);
        while (_inGameManager_s.GetMusicPosition()- NiddleMoveStartTime<(60f/_inGameData_s.bpm)-0.1f)
        {
            target.GetComponent<Image>().color = new Vector4(1, 1, 1,alpha);
            alpha -= 0.05f;
            yield return null;
        }
        target.GetComponent<Image>().color = new Vector4(1, 1, 1, 0);
    }
    private void GetRandomeLinkLineAttack()
    {
        int count = UnityEngine.Random.Range(0, 2);
        int secondcount = UnityEngine.Random.Range(0, 2);
        if (count == 0) // row attack
        {
            int y = _inGameManager_s.GetPlayerPos().y;
            if (secondcount == 0)// left
            {
                _curLinkLineAttackPos = new Vector2Int(0, y);
                _curLinkLineAttackEndPos = new Vector2Int(_inGameData_s.divideSize.x - 1, y);
         
            }
            else
            {
                _curLinkLineAttackPos = new Vector2Int(_inGameData_s.divideSize.x - 1, y);
                _curLinkLineAttackEndPos = new Vector2Int(0, y);
             
            }
        }
        else // columns attack
        {
            int x = _inGameManager_s.GetPlayerPos().x;
            if (secondcount == 0)
            {
                _curLinkLineAttackPos = new Vector2Int(x, 0);
                _curLinkLineAttackEndPos = new Vector2Int(x, _inGameData_s.divideSize.y-1);
            }
            else
            {
                _curLinkLineAttackPos = new Vector2Int(x, _inGameData_s.divideSize.y - 1);
                _curLinkLineAttackEndPos = new Vector2Int(x, 0);
            }
        }
        GameObject instObj= Instantiate(_inGameData_s.linkLineAttackSampleObj, _inGameData_s.linkLineAttackTsf);
        instObj.transform.localPosition = Vector2.zero;
        _curLinkLineAttackDirection = new Vector2(_curLinkLineAttackEndPos.x - _curLinkLineAttackPos.x, _curLinkLineAttackEndPos.y - _curLinkLineAttackPos.y).normalized;
        Vector2 direction = Vector2.zero;
        if(_curLinkLineAttackDirection == Vector2.down)
        {
            direction = new Vector2(0, -150f);
            instObj.transform.Find("Warning").transform.localEulerAngles = new Vector3(0, 0, 90);
        }
        else if(_curLinkLineAttackDirection == Vector2.up)
        {
            direction = new Vector2(0, 150f);
            instObj.transform.Find("Warning").transform.localEulerAngles = new Vector3(0, 0, -90);
        }
        else if (_curLinkLineAttackDirection == Vector2.left)
        {
            direction = new Vector2(150f,0);
            instObj.transform.Find("Warning").transform.localEulerAngles = new Vector3(0, 0, 180);
        }
        else if (_curLinkLineAttackDirection == Vector2.right)
        {
            direction = new Vector2(-150f, 0);
        }
        instObj.transform.Find("Warning").transform.localPosition = _inGameData_s.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].transform+ direction;
        instObj.transform.Find("Attack").transform.localPosition = _inGameData_s.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].transform;
        instObj.transform.Find("Attack").gameObject.SetActive(false);
        _inGameData_s.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].linkLineAttack = instObj;
        _inGameData_s.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].linkLineAttack.transform.Find("WarningTile").localPosition =
    _inGameData_s.sideDatas[_curLinkLineAttackPos.x , _curLinkLineAttackPos.y ].transform;
        instObj.transform.Find("WarningTile").gameObject.SetActive(true);
        _curLinkLineAttackMode = ELinkLineAttackMode.SHOW1;
    }
    private void EndRandomeLinkLineAttack()
    {
        _curLinkLineAttackMode = ELinkLineAttackMode.NONE;
        RemoveAllTargetObj("linkLineAttack");
    }
    private void LinkLineAttackAction()
    {
        if (_inGameManager_s.curInGameStatus == EInGameStatus.SHOWPATH)
        {
            //GetRandomeLineAttack();
        }
        else
        {
            if (isEnemyPhaseEnd)
            {
                EndRandomeLinkLineAttack();
            }
            else
            {
                if (_curEnemyATKGauge >= _inGameData_s.enemyAttackGaugeMax && _curLinkLineAttackMode == ELinkLineAttackMode.NONE)
                {
                    GetRandomeLinkLineAttack();
                }
                else
                {
                    switch (_curLinkLineAttackMode)
                    {
                        case ELinkLineAttackMode.SHOW1:
                            _curLinkLineAttackMode = ELinkLineAttackMode.SHOW2;
                            _inGameData_s.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].linkLineAttack.transform.Find("WarningTile").gameObject.SetActive(true);
                            _inGameData_s.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].linkLineAttack.transform.Find("WarningTile").Find("Effect").GetComponent<ParticleSystem>().Play();
                            break;
                        case ELinkLineAttackMode.SHOW2:
                            _curLinkLineAttackMode = ELinkLineAttackMode.ATTACK;
                            _inGameData_s.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].linkLineAttack.transform.Find("Attack").gameObject.SetActive(true);
                            _curLinkLineAttackCount = 1;                     
                            _inGameData_s.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].linkLineAttack.transform.Find("WarningTile").localPosition =
                                _inGameData_s.sideDatas[_curLinkLineAttackPos.x + (int)_curLinkLineAttackDirection.x, _curLinkLineAttackPos.y + (int)_curLinkLineAttackDirection.y].transform;
                            _inGameData_s.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].linkLineAttack.transform.Find("WarningTile").Find("Effect").GetComponent<ParticleSystem>().Play();
                            break;
                        case ELinkLineAttackMode.ATTACK:
                            if (_curLinkLineAttackPos == _curLinkLineAttackEndPos)
                            {
                                EndRandomeLinkLineAttack();
                            }
                            else
                            {                           
                                GameObject temp = _inGameData_s.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].linkLineAttack;
                                if (_curLinkLineAttackCount == 2)
                                {
                                    _inGameData_s.sideDatas[_curLinkLineAttackPos.x - (int)_curLinkLineAttackDirection.x, _curLinkLineAttackPos.y - (int)_curLinkLineAttackDirection.y].linkLineAttack = null;
                                }
                                else
                                {
                                    _curLinkLineAttackCount++;
                                }
                                _curLinkLineAttackPos += new Vector2Int((int)_curLinkLineAttackDirection.x, (int)_curLinkLineAttackDirection.y);
                                _inGameData_s.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].linkLineAttack = temp;
                                temp.transform.Find("Attack").transform.localPosition = _inGameData_s.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].transform;
                                if (_curLinkLineAttackPos != _curLinkLineAttackEndPos)
                                {
                                    _inGameData_s.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].linkLineAttack.transform.Find("WarningTile").localPosition =
                                    _inGameData_s.sideDatas[_curLinkLineAttackPos.x + (int)_curLinkLineAttackDirection.x, _curLinkLineAttackPos.y + (int)_curLinkLineAttackDirection.y].transform;
                                    _inGameData_s.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].linkLineAttack.transform.Find("WarningTile").Find("Effect").GetComponent<ParticleSystem>().Play();
                                }
                                else
                                {
                                    _inGameData_s.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].linkLineAttack.transform.Find("WarningTile").gameObject.SetActive(false);
                                }
                            }
                            break;
                    }


                }
            }
        }
    }
    public void SetGhost() //1,3
    {
      _curGhost=  Instantiate(_inGameData_s.ghostSampleObj, _inGameData_s.ghostTsf);
        _curGhost.transform.localPosition = _inGameData_s.sideDatas[0, 0].transform;
        _curGhostPos = 0;
    }
    public void EndGhost()
    {
        if (curGhostI != null)
        {
            StopCoroutine(curGhostI);
        }
        Destroy(_curGhost);
    }
    public void GhostAction()
    {
        if (_inGameManager_s.curInGameStatus == EInGameStatus.SHOWPATH)
        {
            SetGhost();
        }
        else
        {
            Vector2 moveTarget = Vector2.zero;
            switch (_curGhostPos)
            {
                case 0:
                    moveTarget = _inGameData_s.sideDatas[1, 0].transform;
                    break;
                case 1:
                    moveTarget = _inGameData_s.sideDatas[2, 0].transform;
                    break;
                case 2:
                    moveTarget = _inGameData_s.sideDatas[2, 1].transform;
                    break;
                default:
                    break;
            }
            _curGhostPos++;
            if (_curGhostPos == 3)
            {
                _curGhostPos = 0;
            }
           curGhostI=MoveGhost(moveTarget);
            StartCoroutine(curGhostI);
        }
    }
    public IEnumerator MoveGhost(Vector2 targetPos)
    {
        float lerpValue = 0;
        float GhostMoveStartTime = _inGameManager_s.GetMusicPosition();
        Vector2 GhostStartPos = _curGhost.transform.localPosition;
        Vector2 GhostEndPos = targetPos;
        while (lerpValue <= 1)
        {
            lerpValue = (_inGameManager_s.GetMusicPosition() - GhostMoveStartTime) * 1 / _inGameData_s.movingTime;
            _curGhost.transform.localPosition= Vector2.Lerp(GhostStartPos,GhostEndPos, lerpValue);
            yield return null;
        }
        _curGhost.transform.localPosition = GhostEndPos;
        if (_curGhostPos == 0)
        {
            _curGhost.transform.localPosition = _inGameData_s.sideDatas[0, 0].transform;
        }
    }
    /*private void GetRandomeNeedleAttack()
    {
        int count = UnityEngine.Random.Range(0, 2);
        int secondcount = UnityEngine.Random.Range(0, 2);
        if (count == 0) // row attack
        {
            int y = UnityEngine.Random.Range(0, _arrY);
            if (secondcount == 0)
            {
                for (int i = 0; i < _arrX; i++)
                {
                    if (_cubeDatas[i, y].isCanMakeCheck(true, "needle"))
                    {
                        GameObject instObj = Instantiate(_needleAttackSampleObj, _needleAttackTsf);
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
    GameObject curNeedle;
    float needleStartMusicPos;
    IEnumerator StartNeedle(GameObject target)
    {
        curNeedle = target;
        needleStartMusicPos = _audioManager.curMainMusicPosition;
        yield return new WaitForSeconds(_bpm * 4);
        curNeedle = null;
    }*/
}

