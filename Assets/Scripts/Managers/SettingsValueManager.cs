using UnityEngine;


public static class SettingsValueManager
{
    public static void ApplyPlayerPrefsValues(AudioSource _bgm, AudioSource _transition, AudioSource _interaction, AudioSource _interface)
    {
        // PlayerPrefs 적용
        float defaultValue = (float)ESettingsValue.On / 2f;
        float bgmValue = PlayerPrefs.GetFloat(PlayerPrefsKeyNames.BGM_VOLUME, defaultValue);
        float seValue = PlayerPrefs.GetFloat(PlayerPrefsKeyNames.SE_VOLUME, defaultValue);
        _bgm.volume = bgmValue;
        _transition.volume = seValue;
        _interaction.volume = seValue;
        _interface.volume = seValue;
    }
}