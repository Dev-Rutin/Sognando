using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EStage
{
    STAGE_ONE,
    STAGE_TWO,
    STAGE_THREE,
    STAGE_FOUR,
    STAGE_FIVE,
    STAGE_SIX
}
public enum EInputMode
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
public enum EGameStatus
{
    NONE,
    STARTWAITTING,
    PLAYING,
    ENDWAIT,
    END,
    PAUSE
}
public enum EInGameStatus
{
    NONE,
    SHOWPATH,
    PLAYERMOVE,
    TIMEWAIT
}
public enum ECubeFace
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
    GHOST,
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
    SHOW1,
    SHOW2,
    ATTACK
}
public enum ELinkLineAttackMode
{
    NONE,
    SHOW1,
    SHOW2,
    ATTACK
}
public enum EEnemyPhase
{
    None,
    Ghost,
    Phase1,
    Phase2,
    Phase3
}
public class InGameEnum_s
{
   
}
