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

}
