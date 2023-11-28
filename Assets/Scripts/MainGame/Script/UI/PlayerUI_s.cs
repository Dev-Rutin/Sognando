using FMODPlus;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI_s : Singleton<PlayerUI_s>
{
    [Header("HP")]
    [SerializeField] private HPBarController _hpController;

    [Header("Effect")]
    //[SerializeField] private ParticleSystem _playerHealEffect;
    //[SerializeField] private ParticleSystem _playerHurtEffect;

    [Header("Attack Particle")]
    [SerializeField] private Transform _attackTsf;
    [SerializeField] private ParticleSystem _attackLevel1;
    [SerializeField] private ParticleSystem _attackLevel2;
    [SerializeField] private ParticleSystem _attackLevel3;
    [SerializeField] private ParticleSystem _attackIncrease;
    [SerializeField] private ParticleSystem _attackTrail;
    [SerializeField] private Transform _playerAttackStartPos;
    [SerializeField] private Transform _playerAttackMiddlePos;
    [SerializeField] private Transform _playerAttackEndPos;
    [SerializeField] float _attackAnimationTime;
    [SerializeField] private ParticleSystem _curAttackParticle;
    [Header("Attack UI")]
    [SerializeField] private GameObject _attackGuagesObj;
    [SerializeField] private UnityEngine.UI.Image[] _attackGuageUIs;
    [SerializeField] private Sprite AttackGaugeOn;
    [SerializeField] private Sprite AttackGaugeOff;
    [SerializeField] private ParticleSystem[] _attackGuageParticles;

    [Header("Attack Data")]
    [SerializeField] private int _level1DMG;
    [SerializeField] private int _level2DMG;
    [SerializeField] private int _level3DMG;
    private int _curDMG;
    private int _curAttackLevel;

    private WaitForEndOfFrame _waitUpdate;

    [Header("Animation")]
    [SerializeField] private SkeletonAnimation _animation;

    [Header("Sound")]
    [SerializeField] private CommandSender _hitSound;
    [SerializeField] private CommandSender _attackSound;
    [SerializeField] private CommandSender _magicCircleSound;

    private void Start()
    {
        _curAttackParticle = null;
        _waitUpdate = new WaitForEndOfFrame();
        InGameFunBind_s.Instance.Epause += Pause;
        InGameFunBind_s.Instance.EunPause += UnPause;
    }
    public void SinglePlayerAnimation(string name, bool isLoop)
    {
        _animation.AnimationState.SetAnimation(0, name, isLoop);
    }
    public void MutiplePlayerAnimation(List<string> data)
    {
        Animation.ShowCharacterAnimation(data, _animation);
    }
    private void Pause()
    {
        _animation.timeScale = 0;
        /* foreach (ParticleSystem p in _playerHealEffect.GetComponentsInChildren<ParticleSystem>())
         {
             var main = p.main;
             main.simulationSpeed = 0;
         }
         foreach (ParticleSystem p in _playerHurtEffect.GetComponentsInChildren<ParticleSystem>())
         {
             var main = p.main;
             main.simulationSpeed = 0;
         }*/
    }
    private void UnPause()
    {
        _animation.timeScale = 1;
        /* foreach (ParticleSystem p in _playerHealEffect.GetComponentsInChildren<ParticleSystem>())
         {
             var main = p.main;
             main.simulationSpeed = 1;
         }
         foreach (ParticleSystem p in _playerHurtEffect.GetComponentsInChildren<ParticleSystem>())
         {
             var main = p.main;
             main.simulationSpeed = 1;
         }*/
    }
    public void PlayerHPInitialize(int maxValue)
    {
    }
    public void PlayerHPUp()
    {
        //_playerHealEffect.Play();
    }
    public void PlayerHPDown()
    {
        // _playerHurtEffect.Play();
        _hitSound.SendCommand();
        MutiplePlayerAnimation(new List<string>() { "hit", "idle" });
    }
    public void PlayerHPUpdate(int curPlayerHP)
    {
        _hpController.PlayerDamage();
    }
    public void ShowAttackGuage(EPlayerAttackLevel level)
    {
        int count = 0;
        foreach(var data in _attackGuageUIs)
        {
            if(count<(int)level)
            {
                _attackGuageUIs[count].sprite = AttackGaugeOn;
            }
            else
            {
                _attackGuageUIs[count].sprite = AttackGaugeOff;
            }
            count++;
        }
        _attackGuageParticles[(int)level - 1].Play();
    }
    public void ShowAttackParticle(EPlayerAttackLevel level)
    {
        _magicCircleSound.SendCommand();
        _attackIncrease.Play();
        _attackGuagesObj.SetActive(false);
        switch (level)
        {
            case EPlayerAttackLevel.NONE:
                break;
            case EPlayerAttackLevel.ONE:
                _attackLevel1.Play();
                _curAttackParticle = _attackLevel1;
                _curDMG = _level1DMG;
                break;
            case EPlayerAttackLevel.TWO:             
                _attackLevel2.Play();
                _curAttackParticle = _attackLevel2;
                _curDMG = _level2DMG;
                break;
            case EPlayerAttackLevel.THREE:
                _attackLevel3.Play();
                _curAttackParticle = _attackLevel3;
                _curDMG = _level3DMG;
                break;
            default:
                break;
        }
        _curAttackLevel = (int)level;
    }
    public void StopAttackParticle()
    {
        if(_curAttackParticle!=null)
        {
            _curAttackParticle.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
    public void PlayerAttack()
    {
        StartCoroutine(IAttack());      
    }
    private IEnumerator IAttack()
    {
        PlayerUI_s.Instance.SinglePlayerAnimation("attack", false);
        DoremiUI_s.Instance.SingleDoremiAnimation("attack", false);
        _attackTrail.gameObject.SetActive(true);
        _attackTrail.Play();

        double startPosition = InGameMusicManager_s.Instance.musicPosition;
        double curTimeCount = 0;
        while(curTimeCount<=2.2f)
        {
            curTimeCount = InGameMusicManager_s.Instance.musicPosition - startPosition;
            yield return _waitUpdate;
        }
        startPosition = InGameMusicManager_s.Instance.musicPosition;
        curTimeCount = 0;
        float lerpValue = 0;
        while (curTimeCount<=_attackAnimationTime-2.2f)
        {
            lerpValue = (float)(curTimeCount/(_attackAnimationTime-2.2f));
            if (lerpValue <= 0.5f)
            {
                _attackTsf.localPosition = Vector3.Slerp(_playerAttackStartPos.localPosition, _playerAttackMiddlePos.localPosition, lerpValue/0.5f);
            }
            else
            {
                _attackTsf.localPosition = Vector3.Lerp(_playerAttackMiddlePos.localPosition, _playerAttackEndPos.localPosition, (lerpValue-0.5f)/0.5f);
            }
            curTimeCount = InGameMusicManager_s.Instance.musicPosition - startPosition;
            yield return _waitUpdate;
        }
        _attackSound.SendCommand();
        InGameEnemy_s.Instance.UpdateEnemyHP(_curDMG * -1,_curAttackLevel);
        _attackTrail.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        _attackTrail.gameObject.SetActive(false);
        _curAttackParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        PlayerUI_s.Instance.SinglePlayerAnimation("idle", true);
        DoremiUI_s.Instance.SingleDoremiAnimation("idle", true);
        yield return new WaitForSeconds(1f);
        _attackTsf.localPosition = _playerAttackStartPos.localPosition;
        _attackGuagesObj.SetActive(true);
    }
    
}
