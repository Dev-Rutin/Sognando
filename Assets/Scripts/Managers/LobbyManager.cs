using System;
using System.Collections;
using System.Collections.Generic;
using FMODPlus;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : Singleton<LobbyManager>
{
    
    [SerializeField] private GameObject _fadePenal;
    [SerializeField] private float _fadeTime;
    [SerializeField] private GameObject _dataObject;
    [SerializeField] private CommandSender _commandSender;
    [SerializeField] private GameObject StageSelect;

    private bool _isFade;

    private int _continueStage;
    // Start is called before the first frame update
    void Start()
    {
        FadeUtlity.Instance.CallFade(_fadeTime, _fadePenal, EGameObjectType.UI, EFadeType.FadeIn);
        //_continueStage = PlayerPrefs.GetInt("continueStage");
        StartCoroutine(RaySetting());
        _commandSender.SendCommand();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StageSelect.SetActive(false);
        }
        if (StageSelect.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            
            int stage = StageDataController.Instance.stage;
            switch (stage)
            {
                case 1:
                    LoadScene("GameSceneStage1");
                    break;
                case 2:
                    if (PlayerPrefs.GetInt(PlayerPrefsKeyNames.STAGE1CLEARCHECK) == 1)
                    {
                        LoadScene("GameSceneStage2");
                    }
                    break;
            }
        }
    }

    public void StartGame()
    {
        FadeUtlity.Instance.CallFade(_fadeTime, _fadePenal, EGameObjectType.UI, EFadeType.FadeOut);
        _dataObject.GetComponent<StageDataController>().stage = 1;
        _isFade = true;
        //SoundUtility.Instance.StopSound(ESoundTypes.Bgm);
    }

    public void LoadScene(string name)
    {
        StartCoroutine(ChangeScene(name));
    }

    private IEnumerator ChangeScene(string SceneName)
    {
        _fadePenal.GetComponent<Image>().raycastTarget = true;
        FadeUtlity.Instance.CallFade(_fadeTime, _fadePenal, EGameObjectType.UI, EFadeType.FadeOut);
        while (_fadePenal.GetComponent<CanvasGroup>().alpha < 1)
        {
            yield return null;
        }
        SoundUtility.Instance.StopSound(ESoundTypes.BGM, true);
        SceneManager.LoadScene(SceneName);
    }

    private IEnumerator RaySetting()
    {
        while (_fadePenal.GetComponent<CanvasGroup>().alpha > 0)
        {
            yield return null;
        }
        _fadePenal.GetComponent<Image>().raycastTarget = false;
    }

    public void OpenStageSelect()
    {
        StageSelect.SetActive(true);
    }

}
