<<<<<<< HEAD
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
=======
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using Animation = UnityEngine.Animation;

public class GameSceneManager : Singleton<GameSceneManager>
{
    [SerializeField] private GameObject _fadePenal;
    [SerializeField] private GameObject _gameManagerObject;
    [SerializeField] private float _fadeTime;
    [SerializeField] private GameObject _cubeAnimation;
    [SerializeField] private GameObject _cube;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _fairy;
    [SerializeField] private GameObject _creature;
    [SerializeField] private GameObject _systemCanvas;
    [SerializeField] private GameObject _systemBGCanvas;
    [SerializeField] private GameObject _mainGameCanvas;


    public bool isAnimationStart;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartGame());
    }
    private IEnumerator StartGame()
    {
        FadeUtlity.Instance.CallFade(_fadeTime, _fadePenal, EGameObjectType.UI, EFadeType.FadeIn);
        while (_fadePenal.GetComponent<CanvasGroup>().alpha != 0)
        {
            yield return null;
        }
        
        /*BackGroundController.Instance.StartPhase1();

        while (!Phase1Controller.Instance.isPhaseEnd)
        {
            yield return null;
        }
        
        isAnimationStart = true;
        
        _cubeAnimation.SetActive(true);
        _cubeAnimation.GetComponent<SkeletonAnimation>().AnimationName = "cube_animation";

        
        yield return new WaitForSeconds(6.0f);
        _cube.SetActive(true);
        _cube.GetComponent<Animation>().Play();

        yield return new WaitForSeconds(3.5f);
        
        _cube.GetComponent<Animation>().Stop();*/
        _cube.SetActive(true);
        FadeUtlity.Instance.CallFade(_fadeTime - 2.5f, _systemBGCanvas, EGameObjectType.UI, EFadeType.FadeOut);
        yield return new WaitForSeconds(_fadeTime - 2.0f);
        
        FadeUtlity.Instance.CallFade(_fadeTime - 2.5f, _player, EGameObjectType.GameObject, EFadeType.FadeOut);
        FadeUtlity.Instance.CallFade(_fadeTime - 2.5f, _creature, EGameObjectType.GameObject, EFadeType.FadeOut);
        _fairy.SetActive(true);
        yield return new WaitForSeconds(_fadeTime - 2.0f);
        
        FadeUtlity.Instance.CallFade(_fadeTime - 2.5f, _systemCanvas, EGameObjectType.UI, EFadeType.FadeOut);
        yield return new WaitForSeconds(_fadeTime - 2.0f);
        
        FadeUtlity.Instance.CallFade(_fadeTime - 2.5f, _mainGameCanvas, EGameObjectType.UI, EFadeType.FadeOut);
        yield return new WaitForSeconds(_fadeTime - 2.0f);
        
        _gameManagerObject.GetComponent<InGameManager_s>().GameStart();
    }
}
>>>>>>> origin/feature_kohdeawook
