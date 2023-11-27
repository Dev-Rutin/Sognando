using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingUtility : Singleton<SettingUtility>
{
    [SerializeField] private GameObject _soundMenu;
    [SerializeField] private Button _closeButton;
    [SerializeField] private float _moveSpeed;
    private Slider[] _sliders;
    private Toggle[] _toggles;
    private Vector3 _closePos = new Vector3(-446, 0, 0);
    private Vector3 _openPos = new Vector3(436, 0, 0);
    private void Start()
    {
        _closeButton.onClick.AddListener(CloseSetting);
        _sliders = _soundMenu.GetComponentsInChildren<Slider>();
        _toggles = _soundMenu.GetComponentsInChildren<Toggle>();
        _closePos = _soundMenu.transform.position;
        _closePos.x = -446;
        _openPos = _closePos;
        _openPos.x = 436;
        int i = 0;
        foreach (var slider in _sliders)
        {
            var index = i;
            slider.onValueChanged.AddListener( start => { SetSound(index); });
            ++i;
        }

        i = 0;
        foreach (var toggle in _toggles)
        {
            var index = i;
            toggle.onValueChanged.AddListener( start => { ToggleSound(index, toggle.isOn); });
            ++i;
        }
        
    }

    private void SetSound(int index)
    {
        switch (index)
        {
            case 0:
                SoundUtility.Instance.SetMasterVolume(_sliders[index].value);
                break;
            case 1:
                SoundUtility.Instance.SetBGMVolume(_sliders[index].value);
                break;
            case 2:
                SoundUtility.Instance.SetSFXVolume(_sliders[index].value);
                break;
            case 3:
                SoundUtility.Instance.SetAMBVolume(_sliders[index].value);
                break;
            default:
                break;
        }
    }

    private void ToggleSound(int index, bool toggle)
    {
        float value = toggle ? 0 : _sliders[index].value;
        _sliders[index].interactable = !toggle;
        switch (index)
        {
            case 0:
                SoundUtility.Instance.SetMasterVolume(value);
                break;
            case 1:
                SoundUtility.Instance.SetBGMVolume(value);
                break;
            case 2:
                SoundUtility.Instance.SetSFXVolume(value);
                break;
            case 3:
                SoundUtility.Instance.SetAMBVolume(value);
                break;
            default:
                break;
        }
    }

    private void CloseSetting()
    {
        Debug.Log("close btn");
        StartCoroutine(CloseSettingPenal());
    }

    private IEnumerator CloseSettingPenal()
    {
        Vector3 movePos = _soundMenu.transform.position;
        float moveTime = 0;
        while (movePos != _closePos)
        {
            moveTime += Time.deltaTime;
            movePos = Vector3.Lerp(_openPos, _closePos, moveTime / _moveSpeed);
            _soundMenu.transform.position = movePos;
            yield return null;
        }
    }

    public void OpenSetting()
    {
        Debug.Log("open btn");
        StartCoroutine(OpenSettingPenal());
    }

    private IEnumerator OpenSettingPenal()
    {
        Vector3 movePos = _soundMenu.transform.position;
        float moveTime = 0;
        while (movePos != _openPos)
        {
            moveTime += Time.deltaTime;
            movePos = Vector3.Lerp(_closePos, _openPos, moveTime / _moveSpeed);
            _soundMenu.transform.position = movePos;
            yield return null;
        }
    }
}
