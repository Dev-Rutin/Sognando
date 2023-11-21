using System;
using System.Collections;
using System.Collections.Generic;
using FMODPlus;
using UnityEngine;


public class SoundSenderController : MonoBehaviour
{
    private CommandSender sender;
    
    public ESoundTypes audioType = ESoundTypes.BGM;

    private void Awake()
    {
        sender = GetComponent<CommandSender>();
        switch (audioType)
        {
            case ESoundTypes.AMB:
                sender.audioSource = SoundUtility.Instance.AMBAudioSource;
                break;
            case ESoundTypes.BGM:
                sender.audioSource = SoundUtility.Instance.BGMAudioSource;
                break;
            case ESoundTypes.SFX:
                sender.audioSource = SoundUtility.Instance.SFXAudioSource;
                break;
        } 
    }

    public void KeyOff()
    {
        switch (audioType)
        {
            case ESoundTypes.AMB:
                SoundUtility.Instance.AMBAudioSource.KeyOff();
                break;
            case ESoundTypes.BGM:
                SoundUtility.Instance.BGMAudioSource.KeyOff();
                break;
            case ESoundTypes.SFX:
                SoundUtility.Instance.SFXAudioSource.KeyOff();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
