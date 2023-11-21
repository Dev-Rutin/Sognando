using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundController : Singleton<BackGroundController>
{
    [SerializeField] private GameObject _phase1;
    [SerializeField] private GameObject _phase2;
    [SerializeField] private GameObject _phase3;
    [SerializeField] private GameObject _phase4;
    [SerializeField] private GameObject _phase5;
    
    public void StartPhase1()
    {
        _phase1.GetComponent<Phase1Controller>().PhaseStart();
    }
    public void StartPhase2()
    {
        _phase2.GetComponent<Phase2Controller>().PhaseStart();
    }
    public void StartPhase3()
    {
        _phase3.GetComponent<Phase3Controller>().PhaseStart();
    }
    public void StartPhase4()
    {
        _phase4.GetComponent<Phase4Controller>().PhaseStart();
    }
    public void StartPhase5()
    {
        _phase5.GetComponent<Phase5Controller>().PhaseStart();
    }
}
