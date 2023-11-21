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
    private Slider[] _sliders;
    private Toggle[] _toggles;
    private void Start()
    {
        _closeButton.onClick.AddListener(() => CloseSetting(gameObject));
        _sliders = _soundMenu.GetComponentsInChildren<Slider>();
        _toggles = _soundMenu.GetComponentsInChildren<Toggle>();
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

    private void CloseSetting(GameObject setting)
    {
        setting.GetComponent<Canvas>().enabled = false;
    }
}
