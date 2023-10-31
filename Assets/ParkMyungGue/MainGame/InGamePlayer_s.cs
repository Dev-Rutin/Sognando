using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public partial class InGamePlayer_s : MonoBehaviour,IDataSetting,IScript,IInGame//data
{
  
    private InGameManager_s _inGameManager_s;
    private InGameData_s _inGameData_s;
    [Header("player data")]
    public GameObject inGamePlayerObj;
    public Vector2Int playerPos;
    [SerializeField]private int _curPlayerHP;
    private bool _isHPDown;
    private int _playerHitBlock;
    [Header("player move")]
    private float playerMoveStartTime;
    private Vector2 playerMoveStartPos;
    private Vector2 playerMoveEndPos;   
    private float lerpValue;
    public bool IsplayerMoveCheck;
    private IEnumerator moveFun;
    public void ScriptBind(InGameManager_s gameManager)
    {
        _inGameManager_s = gameManager;
        _inGameData_s = gameManager.GetInGameDataScript();
    }
    public void DefaultDataSetting()
    {
        inGamePlayerObj = _inGameData_s.inGamePlayerObj;
    }
}
public partial class InGamePlayer_s//game system
{
    public void GameStart()
    {
        playerPos = Vector2Int.zero;
        inGamePlayerObj.transform.localPosition = _inGameData_s.sideDatas[playerPos.x, playerPos.y].transform;
        _curPlayerHP = 0;
        UpdatePlayerHP(_inGameData_s.playerMaxHP);
        _isHPDown = false;
        _playerHitBlock = 0;
        playerMoveStartTime = 0;
        playerMoveStartPos = Vector2.zero;
        playerMoveEndPos = Vector2.zero;
        lerpValue = 0;
        IsplayerMoveCheck = false;
    }
    public void GameEnd()
    {
        StopAllCoroutines();
    }
    private void Update()
    {
    }
    public void MoveNextBit(EInGameStatus curInGameStatus)
    {
        switch (curInGameStatus)
        {
            case EInGameStatus.SHOWPATH:
                break;
            case EInGameStatus.PLAYERMOVE:
                if (_inGameManager_s.curInputMode == EInputMode.MAINTAIN)
                {
                    IsplayerMoveCheck = false;
                    inGamePlayerObj.transform.Find("Effect").GetComponent<ParticleSystem>().Stop();
                    if (_isHPDown && _playerHitBlock == 0)
                    {
                        _isHPDown = false;
                        Color playerColor = inGamePlayerObj.GetComponent<Image>().color;
                        playerColor.a = 1f;
                        inGamePlayerObj.GetComponent<Image>().color = playerColor;
                    }
                    else if (_playerHitBlock > 0)
                    {
                        _playerHitBlock--;
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

    }
}
public partial class InGamePlayer_s  //move
{  
    public void MovePlayer(Vector2Int movePos, Vector2Int divideSize)
    {
        Vector2Int movedPosition = playerPos + movePos;
        movedPosition.x = movedPosition.x < 0 ? 0 : movedPosition.x;
        movedPosition.x = movedPosition.x > divideSize.x - 1 ? divideSize.x - 1 : movedPosition.x;
        movedPosition.y = movedPosition.y < 0 ? 0 : movedPosition.y;
        movedPosition.y = movedPosition.y > divideSize.y - 1 ? divideSize.y - 1 : movedPosition.y;
        playerPos = movedPosition;
        if (moveFun != null)
        {
            PlayerPostionChange(playerMoveEndPos);
            StopCoroutine(moveFun);
        }
        moveFun = MoveTimeLock();
        StartCoroutine(MoveTimeLock());
    }
    public IEnumerator MoveTimeLock()
    {
        IsplayerMoveCheck = true;
        lerpValue = 0;
        playerMoveStartTime = _inGameManager_s.GetMusicPosition();
        playerMoveStartPos = inGamePlayerObj.transform.localPosition;
        playerMoveEndPos = _inGameData_s.sideDatas[playerPos.x, playerPos.y].transform;
        while(lerpValue<=1&&_inGameManager_s.curGameStatus==EGameStatus.PLAYING)
        {
            lerpValue = (_inGameManager_s.GetMusicPosition() - playerMoveStartTime) * 1 / _inGameData_s.movingTime;
            PlayerPostionChange(Vector2.Lerp(playerMoveStartPos, playerMoveEndPos, lerpValue));
            yield return null;
        }
        PlayerPostionChange(playerMoveEndPos);
        inGamePlayerObj.transform.Find("Effect").GetComponent<ParticleSystem>().Play(); //player move end effect
        _inGameManager_s.PlayerPostionCheck_InGame(playerPos, true);
        moveFun = null;
    }
    private void PlayerPostionChange(Vector2 position)
    {
        inGamePlayerObj.transform.localPosition = position;
        if (_inGameManager_s.GetEnemyMods().Contains(EEnemyMode.PATH))
        {
            _inGameManager_s.UpdateMovePathLine();
        }
    }
    public bool PlayerMoveCheck(Vector2Int movePos, Vector2Int divideSize)//그 포지션으로 이동이 가능한지 불가능한지 판단
    {
        Vector2Int movedPosition = playerPos + movePos;
        movedPosition.x = movedPosition.x < 0 ? 0 : movedPosition.x;
        movedPosition.x = movedPosition.x > divideSize.x - 1 ? divideSize.x - 1 : movedPosition.x;
        movedPosition.y = movedPosition.y < 0 ? 0 : movedPosition.y;
        movedPosition.y = movedPosition.y > divideSize.y - 1 ? divideSize.y - 1 : movedPosition.y;
        if (movedPosition == playerPos)
        {
            return false;
        }
        if (_inGameManager_s.GetEnemyMods().Contains(EEnemyMode.PATH))
        {
            if (_inGameManager_s.GetPlayerMovePathQueue().Count != 0)
            {
                if (_inGameManager_s.GetPlayerMovePathQueue().Peek() != movedPosition)
                {
                    _inGameManager_s.PlayerHPDown(-1, "WorngPath");
                    return false;
                }
            }
        }
        if (_inGameManager_s.GetEnemyMods().Contains(EEnemyMode.BLOCK))
        {
            if (_inGameData_s.sideDatas[movedPosition.x, movedPosition.y].wall != null)
            {
                return false;
            }
        }
        return true;
    }
 
}
public partial class InGamePlayer_s  //ui,data change
{
    public void UpdatePlayerHP(int changeValue)
    {
        if (changeValue > 0)
        {
            PlayerHPUp(changeValue);
        }
        else if (changeValue < 0)
        {
            PlayerHPDown(changeValue);
        }
        int count = 0;
        foreach (Transform data in _inGameData_s.playerHPOffTsf)
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
    private void PlayerHPUp(int changeValue)
    {
        _inGameData_s.playerHealEffect.GetComponent<ParticleSystem>().Play();
        _curPlayerHP += changeValue;
        _curPlayerHP = _curPlayerHP > _inGameData_s.playerMaxHP ? _inGameData_s.playerMaxHP : _curPlayerHP;
    }
    private void PlayerHPDown(int changeValue)
    {
        if (!_isHPDown)
        {
            Color playerColor = inGamePlayerObj.GetComponent<Image>().color;
            playerColor.a = 0.7f;
            inGamePlayerObj.GetComponent<Image>().color = playerColor;
            _inGameData_s.playerHurtEffect.GetComponent<ParticleSystem>().Play();
            _curPlayerHP += changeValue;
            //InGameUI.ShowCharacterAnimation(new List<string>() { "hit" },_inGameData_s.playerImageObj);
            if (_curPlayerHP <= 0)
            {
                _inGameManager_s.GameOverByPlayer();
            }
            _isHPDown = true;
            _playerHitBlock = 1;
            _inGameManager_s.UpdateCombo(_inGameManager_s.combo * -1);
        }
    }
    
    public IEnumerator PlayerAttack()
    {
        float count = 0;
        Transform AttackTsf = _inGameData_s.playerAttackEffectObj.transform;
        WaitForSeconds wait = new WaitForSeconds(0.03f);
        while (count <=0.5f)
        {
            AttackTsf.transform.localPosition = 
                Vector2.Lerp(_inGameData_s.playerAttackEffectObj.transform.parent.Find("HitStartPos").transform.localPosition, _inGameData_s.playerAttackEffectObj.transform.parent.Find("HitEndPos").transform.localPosition,count/0.5f);
            yield return wait;
            count += 0.03f;
        }
        _inGameManager_s.EnemyHPChange(-1);
        yield return new WaitForSeconds(0.1f);
        AttackTsf.transform.gameObject.SetActive(false);
        AttackTsf.transform.localPosition = _inGameData_s.playerAttackEffectObj.transform.parent.Find("HitStartPos").transform.localPosition;
        yield return new WaitForSeconds(1.8f - 0.02f - 0.1f - 0.5f);
        AttackTsf.transform.gameObject.SetActive(true);
    }
}
