using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternMaker_s : MonoBehaviour
{

    [SerializeField] ECurCubeFace _curCubeFace;
    // Start is called before the first frame update
    void Start()
    {
        GetCubeImage(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GetCubeImage()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(1920 / 2, 1080 / 2, 0));
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        _curCubeFace = (ECurCubeFace)Enum.Parse(typeof(ECurCubeFace), hit.transform.name);
    }
}
