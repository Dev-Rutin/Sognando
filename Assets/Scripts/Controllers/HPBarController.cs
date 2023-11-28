using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour
{
    [SerializeField] private Transform[] _HPGauge;
    [SerializeField] private float _animationSpeed;
    private float _distance;

    private int _HPindex = 1;
    // Start is called before the first frame update
    void Start()
    {
        _HPGauge = GetComponentsInChildren<Transform>();
        _distance = 30f;
    }
    
    public void PlayerDamage()
    {
        StartCoroutine(Damage());
    }

    public void MonsterDamage(int count)
    {
        StartCoroutine(Damage(count));
    }

    private IEnumerator Damage()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector3 target = _HPGauge[_HPindex].localPosition;
            Vector3 origin;
            Vector3 start = target;
            target.x -= _distance;
            float time = 0;
            while (true)
            {
                time += Time.deltaTime;
                origin = Vector3.Lerp(start, target, time / _animationSpeed);
                _HPGauge[_HPindex].localPosition = origin;
                if (origin == target)
                {
                    break;
                }

                yield return null;
            }
            Debug.Log("Start for()");
            for (int j = _HPindex + 1; j < _HPGauge.Length; j++)
            {
                _HPGauge[j].localPosition = _HPGauge[_HPindex].localPosition;
            }
            ++_HPindex;
        }
    }
    
    private IEnumerator Damage(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 target = _HPGauge[_HPindex].localPosition;
            Vector3 origin;
            Vector3 start = target;
            target.x -= _distance;
            float time = 0;
            while (true)
            {
                time += Time.deltaTime;
                origin = Vector3.Lerp(start, target, time / _animationSpeed);
                _HPGauge[_HPindex].localPosition = origin;
                if (origin == target)
                {
                    break;
                }

                yield return null;
            }
            Debug.Log("Start for()");
            for (int j = _HPindex + 1; j < _HPGauge.Length; j++)
            {
                _HPGauge[j].localPosition = _HPGauge[_HPindex].localPosition;
            }
            ++_HPindex;
        }
    }
}
