using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class NoisePattern_s : Singleton<NoisePattern_s>
{
    [Header("data")]
    [SerializeField]private GameObject _noisePrefab;
    [SerializeField] private Transform _noiseTsf;
    [SerializeField] private int _noiseCreateCount;
    public void GetFirstNoisePattern()
    {
        InGameEnemy_s.Instance.SetObj("noise", _noisePrefab, false, _noiseTsf, new Vector2Int(2, 1));
        InGameEnemy_s.Instance.SetObj("noise", _noisePrefab, false, _noiseTsf, new Vector2Int(1, 2));
        InGameEnemy_s.Instance.SetObj("noise", _noisePrefab, false, _noiseTsf, new Vector2Int(3, 3));
    }
    public void GetNoisePattern()
    {
        InGameEnemy_s.Instance.GetRandomeObjs("noise", _noisePrefab, false, _noiseTsf, _noiseCreateCount);
    }

    public void EndNoisePattern()
    {
        InGameEnemy_s.Instance.RemoveAllTargetObj("noise", true);
    }
    public void Action(EInGameStatus status)
    {
        if (status == EInGameStatus.SHOWPATH)
        {
            if (InGameEnemy_s.Instance.curEnemyPhase == EEnemyPhase.Ghost)
            {
                GetFirstNoisePattern();
            }
            else
            {
                GetNoisePattern();
            }
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
