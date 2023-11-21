using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase3Controller : MonoBehaviour
{
    [SerializeField] private float _fadeTime;
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
        _moon.gameObject.GetComponent<MoonController>().MoveToLevel(3);
        foreach (var child in _childs)
        {
            Debug.Log($"name : {child.name}");
            FadeUtlity.Instance.CallFade(_fadeTime, child.gameObject, EGameObjectType.GameObject, EFadeType.FadeOut);
            yield return new WaitForSeconds(_fadeTime + 0.5f);
        }
        
        _windowLight.GetComponent<WindowLightController>().FadeStart(_windowLightAlpha);
    }
}
