using UnityEngine;

public class DebugUI_s : Singleton<DebugUI_s>
{
    [SerializeField] private InGameManager_s _inGameManager_s;
    [SerializeField] GameObject _startButton;
    [SerializeField] GameObject _endButton;
    public void GameStart()
    {
        _inGameManager_s.GameStart();
        _startButton.SetActive(false);
    }
    public void GameEnd()
    {
        _inGameManager_s.GameEnd();
        _startButton?.SetActive(true);
    }
}
