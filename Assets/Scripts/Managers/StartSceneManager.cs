using System;
using System.Collections;
using System.Collections.Generic;
using FMODPlus;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject _fade;
    [SerializeField] private float _fadeTime;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _endButton;
    [SerializeField] private CommandSender _commandSender;
    
    private bool _isFade;
    // Update is called once per frame
    

    private void Start()
    {
        _startButton.onClick.AddListener(StartButton);
        _endButton.onClick.AddListener(ExitGame);
        _startButton.Select();
        FadeUtlity.Instance.CallFade(_fadeTime, _fade, EGameObjectType.UI, EFadeType.FadeIn);
        _commandSender.SendCommand();
    }
    
    private void StartButton()
    {
        FadeUtlity.Instance.CallFade(_fadeTime, _fade, EGameObjectType.UI, EFadeType.FadeOut);

        StartCoroutine(SceneChange());
    }

    private IEnumerator SceneChange()
    {
        while (_fade.GetComponent<CanvasGroup>().alpha < 1)
        {
            yield return null;
        }
        SoundUtility.Instance.StopSound(ESoundTypes.BGM, true);
        SceneManager.LoadScene("LobbyScene");
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
