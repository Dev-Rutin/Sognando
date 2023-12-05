using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class PathPattern : Singleton<PathPattern>
{
    [SerializeField] private int _pathCreatCount;
    private Queue<Vector2Int> _pathQueue;
    [SerializeField] private GameObject _pathPrefab;
    [SerializeField] Transform _pathTsf;
    private List<Vector2Int> _canMovedList;
    private LineRenderer _line;
    private void Start()
    {
        _pathQueue = new Queue<Vector2Int>();
        _canMovedList = new List<Vector2Int>();
        _line = GetComponent<LineRenderer>();
    }
    private void Update()
    {
        if(_pathQueue.Count!=0)
        {
            _line.SetPosition(0, InGamePlayer_s.Instance.GetPlayerObj().transform.position);
            _line.SetPosition(1, InGameSideData_s.Instance.sideDatas[_pathQueue.Peek().x, _pathQueue.Peek().y].pathNoise.transform.position);
        }
    }
    private void AddCanMovePostion(Vector2Int target,Vector2Int previousPos)
    {
        Vector2Int addPos = target + Vector2Int.left;
        if (InGameSideData_s.Instance.SizeCheck(addPos)&&addPos!=previousPos&&!_pathQueue.Contains(addPos))
        {
            _canMovedList.Add(addPos);
        }
        addPos = target + Vector2Int.right;
        if (InGameSideData_s.Instance.SizeCheck(addPos) && addPos != previousPos && !_pathQueue.Contains(addPos))
        {
            _canMovedList.Add(addPos);
        }
        addPos = target + Vector2Int.up;
        if (InGameSideData_s.Instance.SizeCheck(addPos) && addPos != previousPos && !_pathQueue.Contains(addPos))
        {
            _canMovedList.Add(addPos);
        }
        addPos = target + Vector2Int.down;
        if (InGameSideData_s.Instance.SizeCheck(addPos) && addPos != previousPos && !_pathQueue.Contains(addPos))
        {
            _canMovedList.Add(addPos);
        }
    }
    private void GetRandomePath()
    {
        _pathQueue = new Queue<Vector2Int>();
        Debug.Log("pathQueueCount" + _pathQueue.Count);
        Vector2Int curPos = new Vector2Int();
        curPos.x = 0;
        curPos.y = 0;
        Vector2Int previousPos = new Vector2Int();
        previousPos.x = curPos.x;
        previousPos.y = curPos.y;
        int createCount =  Random.Range(8, 10);
        int blockCount= 0;
        while (_pathQueue.Count <= createCount)
        {
            _canMovedList.Clear();
            AddCanMovePostion(curPos, previousPos);
            if (_canMovedList.Count > 0)
            {
                Vector2Int moveTarget = _canMovedList[Random.Range(0, _canMovedList.Count)];
                Debug.Log(moveTarget);
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
                }
            }
        }
        int count = 0;
        foreach (var data in _pathQueue)
        {
            Debug.Log(data + " and " + count);
            InGameEnemy_s.Instance.SetObj("pathNoise", _pathPrefab, false, _pathTsf, data);
            count++;
        }
        _line.positionCount = 2;
    }
    public Vector2Int GetMoveTarget()
    {
        return _pathQueue.Peek();
    }
    public void MovePath()
    {
        InGameEnemy_s.Instance.RemoveTargetObj("pathNoise", _pathQueue.Peek().x, _pathQueue.Peek().y, true);
        _pathQueue.Dequeue();
        if (InGameEnemy_s.Instance.EnemyPhaseEndCheck())
        {
            UnityEngine.Debug.Log("end");
            InGameManager_s.Instance.ChangeInGameState(EInGameStatus.CUBEROTATE);
        }
    }
    private void EndRandomePath()
    {
        _pathQueue.Clear();
    }
    public void Action(EInGameStatus inGameStatus)
    {
        switch(inGameStatus)
        {
            case EInGameStatus.SHOWPATH:
                Debug.Log("get randome path");
                _pathQueue.Clear();
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
