using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingUtility : Singleton<SettingUtility>
{
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private GameObject _soundMenu;
    [SerializeField] private Button _closeButton;
    private AudioMixerGroup[] _mixerGroups;
    private List<AudioMixerGroup> _mixerGroupList;
    private Slider[] _sliders;
    private void Start()
    {
        /*_mixerGroups = _mixer.FindMatchingGroups("Master");
        _mixerGroupList = new List<AudioMixerGroup>();
        foreach (var group in _mixerGroups)
        {
            _mixerGroupList.Add(group);
            Debug.Log(group.name);
        }
        Debug.Log(_mixerGroupList.Count);*/
        
        
        _closeButton.onClick.AddListener(() => CloseSetting(gameObject));
        _sliders = _soundMenu.GetComponentsInChildren<Slider>();
        int i = 0;
        foreach (var slider in _sliders)
        {
            var index = i;
            slider.onValueChanged.AddListener( start => { SetSound(index); });
            ++i;
        }
    }

    private void SetSound(int index)
    {
        switch (index)
        {
            case 0:
                _mixer.SetFloat("Master", _sliders[index].value);
                Debug.Log($"Master : {_mixer.GetFloat("Master", out _)}");
                break;
            case 1:
                _mixer.SetFloat("BGM", _sliders[index].value);
                Debug.Log($"BGM : {_mixer.GetFloat("BGM", out _)}");
                break;
            case 2:
                _mixer.SetFloat("SE", _sliders[index].value);
                Debug.Log($"SE : {_mixer.GetFloat("SE", out _)}");
                break;
            default:
                break;
        }
    }

    private void CloseSetting(GameObject setting)
    {
        Debug.Log("call button");
        setting.GetComponent<Canvas>().enabled = false;
    }
}
