using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class LineAttackPattern : Singleton<LineAttackPattern>
{
    [Header("data")]
    [SerializeField] private GameObject _lineAttackPrefab;
    [SerializeField] private Transform _lineAttackTsf;
    private List<GameObject> _lineAttackObjList;
    private List<CanvasGroup> _lineAttackCanvasList;
    public ELineAttackMode curLineAttackMod { get; private set; }
    [SerializeField] private Vector2 _attackStartPos; // 0 35
    [SerializeField] private Vector2 _attackEndPos; // 0 -10
    [SerializeField] private float _attackTime; // 0.1f

    [SerializeField] private Sprite _attackStartImage;
    [SerializeField] private Sprite _attackEndImage;
    private void Start()
    {
        _lineAttackObjList = new List<GameObject>();
        _lineAttackCanvasList = new List<CanvasGroup>();
        for(int i=0;i<Mathf.Max(InGameSideData_s.Instance.divideSize.x, InGameSideData_s.Instance.divideSize.y);i++)
        {
            _lineAttackObjList.Add(Instantiate(_lineAttackPrefab, _lineAttackTsf));
            _lineAttackCanvasList.Add(_lineAttackObjList[i].transform.GetChild(1).GetComponent<CanvasGroup>());
            _lineAttackObjList[i].transform.position= InGameManager_s.throwVector2;
        }
        InGameFunBind_s.Instance.EgameStart+= GameStart;
       InGameFunBind_s.Instance.EgameEnd+= GameEnd;
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
                int y = InGamePlayer_s.Instance.playerPos.y;
                for (int i = 0; i < InGameSideData_s.Instance.divideSize.x; i++)
                {
                    if (InGameSideData_s.Instance.sideDatas[i, y].isCanMakeCheck(true, "lineAttack"))
                    {
                        _lineAttackObjList[i].transform.GetChild(0).gameObject.SetActive(false);
                        _lineAttackObjList[i].transform.localPosition = InGameSideData_s.Instance.sideDatas[i, y].transform;
                        InGameSideData_s.Instance.sideDatas[i, y].lineAttack = _lineAttackObjList[i];
                    }
                }
            }
            else // columns attack
            {
                int x = InGamePlayer_s.Instance.playerPos.x;
                for (int i = 0; i < InGameSideData_s.Instance.divideSize.y; i++)
                {
                    if (InGameSideData_s.Instance.sideDatas[x, i].isCanMakeCheck(true, "lineAttack"))
                    {
                        _lineAttackObjList[i].transform.GetChild(0).gameObject.SetActive(false);
                        _lineAttackObjList[i].transform.localPosition = InGameSideData_s.Instance.sideDatas[x, i].transform;
                        InGameSideData_s.Instance.sideDatas[x, i].lineAttack = _lineAttackObjList[i];
                    }
                }
            }
        for (int i = 0; i < _lineAttackObjList.Count; i++)
        {
            _lineAttackObjList[i].SetActive(true);
            _lineAttackObjList[i].transform.GetChild(0).GetComponent<Image>().sprite = _attackStartImage;
        }
        EnemyUI_s.Instance.MutipleEnemyAnimation(new List<string>() { "attack1", "idle" });
    }
    private void EndRandomeLineAttack()
    {
      InGameEnemy_s.Instance.RemoveAllTargetObj("lineAttack",false);
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
                            StartCoroutine(ObjectAction.ImageFade(_lineAttackCanvasList[i], InGameMusicManager_s.Instance.secPerBeat , false, 1, 0, 1));
                        }
                        curLineAttackMod = ELineAttackMode.SHOW1;
                        break;
                    case ELineAttackMode.SHOW1:
                        for (int i = 0; i < _lineAttackObjList.Count; i++)
                        {
                            StartCoroutine(ObjectAction.ImageFade(_lineAttackCanvasList[i], InGameMusicManager_s.Instance.secPerBeat , false, 1, 0, 1));
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
        StartCoroutine(ObjectAction.MovingObj(target, _attackEndPos, _attackTime));
        yield return new WaitUntil(() => target.transform.localPosition.y == _attackEndPos.y);
        for (int i = 0; i < _lineAttackObjList.Count; i++)
        {
            _lineAttackObjList[i].transform.GetChild(0).GetComponent<Image>().sprite = _attackEndImage;
        }
        StartCoroutine(ObjectAction.ImageFade(target.GetComponent<Image>(), InGameMusicManager_s.Instance.secPerBeat-_attackTime,true,1,0,1 ));
    }
}
