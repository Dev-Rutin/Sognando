using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : Singleton<LobbyManager>
{
    
    [SerializeField] private GameObject _fadePenal;
    [SerializeField] private float _fadeTime;
    [SerializeField] private string _sceneName;
    [SerializeField] private GameObject _dataObject;
    private bool _isFade;

    private int _continueStage;
    // Start is called before the first frame update
    void Start()
    {
        FadeUtlity.Instance.CallFade(_fadeTime, _fadePenal, EGameObjectType.UI, EFadeType.FadeIn);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isFade && _fadePenal.GetComponent<CanvasGroup>().alpha == 1)
        {
            SceneManager.LoadScene(_sceneName);
        }
    }

    public void StartGame()
    {
        FadeUtlity.Instance.CallFade(_fadeTime, _fadePenal, EGameObjectType.UI, EFadeType.FadeOut);
        _dataObject.GetComponent<StageDataController>().stage = 1;
        _isFade = true;
        
    }

}
