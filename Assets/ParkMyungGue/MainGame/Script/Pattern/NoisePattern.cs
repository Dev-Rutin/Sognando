using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class NoisePattern_s : MonoBehaviour
{
    [Header("script")]
    [SerializeField] private InGameEnemy_s _inGameEnemy_s;

    [Header("data")]
    [SerializeField]private GameObject _noisePrefab;
    [SerializeField] private Transform _noiseTsf;
    [SerializeField] private int _noiseCreateCount;
    public void GetNoisePattern()
    {
        _inGameEnemy_s.GetRandomeObjs("noise", _noisePrefab, false, _noiseTsf, _noiseCreateCount);
    }

    public void EndNoisePattern()
    {
        _inGameEnemy_s.RemoveAllTargetObj("noise", true);
    }
    public void Action(EInGameStatus status)
    {
        if (status == EInGameStatus.SHOWPATH)
        {
            GetNoisePattern();
        }
    }
    public bool isPatternEnd()
    {
        if(_noiseTsf.childCount==0)
        {
            return true;
        }
        return false;
    }
}
