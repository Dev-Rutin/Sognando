using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    private float _targetY = 3586f;
    // Start is called before the first frame update
    void Start()
    {
        
        _originSpeed = _moveSpeed;
        _speedUp = _moveSpeed / 2;
        StartCredit();
    }

    // Update is called once per frame
    /*void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _moveSpeed = _speedUp;
        }
        else
        {
            _moveSpeed = _originSpeed;
        }
    }*/

    private void StartCredit()
    {
        StartCoroutine(StartMove());
    }

    private IEnumerator StartMove()
    {
        FadeUtlity.Instance.CallFade(_fadeTime, _fade, EGameObjectType.UI, EFadeType.FadeIn);
        while (_fade.GetComponent<CanvasGroup>().alpha > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        Vector3 movePos = _credit.transform.localPosition;
        Vector3 startPos = movePos;
        Vector3 targetPos = movePos;
        targetPos.y = _targetY;
        float moveTime = 0;
        while (true)
        {
            moveTime += Time.deltaTime;
            movePos = Vector3.Lerp(startPos, targetPos, moveTime / _moveSpeed);
            _credit.transform.localPosition = movePos;
            if (movePos == targetPos)
            {
                break;
            }

            yield return null;
        }
        
        yield return new WaitForSeconds(1.5f);
        FadeUtlity.Instance.CallFade(_fadeTime, _fade, EGameObjectType.UI, EFadeType.FadeOut);
        while (_fade.GetComponent<CanvasGroup>().alpha < 1)
        {
            yield return null;
        }

        SceneManager.LoadScene("LobbyScene");
    }
}
