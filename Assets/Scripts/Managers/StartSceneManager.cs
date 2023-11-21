using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject _startText;
    [SerializeField] private GameObject _fade;
    [SerializeField] private float _fadeTime;
    [SerializeField] private string _sceneName;
    private bool _isFade;
    // Update is called once per frame

    private void Start()
    {
        FadeUtlity.Instance.CallFade(_fadeTime, _fade, EGameObjectType.UI, EFadeType.FadeIn);
        FadeUtlity.Instance.BlinkUI(_fadeTime, _startText);
        //SoundUtility.Instance.PlaySound(ESoundTypes.Bgm, SceneSoundNames.INTRO_BGM);
    }

    private void Update()
    {
        if (Input.anyKeyDown && !_isFade)
        {
            FadeUtlity.Instance.StopFade();
            _isFade = true;
            FadeUtlity.Instance.CallFade(_fadeTime, _fade, EGameObjectType.UI, EFadeType.FadeOut);
        }

        if (_isFade && _fade.GetComponent<CanvasGroup>().alpha == 1)
        {
            SceneManager.LoadScene(_sceneName);
        }
    }
}
