using Spine.Unity;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI_s : Singleton<EnemyUI_s>
{
    [Header("HP")]
    [SerializeField] private HPBarController hpController;

    [Header("Effect")]
    [SerializeField] private ParticleSystem _enemyHitEffect;

    [SerializeField] private GameObject _enemyImage;
    [SerializeField] private SkeletonAnimation _animation;

    [SerializeField] private ParticleSystem _dieParticle;
    private void Start()
    {
        InGameFunBind_s.Instance.Epause += Pause;
        InGameFunBind_s.Instance.EunPause += UnPause;
    }
    public void UnShowEnemy(bool instance)
    {
        _enemyImage.SetActive(instance);
        _dieParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
    public void FadeEnemy(float time)
    {
        _dieParticle.Play();
        FadeUtlity.Instance.CallFade(time, _enemyImage, EGameObjectType.Skeleton, EFadeType.FadeIn);
    }
    public void SingleEnemyAnimation(string name, bool isLoop)
    {
        _animation.AnimationState.SetAnimation(0, name, isLoop);
    }
    public void MutipleEnemyAnimation(List<string> data)
    {
        Animation.ShowCharacterAnimation(data, _animation);
    }
    public void Pause()
    {
        _animation.timeScale = 0;
    }
    public void UnPause()
    {
        _animation.timeScale = 1;
    }
    public void EnemyHPDown()
    {
        _enemyHitEffect.Play();
        MutipleEnemyAnimation(new List<string>() { "hit", "idle" });
    }
    public void EnemyHPUpdate(int attackLevel)
    {
        hpController.MonsterDamage(attackLevel);
    }
}
