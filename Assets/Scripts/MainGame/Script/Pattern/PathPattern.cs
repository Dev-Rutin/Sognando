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

    private void Start()
    {
        _pathQueue = new Queue<Vector2Int>();
        _pathObjList = new List<GameObject>();
        for(int i=0;i<_pathCreatCount;i++)
        {
            _pathObjList.Add(Instantiate(_pathPrefab, _pathTsf));
            _pathObjList[i].transform.position = InGameManager_s.throwVector2;
        }
    }

    private void GetRandomePath()
    {
        Vector2Int curPos = Vector2Int.zero;
        Vector2Int movedPos = curPos;
      for(int i=0;i<_pathCreatCount;i++)
        {
           for(int j=0;j<100;j++)
            {
                
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
