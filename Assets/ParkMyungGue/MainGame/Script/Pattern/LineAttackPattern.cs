using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineAttackPattern : MonoBehaviour
{
    [Header("scripts")]
    [SerializeField] private ScriptManager_s _script;

    [Header("data")]
    [SerializeField] private GameObject _lineAttackPrefab;
    [SerializeField] private Transform _lineAttackTsf;
    private List<GameObject> _lineAttackObjList;
    private List<ParticleSystem> _lineAttackParticleList;
    public ELineAttackMode curLineAttackMod { get; private set; }
    [SerializeField] private Vector2 _attackStartPos; // 0 35
    [SerializeField] private Vector2 _attackEndPos; // 0 -10
    [SerializeField] private float _attackTime; // 0.1f
    private void Start()
    {
        _lineAttackObjList = new List<GameObject>();
        _lineAttackParticleList = new List<ParticleSystem>();
        for(int i=0;i<Mathf.Max(_script._inGameSideData_s.divideSize.x, _script._inGameSideData_s.divideSize.y);i++)
        {
            _lineAttackObjList.Add(Instantiate(_lineAttackPrefab, _lineAttackTsf));
            _lineAttackParticleList.Add(_lineAttackObjList[i].transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>());
        }
        _script._inGamefunBind_s.EgameStart+= GameStart;
        _script._inGamefunBind_s.EgameEnd+= GameEnd;
    }
    private void GameStart()
    {
        for(int i=0;i<_lineAttackObjList.Count;i++)
        {
            _lineAttackObjList[i].SetActive(false);
        }
    }
    private void GameEnd()
    {
        StopAllCoroutines();
    }
    private void GetRandomeLineAttack()
    {
        int count = UnityEngine.Random.Range(0, 2);
        if (count == 0) // row attack
        {
            int y =  _script._inGamePlayer_s.playerPos.y;
            for (int i = 0; i < _script._inGameSideData_s.divideSize.x; i++)
            {
                if (_script._inGameSideData_s.sideDatas[i, y].isCanMakeCheck(true, "lineAttack"))
                {
                    _lineAttackObjList[i].transform.GetChild(0).gameObject.SetActive(false);
                    _lineAttackObjList[i].transform.localPosition = _script._inGameSideData_s.sideDatas[i, y].transform;
                    _script._inGameSideData_s.sideDatas[i, y].lineAttack = _lineAttackObjList[i];
                }
            }
        }
        else // columns attack
        {
            int x = _script._inGamePlayer_s.playerPos.x;
            for (int i = 0; i < _script._inGameSideData_s.divideSize.y; i++)
            {
                if (_script._inGameSideData_s.sideDatas[x, i].isCanMakeCheck(true, "lineAttack"))
                {
                    _lineAttackObjList[i].transform.GetChild(0).gameObject.SetActive(false);
                    _lineAttackObjList[i].transform.localPosition = _script._inGameSideData_s.sideDatas[x, i].transform;
                    _script._inGameSideData_s.sideDatas[x,i].lineAttack = _lineAttackObjList[i];
                }
            }
        }
        for (int i = 0; i < _lineAttackObjList.Count; i++)
        {
            _lineAttackObjList[i].SetActive(true);
        }
    }
    private void EndRandomeLineAttack()
    {
        _script._inGameEnemy_s.RemoveAllTargetObj("lineAttack",false);
        curLineAttackMod = ELineAttackMode.NONE;
    }
    public void Action(EInGameStatus inGameStatus,bool isEnemyPhaseEnd, bool isAttack)
    {
        if(inGameStatus==EInGameStatus.PLAYERMOVE)
        {
            if (isEnemyPhaseEnd)
            {
                EndRandomeLineAttack();
            }
            else if(isAttack)
            {
                switch (curLineAttackMod)
                {
                    case ELineAttackMode.NONE:
                        GetRandomeLineAttack();
                        for (int i = 0; i < _lineAttackObjList.Count; i++)
                        {
                            _lineAttackObjList[i].transform.GetChild(1).gameObject.SetActive(true);
                            _lineAttackParticleList[i].Play();
                        }
                        curLineAttackMod = ELineAttackMode.SHOW1;
                        break;
                    case ELineAttackMode.SHOW1:
                        for (int i = 0; i < _lineAttackObjList.Count; i++)
                        {
                            _lineAttackParticleList[i].Play();
                        }
                        curLineAttackMod = ELineAttackMode.SHOW2;
                        break;
                    case ELineAttackMode.SHOW2:
                        for (int i = 0; i < _lineAttackObjList.Count; i++)
                        {
                            GameObject AttackObj = _lineAttackObjList[i].transform.GetChild(0).gameObject;
                            StartCoroutine(LineAttackMove(AttackObj));
                            _lineAttackObjList[i].transform.GetChild(1).gameObject.SetActive(false);
                        }
                        curLineAttackMod = ELineAttackMode.ATTACK;
                        break;
                    case ELineAttackMode.ATTACK:
                        EndRandomeLineAttack();
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
                EndRandomeLineAttack();
            }
        }
    }
    IEnumerator LineAttackMove(GameObject target)
    {
        ObjectAction.ImageAlphaChange(target.GetComponent<Image>(), 1f);
        target.transform.localPosition = _attackStartPos;
        target.SetActive(true);
        StartCoroutine(ObjectAction.MovingObj(target, _attackEndPos, _attackTime, _script._inGameMusicManager_s));
        yield return new WaitUntil(() => target.transform.localPosition.y == _attackEndPos.y);
        StartCoroutine(ObjectAction.ImageFade(target.GetComponent<Image>(), _script._inGameMusicManager_s.secPerBeat-_attackTime,true , _script._inGameMusicManager_s));
    }
}
