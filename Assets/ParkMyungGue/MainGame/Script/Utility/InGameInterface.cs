using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
interface IInGame
{
    public void InGameBind();
    public void GameStart(); //in game data initialize Before Game Start
    public void GamePlay(); //real game start(ui show)
    public void GameEnd();
    public void MoveNextBit(EInGameStatus curInGameStatus);
    public void ChangeInGameStatus(EInGameStatus changeTarget);
}
interface IScript
{
    public void ScriptBind(ScriptManager_s script);
}
interface IAudio
{
    public void AudioPlay(AudioClip clip);
    public void AudioStop(AudioClip clip);
    public void AudioPause();
    public void AudioUnPause(TimeSpan time);
    public void ChangeVolume(float volume);
}