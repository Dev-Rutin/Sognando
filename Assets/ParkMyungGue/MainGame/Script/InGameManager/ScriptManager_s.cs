using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ScriptManager_s : MonoBehaviour
{
    private InGameFunBind_s m_inGamefunBind_s;
    public InGameFunBind_s _inGamefunBind_s { get => m_inGamefunBind_s;}

    [SerializeField] private InGameMusicManager_s m_inGameMusicManager_s;
    public InGameMusicManager_s _inGameMusicManager_s { get=>m_inGameMusicManager_s; }

    [SerializeField] private InGameBeatManager_s m_inGameBeatManager_s;
    public InGameBeatManager_s _inGameBeatManager_s { get => m_inGameBeatManager_s; }

    [SerializeField] private InGameSideData_s m_inGameSideData_s;
    public InGameSideData_s _inGameSideData_s { get => m_inGameSideData_s; }

    [SerializeField] private InGameManager_s m_inGameManager_s;
    public InGameManager_s _inGameManager_s { get => m_inGameManager_s; }

    [SerializeField] private KeyInputManager_s m_keyInputManager_s;
    public KeyInputManager_s _keyInputManager_s { get => m_keyInputManager_s; }

    [SerializeField] private InGamePlayer_s m_inGamePlayer_s;
    public InGamePlayer_s _inGamePlayer_s { get => m_inGamePlayer_s; }

    [SerializeField] private InGameEnemy_s m_inGameEnemy_s;
    public InGameEnemy_s _inGameEnemy_s { get => m_inGameEnemy_s; }

    [SerializeField] private InGameCube_s m_inGameCube_s;
    public InGameCube_s _inGameCube_s { get => m_inGameCube_s; }

    private void Awake()
    {
        m_inGamefunBind_s = new InGameFunBind_s();
        _inGameMusicManager_s.ScriptBind(this);
        _inGameMusicManager_s.InGameBind();
        _inGameBeatManager_s.ScriptBind(this);
        _inGameBeatManager_s.InGameBind();
        _inGameSideData_s.ScriptBind(this);
        _inGameSideData_s.InGameBind();
        _inGameManager_s.ScriptBind(this);
        _keyInputManager_s.ScriptBind(this);
        _keyInputManager_s.InGameBind();
        _inGamePlayer_s.ScriptBind(this);
        _inGamePlayer_s.InGameBind();
        _inGameEnemy_s.ScriptBind(this);
        _inGameEnemy_s.InGameBind();
        _inGameCube_s.ScriptBind(this);
        _inGameCube_s.InGameBind();
    }
    
}
