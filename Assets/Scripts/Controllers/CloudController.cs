using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    private SpriteRenderer[] _clouds;

    [SerializeField] private float _fadeTime;
    [SerializeField] private float _moveDistance;
    [SerializeField] private float _moveTime;
    // Start is called before the first frame update
    void Start()
    {
        _clouds = gameObject.GetComponentsInChildren<SpriteRenderer>();
    }

    public void CloudStart()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator CloudMove()
    {
        while (true)
        {
            int index = Random.Range(0, _clouds.Length);
            if (_clouds[index].color.a <= 0)
            {
                StartCoroutine(MoveAndFade(index));
                
            }
            yield return new WaitForSeconds(Random.Range(0.3f, 0.8f));
        }
    }

    private IEnumerator Fade()
    {
        foreach (var cloud in _clouds)
        {
            FadeUtlity.Instance.CallFade(_fadeTime, cloud.gameObject, EGameObjectType.GameObject, EFadeType.FadeOut);
            cloud.gameObject.GetComponent<FloatingObjectController>().enabled = true;
            yield return new WaitForSeconds(Random.Range(0.5f, 0.8f));
        }
    }

    private IEnumerator MoveAndFade(int index)
    {
        Vector3 markPosition = _clouds[index].transform.position;
        float firstPositionX = markPosition.x;
        float destination = Random.Range(0, 100) % 2 == 0 ? markPosition.x + _moveDistance : markPosition.x - _moveDistance;
        float timer = 0;
        
        FadeUtlity.Instance.CallFade(_fadeTime, _clouds[index].gameObject, EGameObjectType.GameObject, EFadeType.FadeOut);
        while (_clouds[index].color.a < 1)
        {
            timer += Time.deltaTime;
            markPosition.x = Mathf.Lerp(firstPositionX, destination, timer / _moveTime);
            _clouds[index].transform.position = markPosition;
            yield return null;
        }
        
        FadeUtlity.Instance.CallFade(_fadeTime, _clouds[index].gameObject, EGameObjectType.GameObject, EFadeType.FadeIn);
        while (_clouds[index].color.a > 0)
        {
            yield return null;
        }

        markPosition.x = firstPositionX;
        _clouds[index].transform.position = markPosition;
    }
}
