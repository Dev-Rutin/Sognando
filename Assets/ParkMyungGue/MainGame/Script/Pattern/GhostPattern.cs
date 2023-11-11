using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GhostPattern : MonoBehaviour
{
    [Header("scripts")]
    [SerializeField] private InGameSideData_s _sideData_s;
    [SerializeField] private InGameMusicManager_s _musicManager_s;
    [Header("pattern data")]
    [SerializeField] private GameObject _ghostPrefab;
    [SerializeField] private Transform _ghostTsf;
    private GameObject _curGhostObj;
    private int _curGhostPos;
    private IEnumerator _curGhostI;
    public bool isGhostPlay {  get; private set; }
    public void SetGhost() //1,3
    {
        Debug.Log("aa");
        _curGhostObj = Instantiate(_ghostPrefab, _ghostTsf);
        _curGhostObj.transform.localPosition = _sideData_s.sideDatas[0, 0].transform;
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
                    _curGhostObj.transform.localPosition = _sideData_s.sideDatas[0, 0].transform;
                    moveTarget = _sideData_s.sideDatas[1, 0].transform;
                    break;
                case 1:
                    moveTarget = _sideData_s.sideDatas[2, 0].transform;
                    break;
                case 2:
                    moveTarget = _sideData_s.sideDatas[2, 1].transform;
                    break;
                case 3:
                    moveTarget = _sideData_s.sideDatas[2, 1].transform;
                    _curGhostPos = -1;
                    break;
                default:
                    break;
            }
            _curGhostPos++;
            _curGhostI = ObjectAction.MovingObj(_curGhostObj, moveTarget, 0.1f, _musicManager_s);
            StartCoroutine(_curGhostI);
        }
    }
}
