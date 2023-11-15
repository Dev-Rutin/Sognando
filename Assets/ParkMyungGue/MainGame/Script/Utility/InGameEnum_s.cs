
public enum EPlayerAttackLevel
{
    NONE,
    ONE,
    TWO,
    THREE
}
public enum EStage
{
    NONE,
    STAGE_ONE,
    STAGE_TWO,
    STAGE_THREE,
    STAGE_FOUR,
    STAGE_FIVE,
    STAGE_SIX
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
    END
}
public enum EInGameStatus
{
    NONE,
    SHOWPATH,
    PLAYERMOVE,
    CUBEROTATE,
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
    NOISE,
    LINEATTACK,
    LINKLINEATTACK
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