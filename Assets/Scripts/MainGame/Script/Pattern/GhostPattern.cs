using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GhostPattern : Singleton<GhostPattern>
{
    [Header("pattern data")]
    [SerializeField] private GameObject _ghostPrefab;
    [SerializeField] private Transform _ghostTsf;
    private GameObject _curGhostObj;
    private int _curGhostPos;
    private IEnumerator _curGhostI;
    public bool isGhostPlay {  get; private set; }
    public void SetGhost() //1,3
    {
        _curGhostObj = Instantiate(_ghostPrefab, _ghostTsf);
        _curGhostObj.transform.localPosition = InGameSideData_s.Instance.sideDatas[0, 0].transform;
        _curGhostPos = 0;
        isGhostPlay = true;
    }
    public void EndGhost()
    {
        if (_curGhostI != null)
        {
            StopCoroutine(_curGhostI);
        }
        Destroy(_curGhostObj);
        isGhostPlay = false;
    }
    public void Action(EInGameStatus inGameStatus)
    {
        if (inGameStatus == EInGameStatus.PLAYERMOVE||inGameStatus==EInGameStatus.SHOWPATH)
        {
            Vector2 moveTarget = Vector2.zero;
            switch (_curGhostPos)
            {
                case 0:
                    _curGhostObj.transform.localPosition = InGameSideData_s.Instance.sideDatas[0, 0].transform;
                    moveTarget = InGameSideData_s.Instance.sideDatas[1, 0].transform;
                    break;
                case 1:
                    moveTarget = InGameSideData_s.Instance.sideDatas[2, 0].transform;
                    break;
                case 2:
                    moveTarget = InGameSideData_s.Instance.sideDatas[2, 1].transform;
                    break;
                case 3:
                    moveTarget = InGameSideData_s.Instance.sideDatas[2, 1].transform;
                    _curGhostPos = -1;
                    break;
                default:
                    break;
            }
            _curGhostPos++;
            _curGhostI = ObjectAction.MovingObj(_curGhostObj, moveTarget, 0.1f);
            StartCoroutine(_curGhostI);
        }
    }
}
