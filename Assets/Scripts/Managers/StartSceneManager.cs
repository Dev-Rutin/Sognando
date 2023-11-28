using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject _fade;
    [SerializeField] private float _fadeTime;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _endButton;
    
    private bool _isFade;
    // Update is called once per frame
    

    private void Start()
    {
        _startButton.onClick.AddListener(StartButton);
        _endButton.onClick.AddListener(ExitGame);
        _startButton.Select();
        FadeUtlity.Instance.CallFade(_fadeTime, _fade, EGameObjectType.UI, EFadeType.FadeIn);
    }
    
    private void StartButton()
    {
        _isFade = true;
        FadeUtlity.Instance.CallFade(_fadeTime, _fade, EGameObjectType.UI, EFadeType.FadeOut);
        
        if (_isFade && _fade.GetComponent<CanvasGroup>().alpha == 1)
        {
            SceneManager.LoadScene("LobbyScene");
        }
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
