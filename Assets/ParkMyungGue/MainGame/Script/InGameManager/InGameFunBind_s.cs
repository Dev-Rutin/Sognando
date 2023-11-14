using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InGameFunBind_s:Singleton<InGameFunBind_s>
{
    //event
    public delegate void voidHandler();
    public event voidHandler EgameStart;
    public event voidHandler EgamePlay;
    public event voidHandler EgameEnd;
    public delegate void NextBitHandler(EInGameStatus curInGameStatus);
    public event NextBitHandler EmoveNextBit;
    public delegate void ChangeInGameStateHandler(EInGameStatus changeTargets);
    public event ChangeInGameStateHandler EchangeInGameState;
    public void GameStart()
    {
        EgameStart();
    }
    public void GamePlay()
    {
        EgamePlay();
    }
    public void GameEnd()
    {
        EgameEnd();
    }
    public void MoveNextBit(EInGameStatus curInGameStatus)
    {
        EmoveNextBit(curInGameStatus);
    }
    public void ChangeInGameStatus(EInGameStatus changeTarget)
    {
        EchangeInGameState(changeTarget);
    }
}
