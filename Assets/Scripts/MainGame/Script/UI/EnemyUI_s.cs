using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI_s : Singleton<EnemyUI_s>
{
    [Header("HP")]
    [SerializeField] private Slider _enemyHPSlider;

    [Header("Effect")]
    [SerializeField] private ParticleSystem _enemyHitEffect;

    public void EnemyHPInitialize(int maxValue)
    {
        _enemyHPSlider.maxValue = maxValue;
    }
    public void EnemyHPDown()
    {
        _enemyHitEffect.Play();
    }
    public void EnemyHPUpdate(int curEnemyHP)
    {
        _enemyHPSlider.value = curEnemyHP;
    }
}
