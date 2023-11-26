using System;
using System.Collections.Generic;
using UnityEngine;

public partial class KeyInputManager_s : Singleton<KeyInputManager_s>, IInGame
{
    [Header("data")]
    private Dictionary<KeyCode, Action> _otherKeyBinds;
    private Dictionary<KeyCode, Func<bool>> _cubeKeyBinds;
    private Dictionary<KeyCode, Action> _playerKeyBinds;
    private bool _isInput;
    private Queue<KeyCode> _inGameInputQueue;
    public bool cubeRotateClear { get; private set; }
    private bool _isFirstInput;
    private void Start()
    {
        _otherKeyBinds = new Dictionary<KeyCode, Action>();
        _cubeKeyBinds = new Dictionary<KeyCode, Func<bool>>();
        _playerKeyBinds = new Dictionary<KeyCode, Action>();
        BindSetting();
        _inGameInputQueue = new Queue<KeyCode>();
    }
}
public partial class KeyInputManager_s //Data Setting
{
    private void BindSetting()
    {
        CubeKeyBind(ref _cubeKeyBinds, KeyCode.LeftArrow, new Vector3(0, -90, 0));
        CubeKeyBind(ref _cubeKeyBinds, KeyCode.RightArrow, new Vector3(0, 90, 0));
        CubeKeyBind(ref _cubeKeyBinds, KeyCode.UpArrow, new Vector3(-90, 0, 0));
        CubeKeyBind(ref _cubeKeyBinds, KeyCode.DownArrow, new Vector3(90, 0, 0));

        CubeKeyBind(ref _cubeKeyBinds, KeyCode.A, new Vector3(0, -90, 0));
        CubeKeyBind(ref _cubeKeyBinds, KeyCode.D, new Vector3(0, 90, 0));
        CubeKeyBind(ref _cubeKeyBinds, KeyCode.W, new Vector3(-90, 0, 0));
        CubeKeyBind(ref _cubeKeyBinds, KeyCode.S, new Vector3(90, 0, 0));

        PlayerKeyBind(ref _playerKeyBinds, KeyCode.LeftArrow, Vector2Int.left);
        PlayerKeyBind(ref _playerKeyBinds, KeyCode.RightArrow, Vector2Int.right);
        PlayerKeyBind(ref _playerKeyBinds, KeyCode.UpArrow, Vector2Int.down);
        PlayerKeyBind(ref _playerKeyBinds, KeyCode.DownArrow, Vector2Int.up);

        PlayerKeyBind(ref _playerKeyBinds, KeyCode.A, Vector2Int.left);
        PlayerKeyBind(ref _playerKeyBinds, KeyCode.D, Vector2Int.right);
        PlayerKeyBind(ref _playerKeyBinds, KeyCode.W, Vector2Int.down);
        PlayerKeyBind(ref _playerKeyBinds, KeyCode.S, Vector2Int.up);

        KeyBind(ref _otherKeyBinds, KeyCode.Space, () => InGameManager_s.Instance.GamePause());
    }
    private void KeyBind(ref Dictionary<KeyCode, Action> binddic, KeyCode keycode, Action action)
    {
        if (!_otherKeyBinds.ContainsKey(keycode) && !binddic.ContainsKey(keycode))
        {
            binddic.Add(keycode, action);
        }
    }
    private void PlayerKeyBind(ref Dictionary<KeyCode, Action> binddic, KeyCode keycode, Vector2Int movePos)
    {
        if (!_otherKeyBinds.ContainsKey(keycode) && !binddic.ContainsKey(keycode))
        {
            binddic.Add(keycode, () => InGamePlayer_s.Instance.MovePlayer(movePos,InGameSideData_s.Instance.divideSize));
        }
    }
    private void CubeKeyBind(ref Dictionary<KeyCode, Func<bool>> binddic, KeyCode keycode, Vector3 rotatePosition)
    {
        if (!_otherKeyBinds.ContainsKey(keycode) && !binddic.ContainsKey(keycode))
        {
            binddic.Add(keycode, () => InGameCube_s.Instance.RotateCube(rotatePosition));
        }
    }
}
public partial class KeyInputManager_s //InGame Setting
{
    public void InGameBind()
    {
        InGameFunBind_s.Instance.EgameStart += GameStart;
        InGameFunBind_s.Instance.EgamePlay += GamePlay;
        InGameFunBind_s.Instance.EgameEnd += GameEnd;
        InGameFunBind_s.Instance.EmoveNextBit += MoveNextBit;
        InGameFunBind_s.Instance.EchangeInGameState += ChangeInGameStatus;
    }
    public void GameStart()
    {
        _isInput = true;
        _inGameInputQueue.Clear();
        cubeRotateClear = false;
        _isFirstInput = false;
    }
    public void GamePlay()
    {
    }
    public void GameEnd()
    {

    }
    public void MoveNextBit(EInGameStatus curInGameStatus)
    {
        switch(curInGameStatus)
        {
            case EInGameStatus.PLAYERMOVE:
                if(!_isInput)
                {
                    InGameManager_s.Instance.DefaultShow();
                }
                _isInput = false;
                break;
            case EInGameStatus.CUBEROTATE:
                if (cubeRotateClear==false)
                {
                    _isInput = false;
                    InGameManager_s.Instance.DefaultShow();
                }
                break;
            default:
                break;
        }
    }
    public void ChangeInGameStatus(EInGameStatus changeTarget)
    {
        switch (changeTarget)
        {
            case EInGameStatus.PLAYERMOVE:
                break;
            case EInGameStatus.CUBEROTATE:
                cubeRotateClear = false;
                _isInput = true;
                break;
            default:
                break;
        }
    }
    private void Update()
    {
        if (InGameManager_s.Instance.curGameStatus == EGameStatus.PLAYING)
        {
            if (Input.anyKeyDown)
            {
                if (InGameManager_s.Instance.curInGameStatus == EInGameStatus.PLAYERMOVE|| InGameManager_s.Instance.curInGameStatus == EInGameStatus.CUBEROTATE)
                {
                    if (!_isInput)
                    {
                        InGameInputCheck();
                    }
                    if (_inGameInputQueue.Count != 0)
                    {
                        if(!_isFirstInput)
                        {
                            _isFirstInput = true;
                            InGameEnemy_s.Instance.EnemyPhaseChange(EEnemyPhase.Phase1);
                        }
                        if (InGameManager_s.Instance.curInGameStatus == EInGameStatus.PLAYERMOVE)
                        {
                            if (InGameBeatManager_s.Instance.BeatJudgement())
                            {
                                _playerKeyBinds[_inGameInputQueue.Peek()]();
                                InGameBeatManager_s.Instance.ShowHitEffect();
                            }
                            else
                            {
                                InGameManager_s.Instance.MissScore();
                            }
                        }
                        else if (InGameManager_s.Instance.curInGameStatus == EInGameStatus.CUBEROTATE)
                        {
                            cubeRotateClear = _cubeKeyBinds[_inGameInputQueue.Peek()]();
                            if (cubeRotateClear)
                            {
                                if(InGameBeatManager_s.Instance.BeatJudgement())
                                {
                                    InGameBeatManager_s.Instance.ShowHitEffect();
                                }
                                else
                                {
                                    Debug.Log("aa");
                                    InGameManager_s.Instance.MissScore();
                                }
                            }
                            else
                            {
                                InGameManager_s.Instance.MissScore();
                            }
                         }                 
                        _inGameInputQueue.Clear();
                    }
                }
                if (InGameManager_s.Instance.curGameStatus == EGameStatus.PLAYING) //인게임 외의 입력 ex)일시정지
                {
                    foreach (var dic in _otherKeyBinds)
                    {
                        if (Input.GetKey(dic.Key))
                        {
                            dic.Value();
                        }
                    }
                }
            }
        }
    }
    private void InGameInputCheck()
    {
        if (InGameManager_s.Instance.curInGameStatus == EInGameStatus.PLAYERMOVE)
        {
            foreach (var dic in _playerKeyBinds)
            {
                if (Input.GetKey(dic.Key))
                {
                    _isInput = true;
                    _inGameInputQueue.Enqueue(dic.Key);
                }
            }
        }
        else if (InGameManager_s.Instance.curInGameStatus == EInGameStatus.CUBEROTATE)
        {
            foreach (var dic in _cubeKeyBinds)
            {
                if (Input.GetKey(dic.Key))
                {
                    _isInput = true;
                    _inGameInputQueue.Enqueue(dic.Key);
                }
            }
        }
    }
}
