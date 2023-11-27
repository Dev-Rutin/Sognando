using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI_s : Singleton<PlayerUI_s>
{
    [Header("HP")]
    [SerializeField] private Slider _playerHPSlider;

    [Header("Effect")]
    [SerializeField] private ParticleSystem _playerHealEffect;
    [SerializeField] private ParticleSystem _playerHurtEffect;

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
    [SerializeField] private UnityEngine.UI.Image[] AttackGuageUIs;
    [SerializeField] private Sprite AttackGaugeOn;
    [SerializeField] private Sprite AttackGaugeOff;

    [Header("Attack Data")]
    [SerializeField] private int _level1DMG;
    [SerializeField] private int _level2DMG;
    [SerializeField] private int _level3DMG;
    private int _curDMG;
    private WaitForEndOfFrame _waitUpdate;
    private void Start()
    {
        _curAttackParticle = null;
        _waitUpdate = new WaitForEndOfFrame();
        InGameFunBind_s.Instance.Epause += Pause;
        InGameFunBind_s.Instance.EunPause += UnPause;
    }
    private void Pause()
    {
        foreach (ParticleSystem p in _playerHealEffect.GetComponentsInChildren<ParticleSystem>())
        {
            var main = p.main;
            main.simulationSpeed = 0;
        }
        foreach (ParticleSystem p in _playerHurtEffect.GetComponentsInChildren<ParticleSystem>())
        {
            var main = p.main;
            main.simulationSpeed = 0;
        }
    }
    private void UnPause()
    {
        foreach (ParticleSystem p in _playerHealEffect.GetComponentsInChildren<ParticleSystem>())
        {
            var main = p.main;
            main.simulationSpeed = 1;
        }
        foreach (ParticleSystem p in _playerHurtEffect.GetComponentsInChildren<ParticleSystem>())
        {
            var main = p.main;
            main.simulationSpeed = 1;
        }
    }
    public void PlayerHPInitialize(int maxValue)
    {
        _playerHPSlider.maxValue = maxValue;
    }
    public void PlayerHPUp()
    {
        _playerHealEffect.Play();
    }
    public void PlayerHPDown()
    {
        _playerHurtEffect.Play();
    }
    public void PlayerHPUpdate(int curPlayerHP)
    {
        _playerHPSlider.value = curPlayerHP;
    }
    public void ShowAttackGuage(EPlayerAttackLevel level)
    {
        int count = 0;
        foreach(var data in AttackGuageUIs)
        {
            if(count<(int)level)
            {
                AttackGuageUIs[count].sprite = AttackGaugeOn;
            }
            else
            {
                AttackGuageUIs[count].sprite = AttackGaugeOff;
            }
            count++;
        }
    }
    public void ShowAttackParticle(EPlayerAttackLevel level)
    {
        _attackIncrease.Play();
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
        DoremiUI_s.Instance.MutipleDoremiAnimation(new List<string>() { "attack", "attack2" });
        _attackTrail.Play();
        double startPosition = InGameMusicManager_s.Instance.musicPosition;
        double curTimeCount = 0;
        while(curTimeCount<=2.4f)
        {
            curTimeCount = InGameMusicManager_s.Instance.musicPosition - startPosition;
            yield return _waitUpdate;
        }
        startPosition = InGameMusicManager_s.Instance.musicPosition;
        curTimeCount = 0;
        float lerpValue = 0;
        while (curTimeCount<=_attackAnimationTime-2.4f)
        {
            lerpValue = (float)(curTimeCount/(_attackAnimationTime-2.4f));
            if (lerpValue <= 0.5f)
            {
                _attackTsf.localPosition = Vector3.Slerp(_playerAttackStartPos.localPosition, _playerAttackMiddlePos.localPosition, lerpValue/0.5f);
            }
            else
            {
                _attackTsf.localPosition = Vector3.Slerp(_playerAttackMiddlePos.localPosition, _playerAttackEndPos.localPosition, (lerpValue-0.5f)/0.5f);
            }
            curTimeCount = InGameMusicManager_s.Instance.musicPosition - startPosition;
            yield return _waitUpdate;
        }
        InGameEnemy_s.Instance.UpdateEnemyHP(_curDMG * -1);
        _attackTrail.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        _curAttackParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        InGamePlayer_s.Instance.UpdatePlayerHP(1);
        DoremiUI_s.Instance.SingleDoremiAnimation("idle", true);
        yield return new WaitForSeconds(1f);
        _attackTsf.localPosition = _playerAttackStartPos.localPosition;
    }
    
}
