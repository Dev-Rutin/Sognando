using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase5Controller : MonoBehaviour
{
    [SerializeField] private float _fadeTime;
    [SerializeField] private GameObject _notationLight;
    [SerializeField] private float _notationLightFadeTime;
    [SerializeField] private GameObject _notation;
    [SerializeField] private float _windowLightAlpha;
    [SerializeField] private GameObject _windowLight;
    [SerializeField] private GameObject _moon;
    
    private SpriteRenderer[] _childs;
    // Start is called before the first frame update
    void Start()
    {
        _childs = gameObject.GetComponentsInChildren<SpriteRenderer>();
        
    }

    public void PhaseStart()
    {
        StartCoroutine(PhaseStartCorutine());
    }

    private IEnumerator PhaseStartCorutine()
    {
        _moon.gameObject.GetComponent<MoonController>().isMoving = true;
        _moon.gameObject.GetComponent<MoonController>().MoveToLevel(5);
        for (int i = 0; i < 5; i++)
        {
            Debug.Log($"name : {_childs[i].name}");
            FadeUtlity.Instance.CallFade(_fadeTime, _childs[i].gameObject, EGameObjectType.GameObject, EFadeType.FadeOut);
            yield return new WaitForSeconds(_fadeTime + 0.5f);
        }
        StartCoroutine(NotationLightFade());

        _windowLight.GetComponent<WindowLightController>().FadeStart(_windowLightAlpha);
        
        yield return new WaitForSeconds(_fadeTime + 0.5f);
        
        _notation.GetComponent<NotationController>().StartNotation();
    }

    private IEnumerator NotationLightFade()
    {
        Color color = _notationLight.GetComponent<SpriteRenderer>().color;
        float timer;
        while (true)
        {
            timer = 0f;
            while (_notationLight.GetComponent<SpriteRenderer>().color.a < 0.4f)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(0f, 0.4f, timer / _notationLightFadeTime);
                _notationLight.GetComponent<SpriteRenderer>().color = color;
                yield return null;
            }
            
            timer = 0f;
            while (_notationLight.GetComponent<SpriteRenderer>().color.a > 0)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(0.4f, 0, timer / _notationLightFadeTime);
                _notationLight.GetComponent<SpriteRenderer>().color = color;
                yield return null;
            }
        }
        // ReSharper disable once IteratorNeverReturns
    }
}
