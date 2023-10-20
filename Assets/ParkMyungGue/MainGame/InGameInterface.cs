using System.Collections;
using System.Collections.Generic;
using UnityEngine;
interface IUI
{
   /* public void UIOpen();
    public void UIClose();
    public void UIPause();

    public void UpdateCombo(int changevalue);
    public void UpdateScore(int changevalue);
    public void UpdatePlayerHP(int changevalue);
    public void UpdateEnemyHP(int changevalue);*/
}
interface IInGame
{
    public void MoveNextBit(EInGameStatus curInGameStatus);
    public void ChangeInGameStatus(EInGameStatus changeTarget);
}
interface IDataSetting
{
    public void DefaultDataSetting();
    public void GameStart();
    public void GameEnd();
} 
interface IScript
{
    public void ScriptBind(InGameManager_s gameManager);
}
public class InGameInterface
{

}
