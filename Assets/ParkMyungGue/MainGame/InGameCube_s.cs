using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class InGameCube_s : MonoBehaviour,IInGame,IScript //data
{
    private InGameManager_s _inGameManager_s;
    private InGameData_s _inGameData_s;
    [Header("cube data")]
    public GameObject gameCubeObj;
    [Header("cube rotate")]
    private bool IsCubeRotate;
    private Vector3 rotatedivide;
    private Vector3 endRotatePos;
    private float rotateIncrease;
    public void ScriptBind(InGameManager_s gameManager)
    {
        _inGameManager_s = gameManager;
        _inGameData_s = gameManager.GetInGameDataScript();
    } 
    public void DefaultDataSetting()
    {
        gameCubeObj = _inGameData_s.gameCubeObj;
        IsCubeRotate = false;
        rotatedivide = Vector3.zero;
        endRotatePos = Vector3.zero;
        rotateIncrease = 0;
    }
}
public partial class InGameCube_s : MonoBehaviour, IInGame //game system
{
    public void GameStart()
    {
        gameCubeObj.transform.localEulerAngles = Vector3.zero;
        _inGameData_s.cubeUI.gameObject.SetActive(true);
        IsCubeRotate = false;
        rotatedivide = Vector3.zero;
        endRotatePos = Vector3.zero;
        rotateIncrease = 0;
    }
    public void GameEnd()
    {

    }
    private void Update()
    {
        if (_inGameManager_s.curGameStatus == EGameStatus.PLAYING)
        {
            if (IsCubeRotate)
            {
                float curRotateValue = Mathf.Abs(rotatedivide.x + rotatedivide.y + rotatedivide.z) / (1 / Time.deltaTime * _inGameData_s.movingTime);
                gameCubeObj.transform.RotateAround(gameCubeObj.transform.position, rotatedivide, curRotateValue);
                rotateIncrease += curRotateValue;
            }
        }
    }

    public void MoveNextBit(EInGameStatus curInGameStatus)
    {

    }
    public void ChangeInGameStatus(EInGameStatus changeTarget) //change to changeTarget
    {
        switch (changeTarget)
        {
            case EInGameStatus.SHOWPATH:
                _inGameData_s.cubeUI.gameObject.SetActive(true);
                break;
            case EInGameStatus.PLAYERMOVE:
                break;
            case EInGameStatus.TIMEWAIT:
                _inGameData_s.cubeUI.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }
}
public partial class InGameCube_s : MonoBehaviour, IInGame //rotate
{
    public void RotateCube(Vector3 rotateposition)
    {
        StartCoroutine(RotateTimeLock(rotateposition, gameCubeObj));
    }
    public IEnumerator RotateTimeLock(Vector3 rotateposition, GameObject targetobj)
    {
            rotatedivide = rotateposition;
            rotateIncrease = 0;
            IsCubeRotate = true;
            yield return new WaitUntil(() => rotateIncrease >= 90);
            IsCubeRotate = false;
            targetobj.transform.RotateAround(gameCubeObj.transform.position, rotatedivide, Mathf.Abs(rotatedivide.x + rotatedivide.y + rotatedivide.z) - rotateIncrease);
            targetobj.transform.localEulerAngles = new Vector3(MathF.Round(targetobj.transform.localEulerAngles.x), Mathf.Round(targetobj.transform.localEulerAngles.y), Mathf.Round(targetobj.transform.localEulerAngles.z));
            _inGameManager_s.GetFaceName();
            _inGameManager_s.EndRoateMode();
    }
}