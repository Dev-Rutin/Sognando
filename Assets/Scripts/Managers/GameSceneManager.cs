
using System;
using System.Collections;
using System.Collections.Generic;
using FMODPlus;
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
    [SerializeField] private GameObject _mainGameUICanvas;
    [SerializeField] private GameObject _BG;

    [SerializeField] private float _cubeAnimationWait;
    [SerializeField] private CommandSender _cubeAnimationSound;


    public bool isAnimationStart;

    private void Awake()
    {
        _player.GetComponent<SkeletonAnimation>().Skeleton.A = 0;
        _fairy.GetComponent<SkeletonAnimation>().Skeleton.A = 0;
        InGameMusicManager_s.Instance.easyMode = !StageDataController.Instance.isHardModeOn;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartGame());
    }
    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1.0f);
        _cubeAnimation.SetActive(true);
        _cubeAnimationSound.SendCommand();
        _cubeAnimation.GetComponent<SkeletonAnimation>().AnimationName = "cube_animation";
        

        yield return new WaitForSeconds(_cubeAnimationWait);
        _cube.SetActive(true);
        _BG.SetActive(true);
        
        yield return new WaitForSeconds(0.2f);

        FadeUtlity.Instance.CallFade(_fadeTime - 2f, _systemBGCanvas, EGameObjectType.UI, EFadeType.FadeOut);
        FadeUtlity.Instance.CallFade(_fadeTime - 2f, _player, EGameObjectType.Skeleton, EFadeType.FadeOut);
        FadeUtlity.Instance.CallFade(_fadeTime - 2f, _fairy, EGameObjectType.Skeleton, EFadeType.FadeOut);
        yield return new WaitForSeconds(_fadeTime - 1.0f);
        _player.GetComponent<SkeletonAnimation>().Skeleton.A = 1;
        _fairy.GetComponent<SkeletonAnimation>().Skeleton.A = 1;
        FadeUtlity.Instance.CallFade(_fadeTime - 2.5f, _systemCanvas, EGameObjectType.UI, EFadeType.FadeOut);
        yield return new WaitForSeconds(_fadeTime - 2.0f);
        
        FadeUtlity.Instance.CallFade(_fadeTime - 2.5f, _mainGameCanvas, EGameObjectType.UI, EFadeType.FadeOut);
        FadeUtlity.Instance.CallFade(_fadeTime - 2.5f, _mainGameUICanvas, EGameObjectType.UI, EFadeType.FadeOut);
        yield return new WaitForSeconds(_fadeTime - 2.0f);
        
        _creature.SetActive(true);
        _gameManagerObject.GetComponent<InGameManager_s>().GameStart();
        
    }
}
