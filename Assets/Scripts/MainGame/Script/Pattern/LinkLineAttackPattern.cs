using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LinkLineAttackPattern : Singleton<LinkLineAttackPattern>
{
    [Header("data")]
    [SerializeField]private GameObject _linkLineAttackPrefab;
    [SerializeField]private Transform _linkLineAttackTsf;
    private GameObject _totalAttackObj;
    private GameObject _attackObj;

    private GameObject _warningTile;
    private CanvasGroup _warningCanvasGroup;
    
    private Vector2Int _curLinkLineAttackPos;
    private Vector2Int _curLinkLineAttackEndPos;
    private Vector2 _curLinkLineAttackDirection;
    public ELinkLineAttackMode curLinkLineAttackMode { get; private set; }
    private int _curLinkLineAttackCount;
    private void Start()
    {
        _totalAttackObj = Instantiate(_linkLineAttackPrefab, _linkLineAttackTsf);
        _totalAttackObj.transform.localPosition = InGameManager_s.throwVector2;
        _attackObj = _totalAttackObj.transform.GetChild(0).gameObject;
        _warningTile = _totalAttackObj.transform.GetChild(1).gameObject;
        _warningCanvasGroup = _totalAttackObj.transform.GetChild(1).GetComponent<CanvasGroup>();
        InGameFunBind_s.Instance.EgameStart += GameStart;
    }
    private void GameStart()
    {
        _totalAttackObj.SetActive(false);
        _totalAttackObj.transform.localPosition = Vector2.zero;
    }
    private void GetRandomeLinkLineAttack()
    {
        int count = Random.Range(0, 2);
        int secondcount = Random.Range(0, 2);
        if (count == 0) // row attack
        {
            int y = InGamePlayer_s.Instance.playerPos.y;
            if (secondcount == 0)// left
            {
                _curLinkLineAttackPos = new Vector2Int(0, y);
                _curLinkLineAttackEndPos = new Vector2Int(InGameSideData_s.Instance.divideSize.x - 1, y);

            }
            else
            {
                _curLinkLineAttackPos = new Vector2Int(InGameSideData_s.Instance.divideSize.x - 1, y);
                _curLinkLineAttackEndPos = new Vector2Int(0, y);

            }
        }
        else // columns attack
        {
            int x = InGamePlayer_s.Instance.playerPos.x;
            if (secondcount == 0)
            {
                _curLinkLineAttackPos = new Vector2Int(x, 0);
                _curLinkLineAttackEndPos = new Vector2Int(x, InGameSideData_s.Instance.divideSize.y - 1);
            }
            else
            {
                _curLinkLineAttackPos = new Vector2Int(x, InGameSideData_s.Instance.divideSize.y - 1);
                _curLinkLineAttackEndPos = new Vector2Int(x, 0);
            }
        }
        _curLinkLineAttackDirection = new Vector2(_curLinkLineAttackEndPos.x - _curLinkLineAttackPos.x, _curLinkLineAttackEndPos.y - _curLinkLineAttackPos.y).normalized;
        if (_curLinkLineAttackDirection == Vector2.down)
        {
            _attackObj.transform.eulerAngles = Vector4.zero;
        }
        else if (_curLinkLineAttackDirection == Vector2.up)
        {
            _attackObj.transform.eulerAngles = new Vector3(0, 0, -180);
 
        }
        else if (_curLinkLineAttackDirection == Vector2.left)
        {
            _attackObj.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        else if (_curLinkLineAttackDirection == Vector2.right)
        {
            _attackObj.transform.eulerAngles = new Vector3(0, 0, -90);
        }
        _attackObj.transform.localPosition = InGameSideData_s.Instance.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].transform;
        _attackObj.SetActive(false);
        InGameSideData_s.Instance.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].linkLineAttack = _totalAttackObj;
        _warningTile.transform.localPosition = InGameSideData_s.Instance.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].transform;
        _warningTile.SetActive(true);
        _totalAttackObj.SetActive(true);
        EnemyUI_s.Instance.MutipleEnemyAnimation(new List<string>() { "attack2", "idle" });
    }
    private void EndRandomeLinkLineAttack()
    {
        InGameEnemy_s.Instance.RemoveAllTargetObj("linkLineAttack",false);
        curLinkLineAttackMode = ELinkLineAttackMode.NONE;
    }
    public void Action(EInGameStatus inGameStatus, bool isEnemyPhaseEnd, bool isAttack)
    {
        if (inGameStatus==EInGameStatus.PLAYERMOVE)
        { 
            if (isEnemyPhaseEnd)
            {
                EndRandomeLinkLineAttack();
                curLinkLineAttackMode = ELinkLineAttackMode.NONE;
            }
            else if(isAttack)
            {
                switch(curLinkLineAttackMode)
                {
                    case ELinkLineAttackMode.NONE:
                        GetRandomeLinkLineAttack();
                        StartCoroutine(ObjectAction.ImageFade(_warningCanvasGroup, InGameMusicManager_s.Instance.secPerBeat, false, 1, 0, 1));
                        curLinkLineAttackMode = ELinkLineAttackMode.SHOW1;
                        break;
                    case ELinkLineAttackMode.SHOW1:
                        StartCoroutine(ObjectAction.ImageFade(_warningCanvasGroup, InGameMusicManager_s.Instance.secPerBeat, false, 1, 0, 1));
                        curLinkLineAttackMode = ELinkLineAttackMode.SHOW2;
                        break;
                    case ELinkLineAttackMode.SHOW2:
                        _attackObj.gameObject.SetActive(true);
                        _curLinkLineAttackCount = 1;
                        _warningTile.transform.localPosition =
                            InGameSideData_s.Instance.sideDatas[_curLinkLineAttackPos.x + (int)_curLinkLineAttackDirection.x, _curLinkLineAttackPos.y + (int)_curLinkLineAttackDirection.y].transform;
                        StartCoroutine(ObjectAction.ImageFade(_warningCanvasGroup, InGameMusicManager_s.Instance.secPerBeat, false, 1, 0, 1));
                        curLinkLineAttackMode = ELinkLineAttackMode.ATTACK;
                        break;
                    case ELinkLineAttackMode.ATTACK:
                        if (_curLinkLineAttackPos == _curLinkLineAttackEndPos)
                        {
                            EndRandomeLinkLineAttack();
                        }
                        else
                        {
                            GameObject temp = InGameSideData_s.Instance.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].linkLineAttack;
                            if (_curLinkLineAttackCount == 2)
                            {
                                InGameSideData_s.Instance.sideDatas[_curLinkLineAttackPos.x - (int)_curLinkLineAttackDirection.x, _curLinkLineAttackPos.y - (int)_curLinkLineAttackDirection.y].linkLineAttack = null;
                            }
                            else
                            {
                                _curLinkLineAttackCount++;
                            }
                            _curLinkLineAttackPos += new Vector2Int((int)_curLinkLineAttackDirection.x, (int)_curLinkLineAttackDirection.y);
                            InGameSideData_s.Instance.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].linkLineAttack = temp;
                            Vector2 moveTarget = InGameSideData_s.Instance.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].transform;
                            StartCoroutine(ObjectAction.MovingObj(_attackObj, moveTarget, InGameMusicManager_s.Instance.secPerBeat*0.5f));
                            //_attackObj.transform.localPosition = InGameSideData_s.Instance.sideDatas[_curLinkLineAttackPos.x, _curLinkLineAttackPos.y].transform;
                            if (_curLinkLineAttackPos != _curLinkLineAttackEndPos)
                            {
                                _warningTile.transform.localPosition =
                                    InGameSideData_s.Instance.sideDatas[_curLinkLineAttackPos.x + (int)_curLinkLineAttackDirection.x, _curLinkLineAttackPos.y + (int)_curLinkLineAttackDirection.y].transform;
                                StartCoroutine(ObjectAction.ImageFade(_warningCanvasGroup, InGameMusicManager_s.Instance.secPerBeat, false, 1, 0, 1));
                            }
                            else
                            {
                                _warningTile.SetActive(false);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        if(inGameStatus ==EInGameStatus.CUBEROTATE)
        {
            if (isEnemyPhaseEnd)
            {
                EndRandomeLinkLineAttack();
            }
        }
    }
}
