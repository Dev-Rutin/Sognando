using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase4Controller : MonoBehaviour
{
    [SerializeField] private float _fadeTime;
    [SerializeField] private float _windowLightAlpha;
    [SerializeField] private GameObject _windowLight;
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
        foreach (var child in _childs)
        {
            Debug.Log($"name : {child.name}");
            FadeUtlity.Instance.CallFade(_fadeTime, child.gameObject, EGameObjectType.GameObject, EFadeType.FadeOut);
            yield return new WaitForSeconds(_fadeTime + 0.5f);
        }
        
        _windowLight.GetComponent<WindowLightController>().FadeStart(_windowLightAlpha);
    }
}
