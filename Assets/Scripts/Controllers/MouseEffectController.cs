using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEffectController : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private Vector3 pos;
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void FixedUpdate()
    {
        pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _particleSystem.Play();
        }
    }
}
