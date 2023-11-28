using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeUI_s : Singleton<CubeUI_s>
{
    [SerializeField] private Transform _cubeEffectTsf;
    private ParticleSystem _cubeEffect;
    private List<GameObject> _cubeEffectList;
    [SerializeField] private ParticleSystem _cubeHitEffectGood;
    [SerializeField] private ParticleSystem _cubeHitEffectPerfect;
    private void Start()
    {
        _cubeEffect = _cubeEffectTsf.GetComponent<ParticleSystem>();
        _cubeEffectList = new List<GameObject>();
        for(int i=0;i<6;i++)
        {
            _cubeEffectList.Add(_cubeEffectTsf.GetChild(i).gameObject);
        }
        InGameFunBind_s.Instance.Epause += Pause;
        InGameFunBind_s.Instance.EunPause += UnPause;
    }
    private void Pause()
    {
        foreach(ParticleSystem p in _cubeEffect.GetComponentsInChildren<ParticleSystem>())
        {
            var main = p.main;
            main.simulationSpeed = 0;
        }
    }
    private void UnPause()
    {
        foreach (ParticleSystem p in _cubeEffect.GetComponentsInChildren<ParticleSystem>())
        {
            var main = p.main;
            main.simulationSpeed = 1;
        }
    }
    public void ShowEffect(ECubeFace face)
    {
        for(int i=0;i<_cubeEffectList.Count;i++)
        {
            _cubeEffectList[i].SetActive(false);
        }
        _cubeEffectList[(int)face].SetActive(true);
        _cubeEffect.Play();
    }
    public void HitEffect(EBeatJudgement count)
    {
        switch(count)
        {
            case EBeatJudgement.Good:
                _cubeHitEffectGood.Play();
                break;
            case EBeatJudgement.Perfect:
                _cubeHitEffectPerfect.Play();
                break;
            default:
                break;
        }
    }
}
