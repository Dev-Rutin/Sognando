using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EStage
{
    STAGE_ONE,
    STAGE_TWO,
    STAGE_THREE,
    STAGE_FOUR,
    STAGE_FIVE
}
public enum ECubeMode
{
    ROTATE,
    MAINTAIN
}
public enum ERotatePosition
{
    NONE,
    LEFT,
    RIGHT,
    UP,
    DOWN
}
public enum EDivideGameStatus
{
    NONE,
    STARTWAITTING,
    PLAYING,
    END,
    PAUSE
}
public enum EInGameStatus
{
    SHOWPATH,
    PLAYERMOVE,
    TIMEWAIT
}
public enum ECurCubeFace
{
    ONE,
    TWO,
    THREE,
    FOUR,
    FIVE,
    SIX
}

public enum EEnemyMode
{
    PATH,
    COIN,
    LINEATTACK,
    LINKLINEATTACK,
    BLOCK,
    NEEDLEATTACK
}
public enum ELineAttackMode
{
    NONE,
    SHOW,
    ATTACK
}

public class DivideCubeEnum_s
{
   
}
