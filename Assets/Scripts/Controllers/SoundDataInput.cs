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
        SceneSoundManager.Instance.RegistBgm(SceneSoundNames.INTRO_BGM, bgm[(int)BGM.Start]);
        SceneSoundManager.Instance.RegistBgm(SceneSoundNames.LOBBY_BGM, bgm[(int)BGM.Lobby]);
        SceneSoundManager.Instance.RegistBgm(SceneSoundNames.INTRO_BGM, bgm[(int)BGM.Intro]);
        SceneSoundManager.Instance.RegistBgm(SceneSoundNames.LOBBY_BGM, bgm[(int)BGM.Story1]);
        SceneSoundManager.Instance.RegistBgm(SceneSoundNames.INTRO_BGM, bgm[(int)BGM.Story2]);
        SceneSoundManager.Instance.RegistBgm(SceneSoundNames.LOBBY_BGM, bgm[(int)BGM.Story3]);
        SceneSoundManager.Instance.RegistBgm(SceneSoundNames.INTRO_BGM, bgm[(int)BGM.Story4]);
        SceneSoundManager.Instance.RegistBgm(SceneSoundNames.INTRO_BGM, bgm[(int)BGM.Story5]);
        SceneSoundManager.Instance.RegistBgm(SceneSoundNames.LOBBY_BGM, bgm[(int)BGM.Stage1]);
        SceneSoundManager.Instance.RegistBgm(SceneSoundNames.INTRO_BGM, bgm[(int)BGM.Stage2]);
        SceneSoundManager.Instance.RegistBgm(SceneSoundNames.LOBBY_BGM, bgm[(int)BGM.Stage3]);
        SceneSoundManager.Instance.RegistBgm(SceneSoundNames.INTRO_BGM, bgm[(int)BGM.Stage4]);
        SceneSoundManager.Instance.RegistBgm(SceneSoundNames.INTRO_BGM, bgm[(int)BGM.Stage5]);

        SceneSoundManager.Instance.RegistSe(SceneSoundNames.UI_SELECT, se[(int)SE.UISelect]);
        SceneSoundManager.Instance.RegistSe(SceneSoundNames.UI_OPTION, se[(int)SE.UIOption]);
        SceneSoundManager.Instance.RegistSe(SceneSoundNames.UI_BACK, se[(int)SE.UIBack]);
        SceneSoundManager.Instance.RegistSe(SceneSoundNames.UI_NOTICE, se[(int)SE.UINotice]);
        SceneSoundManager.Instance.RegistSe(SceneSoundNames.UI_SELECT, se[(int)SE.Bit]);
        SceneSoundManager.Instance.RegistSe(SceneSoundNames.UI_OPTION, se[(int)SE.PlayerMove]);
        SceneSoundManager.Instance.RegistSe(SceneSoundNames.UI_BACK, se[(int)SE.PlayerAttack]);
        SceneSoundManager.Instance.RegistSe(SceneSoundNames.UI_NOTICE, se[(int)SE.PlayerHit]);
        SceneSoundManager.Instance.RegistSe(SceneSoundNames.UI_SELECT, se[(int)SE.PlayerCharge]);
        SceneSoundManager.Instance.RegistSe(SceneSoundNames.UI_OPTION, se[(int)SE.CreatureAttack]);
        SceneSoundManager.Instance.RegistSe(SceneSoundNames.UI_BACK, se[(int)SE.CreatureHit]);
        SceneSoundManager.Instance.RegistSe(SceneSoundNames.UI_NOTICE, se[(int)SE.CreatureCharge]);

    }
}
