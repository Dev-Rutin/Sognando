//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class GameSceneManager : Singleton<GameSceneManager>
//{
//    [SerializeField] private GameObject _fadePenal;
//    [SerializeField] private GameObject _gameManagerObject;
//    [SerializeField] private float _fadeTime;
    
//    // Start is called before the first frame update
//    void Start()
//    {
//        StartCoroutine(StartGame());
//    }
//    private IEnumerator StartGame()
//    {
//        FadeUtlity.Instance.CallFade(_fadeTime, _fadePenal, EGameObjectType.UI, EFadeType.FadeIn);
//        while (_fadePenal.GetComponent<CanvasGroup>().alpha != 0)
//        {
//            yield return null;
//        }
//        _gameManagerObject.GetComponent<DivideCube_s>().GameStart();
//    }
//}
