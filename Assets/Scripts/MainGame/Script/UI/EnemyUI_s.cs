using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI_s : Singleton<EnemyUI_s>
{
    [Header("Effect")]
    [SerializeField] private ParticleSystem _enemyHitEffect;
    public void EnemyHPDown(ECubeFace face)
    {
        _enemyHitEffect.Play();
    }
}
