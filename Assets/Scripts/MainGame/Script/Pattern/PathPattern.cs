using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPattern : Singleton<PathPattern>
{
    [SerializeField] private int _pathCreatCount;
    private Queue<Vector2Int> _pathQueue;
    [SerializeField] private GameObject _pathPrefab;
    private List<GameObject> _pathObjList;
    [SerializeField] Transform _pathTsf;
    private List<Vector2Int> _canMovedList;
    private void Start()
    {
        _pathQueue = new Queue<Vector2Int>();
        _pathObjList = new List<GameObject>();
        for(int i=0;i<_pathCreatCount;i++)
        {
            _pathObjList.Add(Instantiate(_pathPrefab, _pathTsf));
            _pathObjList[i].transform.position = InGameManager_s.throwVector2;
        }
        _canMovedList = new List<Vector2Int>();
    }
    private void AddCanMovePostion(Vector2Int target,Vector2Int previousPos)
    {
        Vector2Int addPos = target + Vector2Int.left;
        if (InGameSideData_s.Instance.SizeCheck(addPos)&&addPos!=previousPos)
        {
            _canMovedList.Add(addPos);
        }
        addPos = target + Vector2Int.right;
        if (InGameSideData_s.Instance.SizeCheck(addPos) && addPos != previousPos)
        {
            _canMovedList.Add(addPos);
        }
        addPos = target + Vector2Int.up;
        if (InGameSideData_s.Instance.SizeCheck(addPos) && addPos != previousPos)
        {
            _canMovedList.Add(addPos);
        }
        addPos = target + Vector2Int.down;
        if (InGameSideData_s.Instance.SizeCheck(addPos) && addPos != previousPos)
        {
            _canMovedList.Add(addPos);
        }
    }
    private void GetRandomePath()
    {
        Vector2Int curPos = Vector2Int.zero;
        Vector2Int previousPos = curPos;
        int createCount =  Random.Range(8, 10);
        int blockCount= 0;
        while (_pathQueue.Count <= createCount)
        {
            _canMovedList.Clear();
            AddCanMovePostion(curPos, previousPos);
            if (_canMovedList.Count > 0)
            {
                Vector2Int moveTarget = _canMovedList[Random.Range(0, _canMovedList.Count)];               
                _pathQueue.Enqueue(moveTarget);
                previousPos = curPos;
                curPos = moveTarget;
            }
            else
            {
                blockCount++;
                if(blockCount>100)
                {
                    break;
                }
                else
                {
                    _pathQueue.Clear();
                    curPos = Vector2Int.zero;
                    previousPos  = curPos;
                }
            }
        }
    }
    private void EndRandomePath()
    {
    }
    public void Action(EInGameStatus inGameStatus)
    {
        switch(inGameStatus)
        {
            case EInGameStatus.SHOWPATH:
                GetRandomePath();
                break;
            default:
                break;
        }
    }
    public bool isPatternEnd()
    {
        if (_pathTsf.childCount == 0)
        {
            return true;
        }
        return false;
    }
}
