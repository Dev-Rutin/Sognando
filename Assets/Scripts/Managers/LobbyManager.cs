using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : Singleton<LobbyManager>
{
    
    [SerializeField] private GameObject _fadePenal;
    [SerializeField] private float _fadeTime;
    [SerializeField] private GameObject _dataObject;
    private bool _isFade;

    private int _continueStage;
    // Start is called before the first frame update
    void Start()
    {
        FadeUtlity.Instance.CallFade(_fadeTime, _fadePenal, EGameObjectType.UI, EFadeType.FadeIn);
        //_continueStage = PlayerPrefs.GetInt("continueStage");
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

    private IEnumerator ChangeScene(string Scenename)
    {
        FadeUtlity.Instance.CallFade(_fadeTime, _fadePenal, EGameObjectType.UI, EFadeType.FadeOut);
        while (_fadePenal.GetComponent<CanvasGroup>().alpha > 0)
        {
            yield return null;
        }
        SceneManager.LoadScene(Scenename);
    }

}
