using UnityEngine;

enum BGM
{
    Start,
    Lobby,
    Intro,
    Story1,
    Story2,
    Story3,
    Story4,
    Story5,
    Stage1,
    Stage2,
    Stage3,
    Stage4,
    Stage5
}
enum SE
{
    UISelect,
    UIOption,
    UIBack,
    UINotice,
    Bit,
    PlayerMove,
    PlayerAttack,
    PlayerHit,
    PlayerCharge,
    CreatureAttack,
    CreatureHit,
    CreatureCharge
}
public class SoundDataInput : MonoBehaviour
{
    [SerializeField] private AudioClip[] bgm;
    [SerializeField] private AudioClip[] se;

    // Start is called before the first frame update
    void Awake()
    {
        SoundUtility.Instance.RegistBgm(SceneSoundNames.START_BGM, bgm[(int)BGM.Start]);
        SoundUtility.Instance.RegistBgm(SceneSoundNames.LOBBY_BGM, bgm[(int)BGM.Lobby]);
        SoundUtility.Instance.RegistBgm(SceneSoundNames.INTRO_BGM, bgm[(int)BGM.Intro]);
        SoundUtility.Instance.RegistBgm(SceneSoundNames.STORY_BGM_01, bgm[(int)BGM.Story1]);
        SoundUtility.Instance.RegistBgm(SceneSoundNames.STORY_BGM_02, bgm[(int)BGM.Story2]);
        SoundUtility.Instance.RegistBgm(SceneSoundNames.STORY_BGM_03, bgm[(int)BGM.Story3]);
        SoundUtility.Instance.RegistBgm(SceneSoundNames.STORY_BGM_04, bgm[(int)BGM.Story4]);
        SoundUtility.Instance.RegistBgm(SceneSoundNames.STORY_BGM_05, bgm[(int)BGM.Story5]);
        SoundUtility.Instance.RegistBgm(SceneSoundNames.STAGE_BGM_01, bgm[(int)BGM.Stage1]);
        SoundUtility.Instance.RegistBgm(SceneSoundNames.STAGE_BGM_02, bgm[(int)BGM.Stage2]);
        SoundUtility.Instance.RegistBgm(SceneSoundNames.STAGE_BGM_03, bgm[(int)BGM.Stage3]);
        SoundUtility.Instance.RegistBgm(SceneSoundNames.STAGE_BGM_04, bgm[(int)BGM.Stage4]);
        SoundUtility.Instance.RegistBgm(SceneSoundNames.STAGE_BGM_05, bgm[(int)BGM.Stage5]);

        SoundUtility.Instance.RegistSe(SceneSoundNames.UI_SELECT, se[(int)SE.UISelect]);
        SoundUtility.Instance.RegistSe(SceneSoundNames.UI_OPTION, se[(int)SE.UIOption]);
        SoundUtility.Instance.RegistSe(SceneSoundNames.UI_BACK, se[(int)SE.UIBack]);
        SoundUtility.Instance.RegistSe(SceneSoundNames.UI_NOTICE, se[(int)SE.UINotice]);
        SoundUtility.Instance.RegistSe(SceneSoundNames.BIT, se[(int)SE.Bit]);
        SoundUtility.Instance.RegistSe(SceneSoundNames.PLAYER_MOVE, se[(int)SE.PlayerMove]);
        SoundUtility.Instance.RegistSe(SceneSoundNames.PLAYER_ATTACK, se[(int)SE.PlayerAttack]);
        SoundUtility.Instance.RegistSe(SceneSoundNames.PLAYER_HIT, se[(int)SE.PlayerHit]);
        SoundUtility.Instance.RegistSe(SceneSoundNames.PLAYER_CHARGE, se[(int)SE.PlayerCharge]);
        SoundUtility.Instance.RegistSe(SceneSoundNames.CREATURE_ATTACK, se[(int)SE.CreatureAttack]);
        SoundUtility.Instance.RegistSe(SceneSoundNames.CREATURE_HIT, se[(int)SE.CreatureHit]);
        SoundUtility.Instance.RegistSe(SceneSoundNames.CREATURE_CHARGE, se[(int)SE.CreatureCharge]);
    }
}
