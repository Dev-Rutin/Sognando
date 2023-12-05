using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaAttackPattern : Singleton<AreaAttackPattern>
{
    [Header("data")]
    [SerializeField] private GameObject _areaAttackPrefab;
    [SerializeField] private Transform _areaAttackTsf;
    private List<GameObject> _areaAttackObjList;
    private List<CanvasGroup> _areaAttackCanvasList;
    public ELineAttackMode curAreaAttackMod { get; private set; }
    [SerializeField] private Vector2 _attackStartPos; // 0 35
    [SerializeField] private Vector2 _attackEndPos; // 0 -10
    [SerializeField] private float _attackTime; // 0.1f

    [SerializeField] private Sprite _attackStartImage;
    [SerializeField] private Sprite _attackEndImage;
    private void Start()
    {
        _areaAttackObjList = new List<GameObject>();
        _areaAttackCanvasList = new List<CanvasGroup>();
        for (int i = 0; i < (InGameSideData_s.Instance.divideSize.x * InGameSideData_s.Instance.divideSize.y); i++)
        {
            _areaAttackObjList.Add(Instantiate(_areaAttackPrefab, _areaAttackTsf));
            _areaAttackCanvasList.Add(_areaAttackObjList[i].transform.GetChild(1).GetComponent<CanvasGroup>());
            _areaAttackObjList[i].transform.position = InGameManager_s.throwVector2;
        }
        InGameFunBind_s.Instance.EgameStart += GameStart;
        InGameFunBind_s.Instance.EgameEnd += GameEnd;
    }
    private void GameStart()
    {
        for (int i = 0; i < _areaAttackObjList.Count; i++)
        {
            _areaAttackObjList[i].SetActive(false);
        }
    }
    private void GameEnd()
    {
        StopAllCoroutines();
    }
    private void GetRandomeAreaAttack()
    {
        for (int i = 0; i < _areaAttackObjList.Count; i++)
        {
            _areaAttackObjList[i].transform.position = InGameManager_s.throwVector2;
            _areaAttackObjList[i].transform.GetChild(0).GetComponent<Image>().sprite = _attackStartImage;
        }

        int count = 0;
        for(int i=0;i<4;i++)
        {
            for (int j = 0; j < 50; j++)
            {
                Vector2Int target = Vector2Int.zero;
                target.x = Random.Range(0, 4);
                target.y = Random.Range(0, 4);
                if (InGameSideData_s.Instance.sideDatas[target.x, target.y].areaAttackSafe ==false)
                {
                    count++;
                    InGameSideData_s.Instance.sideDatas[target.x, target.y].areaAttackSafe = true;
                    break;
                }
            }
        }
        count = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (InGameSideData_s.Instance.sideDatas[i, j].areaAttackSafe == false)
                {
                    _areaAttackObjList[count].transform.GetChild(0).gameObject.SetActive(false);
                    _areaAttackObjList[count].transform.localPosition = InGameSideData_s.Instance.sideDatas[i, j].transform;
                    count++;
                    Debug.Log(count);
                }
            }
        }
        for (int i = 0; i < _areaAttackObjList.Count; i++)
        {
            _areaAttackObjList[i].SetActive(true);
        }
        if (InGameEnemy_s.Instance.isStage2)
        {
            EnemyUI_s.Instance.MutipleEnemyAnimation(new List<string>() { "attack", "idle" });
        }
        else
        {
            EnemyUI_s.Instance.MutipleEnemyAnimation(new List<string>() { "attack1", "idle" });
        }
    }
    private void EndRandomeAreaAttack()
    {
        InGameEnemy_s.Instance.RemoveAllTargetObj("areaAttackSafe", false);
        for (int i = 0; i < _areaAttackObjList.Count; i++)
        {
            _areaAttackObjList[i].SetActive(false);
        }
        curAreaAttackMod = ELineAttackMode.NONE;
    }
    public void Action(EInGameStatus inGameStatus, bool isEnemyPhaseEnd, bool isAttack)
    {
        if (inGameStatus == EInGameStatus.PLAYERMOVE)
        {
            if (isEnemyPhaseEnd)
            {
                EndRandomeAreaAttack();
            }
            else if (isAttack)
            {
                switch (curAreaAttackMod)
                {
                    case ELineAttackMode.NONE:
                        GetRandomeAreaAttack();
                        for (int i = 0; i < _areaAttackObjList.Count; i++)
                        {
                            _areaAttackObjList[i].transform.GetChild(1).gameObject.SetActive(true);
                            StartCoroutine(ObjectAction.ImageFade(_areaAttackCanvasList[i], InGameMusicManager_s.Instance.secPerBeat, false, 1, 0, 1));
                        }
                        curAreaAttackMod = ELineAttackMode.SHOW1;
                        break;
                    case ELineAttackMode.SHOW1:
                        for (int i = 0; i < _areaAttackObjList.Count; i++)
                        {
                            StartCoroutine(ObjectAction.ImageFade(_areaAttackCanvasList[i], InGameMusicManager_s.Instance.secPerBeat, false, 1, 0, 1));
                        }
                        curAreaAttackMod = ELineAttackMode.SHOW2;
                        break;
                    case ELineAttackMode.SHOW2:
                        for (int i = 0; i < _areaAttackObjList.Count; i++)
                        {
                            GameObject AttackObj = _areaAttackObjList[i].transform.GetChild(0).gameObject;
                            StartCoroutine(AreaAttackMove(AttackObj));
                            _areaAttackObjList[i].transform.GetChild(1).gameObject.SetActive(false);
                        }
                        curAreaAttackMod = ELineAttackMode.ATTACK;
                        break;
                    case ELineAttackMode.ATTACK:
                        EndRandomeAreaAttack();
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
                EndRandomeAreaAttack();
            }
        }
    }
    IEnumerator AreaAttackMove(GameObject target)
    {
        ObjectAction.ImageAlphaChange(target.GetComponent<Image>(), 1f);
        target.transform.localPosition = _attackStartPos;
        target.SetActive(true);
        StartCoroutine(ObjectAction.MovingObj(target, _attackEndPos, _attackTime));
        yield return new WaitUntil(() => target.transform.localPosition.y == _attackEndPos.y);
        for (int i = 0; i < _areaAttackObjList.Count; i++)
        {
            _areaAttackObjList[i].transform.GetChild(0).GetComponent<Image>().sprite = _attackEndImage;
        }
        StartCoroutine(ObjectAction.ImageFade(target.GetComponent<Image>(), InGameMusicManager_s.Instance.secPerBeat - _attackTime, true, 1, 0, 1));
    }
}
