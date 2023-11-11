using System;
using System.Collections.Generic;
using UnityEngine;

public partial class KeyInputManager_s : MonoBehaviour,IInGame
{
    [Header("script")]
    private ScriptManager_s _scripts;

    [Header("data")]
    private Dictionary<KeyCode, Action> _otherKeyBinds;
    private Dictionary<KeyCode, Func<bool>> _cubeKeyBinds;
    private Dictionary<KeyCode, Action> _playerKeyBinds;
    private bool _isInput;
    private Queue<KeyCode> _inGameInputQueue;
    public bool cubeRotateClear { get; private set; }
    private bool _isFirstInput;

    public void ScriptBind(ScriptManager_s script)
    {
        _scripts = script;
    }
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

        KeyBind(ref _otherKeyBinds, KeyCode.Space, () => _scripts._inGameManager_s.GamePause());
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
            binddic.Add(keycode, () => _scripts._inGamePlayer_s.MovePlayer(movePos,_scripts._inGameSideData_s.divideSize));
        }
    }
    private void CubeKeyBind(ref Dictionary<KeyCode, Func<bool>> binddic, KeyCode keycode, Vector3 rotatePosition)
    {
        if (!_otherKeyBinds.ContainsKey(keycode) && !binddic.ContainsKey(keycode))
        {
            binddic.Add(keycode, () => _scripts._inGameCube_s.RotateCube(rotatePosition));
        }
    }
}
public partial class KeyInputManager_s //InGame Setting
{
    public void InGameBind()
    {
        _scripts._inGamefunBind_s.EgameStart += GameStart;
        _scripts._inGamefunBind_s.EgamePlay += GamePlay;
        _scripts._inGamefunBind_s.EgameEnd += GameEnd;
        _scripts._inGamefunBind_s.EmoveNextBit += MoveNextBit;
        _scripts._inGamefunBind_s.EchangeInGameState += ChangeInGameStatus;
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
                    _scripts._inGameManager_s.DefaultShow();
                }
                _isInput = false;
                break;
            case EInGameStatus.CUBEROTATE:
                if (cubeRotateClear==false)
                {
                    _isInput = false;
                    _scripts._inGameManager_s.DefaultShow();
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
                break;
            default:
                break;
        }
    }
    private void Update()
    {
        if (_scripts._inGameManager_s.curGameStatus == EGameStatus.PLAYING)
        {
            if (Input.anyKeyDown)
            {
                if (_scripts._inGameManager_s.curInGameStatus == EInGameStatus.PLAYERMOVE|| _scripts._inGameManager_s.curInGameStatus == EInGameStatus.CUBEROTATE)
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
                            _scripts._inGameEnemy_s.EnemyPhaseChange(EEnemyPhase.Phase1);
                        }
                        if (_scripts._inGameBeatManager_s.BeatJudgement())
                        {
                            if (_scripts._inGameManager_s.curInGameStatus == EInGameStatus.PLAYERMOVE)
                            {
                                _playerKeyBinds[_inGameInputQueue.Peek()]();
                            }
                            else if (_scripts._inGameManager_s.curInGameStatus == EInGameStatus.CUBEROTATE)
                            {
                                cubeRotateClear= _cubeKeyBinds[_inGameInputQueue.Peek()]();
                            }
                        }
                        else if(_scripts._inGameManager_s.curInGameStatus == EInGameStatus.PLAYERMOVE)
                        {
                            _scripts._inGameManager_s.MissScore();
                        }
                        _inGameInputQueue.Clear();
                    }
                }
                if (_scripts._inGameManager_s.curGameStatus == EGameStatus.PLAYING) //인게임 외의 입력 ex)일시정지
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
        if (_scripts._inGameManager_s.curInGameStatus == EInGameStatus.PLAYERMOVE)
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
        else if (_scripts._inGameManager_s.curInGameStatus == EInGameStatus.CUBEROTATE)
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
