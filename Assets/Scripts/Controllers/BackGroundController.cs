using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundController : MonoBehaviour
{
    [SerializeField] private GameObject _phase1;
    [SerializeField] private GameObject _phase2;
    [SerializeField] private GameObject _phase3;
    [SerializeField] private GameObject _phase4;
    [SerializeField] private GameObject _phase5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("StartPhase1");
            _phase1.GetComponent<Phase1Controller>().PhaseStart();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("StartPhase2");
            _phase2.GetComponent<Phase2Controller>().PhaseStart();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("StartPhase3");
            _phase3.GetComponent<Phase3Controller>().PhaseStart();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("StartPhase4");
            _phase4.GetComponent<Phase4Controller>().PhaseStart();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("StartPhase5");
            _phase5.GetComponent<Phase5Controller>().PhaseStart();
        }
    }
}
