using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeUI_s : Singleton<CubeUI_s>
{
    [SerializeField] private Transform _cubeEffectTsf;
    private ParticleSystem _cubeEffect;
    private List<GameObject> _cubeEffectList;
    private void Start()
    { 
        _cubeEffect = _cubeEffectTsf.GetComponent<ParticleSystem>();
        _cubeEffectList = new List<GameObject>();
        for(int i=0;i<6;i++)
        {
            _cubeEffectList.Add(_cubeEffectTsf.GetChild(i).gameObject);
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
}
