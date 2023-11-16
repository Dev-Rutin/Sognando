using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI_s : Singleton<EnemyUI_s>
{
    [Header("HP")]
    private Dictionary<ECubeFace,Transform> _hpsTsf;
    [SerializeField] private Vector3 _pumpSize;
    private Dictionary<ECubeFace, Image> _hpImages;
    private Dictionary<ECubeFace, Sprite> _hpOn;
    private Dictionary<ECubeFace, Sprite> _hpOff;
    [SerializeField] private Transform _hpUITsf;
    [Header("Effect")]
    [SerializeField] private ParticleSystem _enemyHitEffect;

    private void Start()
    {
        _hpsTsf = new Dictionary<ECubeFace, Transform>();
        foreach (Transform data in _hpUITsf)
        {
            _hpsTsf.Add(Enum.Parse<ECubeFace>(data.name), data);
        }
        _hpImages = new Dictionary<ECubeFace, Image>();
        foreach (Transform data in _hpUITsf)
        {
            _hpImages.Add(Enum.Parse<ECubeFace>(data.name),data.GetComponent<Image>());
        }
        _hpOn = new Dictionary<ECubeFace, Sprite>();
        foreach(Sprite data in  Resources.LoadAll<Sprite>("ParkMyungGue/CubeSideImage/ON"))
        {
            _hpOn.Add(Enum.Parse<ECubeFace>(data.name), data);
        }
        _hpOff = new Dictionary<ECubeFace, Sprite>();
        foreach (Sprite data in Resources.LoadAll<Sprite>("ParkMyungGue/CubeSideImage/OFF"))
        {
            _hpOff.Add(Enum.Parse<ECubeFace>(data.name), data);
        }
    }
    public void HPReset()
    {
        foreach(var data in _hpImages)
        {
            data.Value.sprite = _hpOn[data.Key];
        }
    }
    public void EnemyHPDown(ECubeFace face)
    {
        _hpImages[face].sprite = _hpOff[face];
        _enemyHitEffect.Play();
    }
    public void PumpHP(ECubeFace face)
    {
        StartCoroutine(ObjectAction.ObjectScalePump(_hpsTsf[face], _pumpSize, 0.1f));
    }
}
