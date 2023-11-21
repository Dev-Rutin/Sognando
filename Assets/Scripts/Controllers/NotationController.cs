using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotationController : MonoBehaviour
{
    [SerializeField] private float _fadeTime;
    [SerializeField] private float _fadeWaitTime;
    private SpriteRenderer[] _childs;
    // Start is called before the first frame update
    void Start()
    {
        _childs = gameObject.GetComponentsInChildren<SpriteRenderer>();
    }

    public void StartNotation()
    {
        StartCoroutine(StartAnim());
    }

    private IEnumerator StartAnim()
    {
        foreach (var child in _childs)
        {
            StartCoroutine(FadeInAndOut(child.gameObject));
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator FadeInAndOut(GameObject child)
    {
        while (true)
        {
            FadeUtlity.Instance.CallFade(_fadeTime, child, EGameObjectType.GameObject, EFadeType.FadeOut);
            while (child.GetComponent<SpriteRenderer>().color.a < 1)
            {
                yield return null;
            }

            yield return new WaitForSeconds(_fadeWaitTime);
            
            FadeUtlity.Instance.CallFade(_fadeTime, child, EGameObjectType.GameObject, EFadeType.FadeIn);
            while (child.GetComponent<SpriteRenderer>().color.a > 0)
            {
                yield return null;
            }
        }
    }
}
