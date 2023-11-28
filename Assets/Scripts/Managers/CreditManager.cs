using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditManager : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private GameObject _fade;
    [SerializeField] private float _fadeTime;
    
    [Header("Credit")]
    [SerializeField] private GameObject _credit;
    [SerializeField] private float _moveSpeed;
    private float _speedUp;
    private float _originSpeed;
    
    private float _targetY = 3274f;
    // Start is called before the first frame update
    void Start()
    {
        FadeUtlity.Instance.CallFade(_fadeTime, _fade, EGameObjectType.UI, EFadeType.FadeIn);
        _originSpeed = _moveSpeed;
        _speedUp = _moveSpeed / 2;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _moveSpeed = _speedUp;
        }
        else
        {
            _moveSpeed = _originSpeed;
        }
    }

    private IEnumerator StartMove()
    {
        Vector3 movePos = _credit.transform.position;
        Vector3 startPos = movePos;
        Vector3 targetPos = movePos;
        targetPos.y = _targetY;
        float moveTime = 0;
        while (true)
        {
            moveTime += Time.deltaTime;
            movePos = Vector3.Lerp(startPos, targetPos, moveTime / _moveSpeed);
            _credit.transform.position = movePos;
            if (movePos == targetPos)
            {
                break;
            }

            yield return null;
        }
    }
}
