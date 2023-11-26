using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour
{
    [SerializeField] private Transform[] _HPGauge;
    [SerializeField] private float _animationSpeed;
    [SerializeField] private Button _button;
    private float _distance;

    private int _HPindex = 1;
    // Start is called before the first frame update
    void Start()
    {
        _HPGauge = GetComponentsInChildren<Transform>();
        Debug.Log(_HPGauge.Length);
        _distance = 0.106f;
        _button.onClick.AddListener(AnimationStart);
    }
    
    public void AnimationStart()
    {
        StartCoroutine(Damage());
    }

    private IEnumerator Damage()
    {
        Vector3 target = _HPGauge[_HPindex].position;
        Vector3 origin;
        Vector3 start = target;
        target.x -= _distance;
        float time = 0;
        while (true)
        {
            time += Time.deltaTime;
            origin = Vector3.Lerp(start, target, time / _animationSpeed);
            _HPGauge[_HPindex].position = origin;
            if (origin == target)
            {
                break;
            }

            yield return null;
        }
        Debug.Log("Start for()");
        for (int i = _HPindex + 1; i < _HPGauge.Length; i++)
        {
            _HPGauge[i].position = _HPGauge[_HPindex].position;
        }
        ++_HPindex;
    }
}
