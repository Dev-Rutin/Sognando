using FMOD;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public partial class InGamePlayer_s : Singleton<InGamePlayer_s>, IInGame//data
{
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
    [SerializeField] private Image _flickerImage;
    private void Start()
    {
        _playerImage = _playerObj.GetComponent<Image>();
    }
}
public partial class InGamePlayer_s//game system
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
        _curPlayerHP = _playerMaxHP;
        playerPos = Vector2Int.zero;
        _playerAttackLevel = EPlayerAttackLevel.ONE;
        _isGracePeriod = false;
        _playerHitBlock = 0;
    }
    public void GamePlay()
    {
        ObjectAction.ImageAlphaChange(_flickerImage, 0);
        PlayerUI_s.Instance.PlayerHPUpdate(_curPlayerHP);
        _playerObj.transform.localPosition = InGameSideData_s.Instance.sideDatas[playerPos.x, playerPos.y].transform;
        PlayerUI_s.Instance.AttackChange(_playerAttackLevel);
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
                AttackChange(InGameManager_s.Instance.curStage);
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
                MovePlayer(new Vector2Int(playerPos.x * -1, playerPos.y * -1), InGameSideData_s.Instance.divideSize);
                if (KeyInputManager_s.Instance.cubeRotateClear)
                {
                    UpdatePlayerHP(1);
                }
                PlayerUI_s.Instance.PlayerAttack();
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
    private bool PlayerMoveCheck(Vector2Int movePos, Vector2Int divideSize)//그 포지션으로 이동이 가능한지 불가능한지 판단
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
        StartCoroutine(ObjectAction.MovingObj(_playerObj, InGameSideData_s.Instance.sideDatas[playerPos.x, playerPos.y].transform, _movingTime));
        yield return new WaitForSeconds(_movingTime);
        _moveEffect.Play();
        PlayerPositionCheck();
        StartCoroutine(InGameCube_s.Instance.QuakeCube(0.1f));
        StartCoroutine(ObjectAction.ImageFade(_flickerImage, 0.05f, true,0,1));
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(ObjectAction.ImageFade(_flickerImage, 0.05f, true,1,0));

    }
    public void PlayerPositionCheck()
    {
        if (InGameSideData_s.Instance.sideDatas[playerPos.x, playerPos.y].noise != null)
        {
            InGameEnemy_s.Instance.RemoveTargetObj("noise", playerPos.x, playerPos.y, true);
            DoremiUI_s.Instance.DoremiAnimation("ready");
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
                PlayerUI_s.Instance.AttackChange(_playerAttackLevel);
                break;
            case EStage.STAGE_THREE:
                _playerAttackLevel = EPlayerAttackLevel.TWO;
                PlayerUI_s.Instance.AttackChange(_playerAttackLevel);
                break;
            case EStage.STAGE_FIVE:
                _playerAttackLevel = EPlayerAttackLevel.THREE;
                PlayerUI_s.Instance.AttackChange(_playerAttackLevel);
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
        PlayerUI_s.Instance.PlayerHPUpdate(_curPlayerHP);
    }
    private void PlayerHPUp(int changeValue)
    {
        _curPlayerHP += changeValue;
        _curPlayerHP = _curPlayerHP > _playerMaxHP ? _playerMaxHP : _curPlayerHP;
        PlayerUI_s.Instance.PlayerHPUp();
    }
    private void PlayerHPDown(int changeValue)
    {
        if (!_isGracePeriod)
        {
            StageDataController.Instance.loseValuel += 5;
            _curPlayerHP += changeValue;
            if (_curPlayerHP <= 0)
            {
                InGameManager_s.Instance.GameOverByPlayer();
            }
            else
            {
                ObjectAction.ImageAlphaChange(_playerImage, 0.7f);
                _isGracePeriod = true;
                _playerHitBlock = 1;
                PlayerUI_s.Instance.PlayerHPDown();
                InGameManager_s.Instance.UpdateCombo(InGameManager_s.Instance.combo * -1);
            }
        }
    } 
}
