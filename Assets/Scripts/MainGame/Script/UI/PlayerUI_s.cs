using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI_s : Singleton<PlayerUI_s>
{
    [Header("HP")]
    private List<Image> _hpImages;
    [SerializeField] private Sprite _hpOn;
    [SerializeField] private Sprite _hpOff;
    [SerializeField] private Transform _hpUITsf;

    [Header("Effect")]
    [SerializeField] private ParticleSystem _playerHealEffect;
    [SerializeField] private ParticleSystem _playerHurtEffect;

    [Header("Attack")]
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
    private WaitForEndOfFrame _waitUpdate;
    private void Start()
    {
        _hpImages = new List<Image>();
        foreach(Transform data in _hpUITsf)
        {
            _hpImages.Add(data.GetComponent<Image>());
        }
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
        for (int i = 0; i < _hpImages.Count;i++)
        {
            if(i<curPlayerHP)
            {
                _hpImages[i].sprite = _hpOn;
            }
            else
            {
                _hpImages[i].sprite = _hpOff;
            }
        }
    }
    public void AttackChange(EPlayerAttackLevel level)
    {
        switch (level)
        {
            case EPlayerAttackLevel.ONE:
                _attackLevel1.Play();
                _curAttackParticle = _attackLevel1;
                break;
            case EPlayerAttackLevel.TWO:
                _attackIncrease.Play();
                _attackLevel1.Stop();
                _attackLevel2.Play();
                _curAttackParticle = _attackLevel2;
                break;
            case EPlayerAttackLevel.THREE:
                _attackIncrease.Play();
                _attackLevel2.Stop();
                _attackLevel3.Play();
                _curAttackParticle = _attackLevel3;
                break;
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
        float startPosition = InGameMusicManager_s.Instance.musicPosition;
        float curTimeCount = 0;
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
            lerpValue = (curTimeCount/(_attackAnimationTime-2.4f));
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
        _attackTrail.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        _curAttackParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
       InGameEnemy_s.Instance.UpdateEnemyHP(-1);
        DoremiUI_s.Instance.SingleDoremiAnimation("idle", true);
        yield return new WaitForSeconds(1f);
        _attackTsf.localPosition = _playerAttackStartPos.localPosition;
        _curAttackParticle.Play();
    }
    
}
