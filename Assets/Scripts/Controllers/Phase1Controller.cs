using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase1Controller : MonoBehaviour
{
    [SerializeField] private float _fadeTime;
    [SerializeField] private float _musicMarkWaitTime;
    [SerializeField] private float _windowLightAlpha;
    [SerializeField] private GameObject _classroom;
    [SerializeField] private GameObject _windowLight;
    [SerializeField] private GameObject _musicMark;
    [SerializeField] private GameObject _cloud;
    private SpriteRenderer[] _childs;
    // Start is called before the first frame update
    void Start()
    {
        _childs = _classroom.GetComponentsInChildren<SpriteRenderer>();
        
    }

    public void PhaseStart()
    {
        StartCoroutine(PhaseStartCorutine());
    }

    private IEnumerator PhaseStartCorutine()
    {
        foreach (var child in _childs)
        {
            if (child.name == "WindowLight")
            {
                Debug.Log("WindowLight");
                continue;
            }
            Debug.Log($"name : {child.name}");
            FadeUtlity.Instance.CallFade(_fadeTime, child.gameObject, EGameObjectType.GameObject, EFadeType.FadeOut);
            yield return new WaitForSeconds(_fadeTime + 0.5f);
        }
        _windowLight.GetComponent<WindowLightController>().FadeStart(_windowLightAlpha);

        yield return new WaitForSeconds(_musicMarkWaitTime);

        _cloud.GetComponent<CloudController>().CloudStart();
        
        _musicMark.GetComponent<MusicMarkController>().MarkStart();
    }
}
