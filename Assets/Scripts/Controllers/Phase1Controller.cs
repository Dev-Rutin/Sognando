using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
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
    [SerializeField] private GameObject _moon;
    [SerializeField] private GameObject _cubeAnimation;
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
                continue;
            }
            FadeUtlity.Instance.CallFade(_fadeTime, child.gameObject, EGameObjectType.GameObject, EFadeType.FadeOut);
            if (child.name == "Moon")
            {
                _moon.gameObject.GetComponent<MoonController>().isMoving = true;
                _moon.gameObject.GetComponent<MoonController>().MoveToLevel(1);
            }
            yield return new WaitForSeconds(_fadeTime + 0.5f);
        }
        _windowLight.GetComponent<WindowLightController>().FadeStart(_windowLightAlpha);
        

        yield return new WaitForSeconds(_musicMarkWaitTime);
        
        _cubeAnimation.SetActive(true);
        _cubeAnimation.GetComponent<SkeletonAnimation>().AnimationName = "cube_animation";
        
        _cloud.GetComponent<CloudController>().CloudStart();
        
        _musicMark.GetComponent<MusicMarkController>().MarkStart();
    }
}
