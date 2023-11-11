using Spine.Unity;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public partial class InGamePlayer_s : MonoBehaviour,IInGame,IScript//data
{
    [Header("script")]
    private ScriptManager_s _scripts;
    [SerializeField] private PlayerUI_s _playerUI_s;
    [SerializeField] private DoremiUI_s _doremiUI_s;

    [Header("player data")]
    [SerializeField] private GameObject _playerObj;
    private Image _playerImage;
    public Vector2Int playerPos { get; private set; }
    [SerializeField] private float _movingTime;
    [SerializeField] private int _playerMaxHP;
    [SerializeField]private int _curPlayerHP;
    private EPlayerAttackLevel _playerAttackLevel;
    private bool _isGracePeriod;
    private int _playerHitBlock;
    [SerializeField] private ParticleSystem _moveEffect;
    [SerializeField] private ParticleSystem _noiseDissolveEffect;
    public void ScriptBind(ScriptManager_s script)
    {
        _scripts = script;
    }
    private void Start()
    {
        _playerImage = _playerObj.GetComponent<Image>();
    }
}
public partial class InGamePlayer_s//game system
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
        _curPlayerHP = _playerMaxHP;
        playerPos = Vector2Int.zero;
        _playerAttackLevel = EPlayerAttackLevel.ONE;
        _isGracePeriod = false;
        _playerHitBlock = 0;
    }
    public void GamePlay()
    {
        _playerUI_s.PlayerHPUpdate(_curPlayerHP);
        _playerObj.transform.localPosition = _scripts._inGameSideData_s.sideDatas[playerPos.x, playerPos.y].transform;
        _playerUI_s.AttackChange(_playerAttackLevel);
    }
    public void GameEnd()
    {
        StopAllCoroutines();
    }
    public void MoveNextBit(EInGameStatus curInGameStatus)
    {
        switch (curInGameStatus)
        {
            case EInGameStatus.SHOWPATH:
                break;
            case EInGameStatus.PLAYERMOVE:
                if (_isGracePeriod && _playerHitBlock == 0)
                {
                    _isGracePeriod = false;
                    ObjectAction.ImageAlphaChange(_playerImage, 1f);
                }
                else if (_playerHitBlock > 0)
                {
                    _playerHitBlock--;
                }
                break;
            case EInGameStatus.CUBEROTATE:
                break;
            case EInGameStatus.TIMEWAIT:
                break;
            default:
                break;
        }
    }
    public void ChangeInGameStatus(EInGameStatus changeTarget) //change to changeTarget
    {
        switch(changeTarget)
        {
            case EInGameStatus.SHOWPATH:
                AttackChange(_scripts._inGameManager_s.curStage);
                break;
            case EInGameStatus.CUBEROTATE:
                if (_isGracePeriod)
                {
                    _playerHitBlock = 0;
                    _isGracePeriod = false;
                    ObjectAction.ImageAlphaChange(_playerImage, 1f);
                }
                break;
            case EInGameStatus.TIMEWAIT:
                MovePlayer(new Vector2Int(playerPos.x * -1, playerPos.y * -1), _scripts._inGameSideData_s.divideSize);
                if (_scripts._keyInputManager_s.cubeRotateClear)
                {
                    UpdatePlayerHP(1);
                }
                _playerUI_s.PlayerAttack();
                break;
        }
    }
}
public partial class InGamePlayer_s  //move
{
    public void MovePlayer(Vector2Int movePos, Vector2Int divideSize)
    {
        if (PlayerMoveCheck(movePos, divideSize))
        {
            StartCoroutine(MoveTimeLock());
        }
    }
    private bool PlayerMoveCheck(Vector2Int movePos, Vector2Int divideSize)//�� ���������� �̵��� �������� �Ұ������� �Ǵ�
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
        playerPos = movedPosition;
        return true;
    }
    private IEnumerator MoveTimeLock()
    {
        StartCoroutine(ObjectAction.MovingObj(_playerObj, _scripts._inGameSideData_s.sideDatas[playerPos.x, playerPos.y].transform, _movingTime, _scripts._inGameMusicManager_s));
        yield return new WaitForSeconds(_movingTime);
        _moveEffect.Play();
        PlayerPositionCheck();
    }
    public void PlayerPositionCheck()
    {
        if (_scripts._inGameSideData_s.sideDatas[playerPos.x, playerPos.y].noise != null)
        {
            _scripts._inGameEnemy_s.RemoveTargetObj("noise", playerPos.x, playerPos.y, true);
            _doremiUI_s.DoremiAnimation("ready");
            _noiseDissolveEffect.Play();
        }
    }
}
public partial class InGamePlayer_s  //data change
{
    public void AttackChange(EStage stage)
    {
        switch(stage)
        {
            case EStage.STAGE_ONE:
                _playerAttackLevel = EPlayerAttackLevel.ONE;
                _playerUI_s.AttackChange(_playerAttackLevel);
                break;
            case EStage.STAGE_THREE:
                _playerAttackLevel = EPlayerAttackLevel.TWO;
                _playerUI_s.AttackChange(_playerAttackLevel);
                break;
            case EStage.STAGE_FIVE:
                _playerAttackLevel = EPlayerAttackLevel.THREE;
                _playerUI_s.AttackChange(_playerAttackLevel);
                break;
        }
    }
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
        _playerUI_s.PlayerHPUpdate(_curPlayerHP);
    }
    private void PlayerHPUp(int changeValue)
    {
        _curPlayerHP += changeValue;
        _curPlayerHP = _curPlayerHP > _playerMaxHP ? _playerMaxHP : _curPlayerHP;
        _playerUI_s.PlayerHPUp();
    }
    private void PlayerHPDown(int changeValue)
    {
        if (!_isGracePeriod)
        {
            StageDataController.Instance.loseValuel += 5;
            _curPlayerHP += changeValue;
            if (_curPlayerHP <= 0)
            {
                _scripts._inGameManager_s.GameOverByPlayer();
            }
            else
            {
                ObjectAction.ImageAlphaChange(_playerImage, 0.7f);
                _isGracePeriod = true;
                _playerHitBlock = 1;
                _playerUI_s.PlayerHPDown();
                _scripts._inGameManager_s.UpdateCombo(_scripts._inGameManager_s.combo * -1);
            }
        }
    } 
}
