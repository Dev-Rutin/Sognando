using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPattern : Singleton<PathPattern>
{
    private void GetRandomePath()
    {
        
    }
    private void EndRandomePath()
    {
    }
    public void Action(EInGameStatus inGameStatus)
    {
        switch(inGameStatus)
        {
            case EInGameStatus.SHOWPATH:
                break;
            default:
                break;
        }
    }
}
