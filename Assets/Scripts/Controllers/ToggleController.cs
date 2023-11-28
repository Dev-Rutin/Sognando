using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    [SerializeField] private Sprite[] _changeImage;

    private Toggle _toggle;
    private void Awake()
    {
        _toggle = GetComponentInParent<Toggle>();
    }

    public void ChangeSptrie()
    {
        GetComponent<Image>().sprite = _toggle.isOn ? _changeImage[1] : _changeImage[0];
    }
}
