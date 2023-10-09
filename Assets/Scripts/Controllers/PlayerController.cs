using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum EAnimName
    {
        Walk,
        Medicine,
        WalkToMedicine,
        Gotobed
    }
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _animSpeed;
    [SerializeField] private IntroManager _introManager;
    [SerializeField] private GameObject _medicinePoint;
    [SerializeField] private GameObject _bedPoint;

    private bool _isWalking;
    private bool _haaMedicine;
    private bool _startScene;
    private int _cnt = 0;
    private SkeletonAnimation _skeletonAnimation;
    
    // Start is called before the first frame update
    void Start()
    {
        _skeletonAnimation = gameObject.GetComponent<SkeletonAnimation>();
        _haaMedicine = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_introManager._IsPlayerFading && _cnt == 0)
        {
            _cnt++;
            StartCoroutine(StartAnim());
        }
    }

    private IEnumerator StartAnim()
    {
        StartCoroutine(WalkPlayer(1f));
        while (_isWalking)
        {
            yield return null;
        }
        ChangeAnim(EAnimName.WalkToMedicine);
        yield return new WaitForSeconds(1.0f);
        ChangeAnim(EAnimName.Medicine);
        yield return new WaitForSeconds(3.0f);
        ChangeAnim(EAnimName.Walk);
        while (_isWalking)
        {
            yield return null;
        }
        ChangeAnim(EAnimName.Gotobed);
    }

    private void ChangeAnim(EAnimName animName)
    {
        switch (animName)
        {
            case EAnimName.Walk:
                gameObject.transform.Rotate(0, 180f, 0);
                _skeletonAnimation.loop = true;
                _skeletonAnimation.timeScale = 0.85f;
                StartCoroutine(WalkPlayer(-1f));
                break;
            case EAnimName.Medicine:
                _skeletonAnimation.loop = false;
                _skeletonAnimation.AnimationName = "medicine";
                _skeletonAnimation.timeScale = 1.0f;
                break;
            case EAnimName.WalkToMedicine:
                _skeletonAnimation.loop = false;
                _skeletonAnimation.AnimationName = "walk-medicine";
                _skeletonAnimation.timeScale = 1.0f;
                break;
            case EAnimName.Gotobed:
                _skeletonAnimation.loop = false;
                _skeletonAnimation.AnimationName = "gotobed";
                _skeletonAnimation.timeScale = 1.0f;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(animName), animName, null);
        }
        
    }

    private IEnumerator WalkPlayer(float horizental)
    {
        _isWalking = true;
        Vector3 position = transform.position;
        if (horizental > 0)
        {
            while (gameObject.transform.position.x < _medicinePoint.transform.position.x)
            {
                _skeletonAnimation.AnimationName = "walk";
                position.x += horizental * _walkSpeed * Time.deltaTime;
                transform.position = position;
                yield return null;
            }
        }
        else
        {
            while (gameObject.transform.position.x > _bedPoint.transform.position.x)
            {
                _skeletonAnimation.AnimationName = "walk";
                position.x += horizental * _walkSpeed * Time.deltaTime;
                transform.position = position;
                yield return null;
            }
        }
        
        _isWalking = false;
    }
}
