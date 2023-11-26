using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossAttackPattern : Singleton<CrossAttackPattern>
{
    [Header("data")]
    [SerializeField] private GameObject _crossAttackPrefab;
    [SerializeField] private Transform _crossAttackTsf;
    private List<GameObject> _crossAttackObjList;
    private List<ParticleSystem> _crossAttackParticleList;
    public ELineAttackMode curCrossAttackMod { get; private set; }
    [SerializeField] private Vector2 _attackStartPos; // 0 35
    [SerializeField] private Vector2 _attackEndPos; // 0 -10
    [SerializeField] private float _attackTime; // 0.1f
    private void Start()
    {
        _crossAttackObjList = new List<GameObject>();
        _crossAttackParticleList = new List<ParticleSystem>();
        for (int i = 0; i < 5; i++)
        {
            _crossAttackObjList.Add(Instantiate(_crossAttackPrefab, _crossAttackTsf));
            _crossAttackParticleList.Add(_crossAttackObjList[i].transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>());
            _crossAttackObjList[i].transform.position = InGameManager_s.throwVector2;
        }
        InGameFunBind_s.Instance.EgameStart += GameStart;
        InGameFunBind_s.Instance.EgameEnd += GameEnd;
    }
    private void GameStart()
    {
        for (int i = 0; i < _crossAttackObjList.Count; i++)
        {
            _crossAttackObjList[i].SetActive(false);
        }
    }
    private void GameEnd()
    {
        StopAllCoroutines();
    }
    private void GetRandomeCrossAttack()
    {
        Vector2Int playerPos = InGamePlayer_s.Instance.playerPos;
        Vector2Int targetPos = playerPos;
        int count = 0;
        if (InGameSideData_s.Instance.sideDatas[targetPos.x, targetPos.y].isCanMakeCheck(true, "crossAttack"))
        {
            _crossAttackObjList[count].transform.GetChild(0).gameObject.SetActive(false);
            _crossAttackObjList[count].transform.localPosition = InGameSideData_s.Instance.sideDatas[targetPos.x, targetPos.y].transform;
            InGameSideData_s.Instance.sideDatas[targetPos.x, targetPos.y].crossAttack = _crossAttackObjList[count];
            count++;
        }
        targetPos = playerPos + new Vector2Int(1, 0);
        if (InGameSideData_s.Instance.SizeCheck(targetPos))
        {
            if (InGameSideData_s.Instance.sideDatas[targetPos.x, targetPos.y].isCanMakeCheck(true, "crossAttack"))
            {
                _crossAttackObjList[count].transform.GetChild(0).gameObject.SetActive(false);
                _crossAttackObjList[count].transform.localPosition = InGameSideData_s.Instance.sideDatas[targetPos.x, targetPos.y].transform;
                InGameSideData_s.Instance.sideDatas[targetPos.x, targetPos.y].crossAttack = _crossAttackObjList[count];
                count++;
            }
        }
        targetPos = playerPos + new Vector2Int(-1, 0);
        if (InGameSideData_s.Instance.SizeCheck(targetPos))
        {
            if (InGameSideData_s.Instance.sideDatas[targetPos.x, targetPos.y].isCanMakeCheck(true, "crossAttack"))
            {
                _crossAttackObjList[count].transform.GetChild(0).gameObject.SetActive(false);
                _crossAttackObjList[count].transform.localPosition = InGameSideData_s.Instance.sideDatas[targetPos.x, targetPos.y].transform;
                InGameSideData_s.Instance.sideDatas[targetPos.x, targetPos.y].crossAttack = _crossAttackObjList[count];
                count++;
            }
        }
        targetPos = playerPos + new Vector2Int(0, 1);
        if (InGameSideData_s.Instance.SizeCheck(targetPos))
        {
            if (InGameSideData_s.Instance.sideDatas[targetPos.x, targetPos.y].isCanMakeCheck(true, "crossAttack"))
            {
                _crossAttackObjList[count].transform.GetChild(0).gameObject.SetActive(false);
                _crossAttackObjList[count].transform.localPosition = InGameSideData_s.Instance.sideDatas[targetPos.x, targetPos.y].transform;
                InGameSideData_s.Instance.sideDatas[targetPos.x, targetPos.y].crossAttack = _crossAttackObjList[count];
                count++;
            }
        }
        targetPos = playerPos + new Vector2Int(0, -1);
        if (InGameSideData_s.Instance.SizeCheck(targetPos))
        {
            if (InGameSideData_s.Instance.sideDatas[targetPos.x, targetPos.y].isCanMakeCheck(true, "crossAttack"))
            {
                _crossAttackObjList[count].transform.GetChild(0).gameObject.SetActive(false);
                _crossAttackObjList[count].transform.localPosition = InGameSideData_s.Instance.sideDatas[targetPos.x, targetPos.y].transform;
                InGameSideData_s.Instance.sideDatas[targetPos.x, targetPos.y].crossAttack = _crossAttackObjList[count];
                count++;
            }
        }
        for (int i = 0; i < _crossAttackObjList.Count; i++)
        {
            _crossAttackObjList[i].SetActive(true);
        }
    }
    private void EndRandomeCrossAttack()
    {
        InGameEnemy_s.Instance.RemoveAllTargetObj("crossAttack", false);
        curCrossAttackMod = ELineAttackMode.NONE;
    }
    public void Action(EInGameStatus inGameStatus, bool isEnemyPhaseEnd, bool isAttack)
    {
        if (inGameStatus == EInGameStatus.PLAYERMOVE)
        {
            if (isEnemyPhaseEnd)
            {
                EndRandomeCrossAttack();
            }
            else if (isAttack)
            {
                switch (curCrossAttackMod)
                {
                    case ELineAttackMode.NONE:
                        GetRandomeCrossAttack();
                        for (int i = 0; i < _crossAttackObjList.Count; i++)
                        {
                            _crossAttackObjList[i].transform.GetChild(1).gameObject.SetActive(true);
                            _crossAttackParticleList[i].Play();
                        }
                        curCrossAttackMod = ELineAttackMode.SHOW1;
                        break;
                    case ELineAttackMode.SHOW1:
                        for (int i = 0; i < _crossAttackObjList.Count; i++)
                        {
                            _crossAttackParticleList[i].Play();
                        }
                        curCrossAttackMod = ELineAttackMode.SHOW2;
                        break;
                    case ELineAttackMode.SHOW2:
                        for (int i = 0; i < _crossAttackObjList.Count; i++)
                        {
                            GameObject AttackObj = _crossAttackObjList[i].transform.GetChild(0).gameObject;
                            StartCoroutine(LineAttackMove(AttackObj));
                            _crossAttackObjList[i].transform.GetChild(1).gameObject.SetActive(false);
                        }
                        curCrossAttackMod = ELineAttackMode.ATTACK;
                        break;
                    case ELineAttackMode.ATTACK:
                        EndRandomeCrossAttack();
                        break;
                    default:
                        break;
                }
            }
        }
        if (inGameStatus == EInGameStatus.CUBEROTATE)
        {
            if (isEnemyPhaseEnd)
            {
                EndRandomeCrossAttack();
            }
        }
    }
    IEnumerator LineAttackMove(GameObject target)
    {
        ObjectAction.ImageAlphaChange(target.GetComponent<Image>(), 1f);
        target.transform.localPosition = _attackStartPos;
        target.SetActive(true);
        StartCoroutine(ObjectAction.MovingObj(target, _attackEndPos, _attackTime));
        yield return new WaitUntil(() => target.transform.localPosition.y == _attackEndPos.y);
        StartCoroutine(ObjectAction.ImageFade(target.GetComponent<Image>(), InGameMusicManager_s.Instance.secPerBeat - _attackTime, true, 1, 0));
    }
}
