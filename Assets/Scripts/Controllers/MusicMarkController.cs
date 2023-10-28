using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicMarkController : MonoBehaviour
{
    private SpriteRenderer[] _musicMarks;

    [SerializeField] private float _fadeTime;
    [SerializeField] private float _moveDistance;
    [SerializeField] private float _moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        _musicMarks = gameObject.GetComponentsInChildren<SpriteRenderer>();
    }

    public void MarkStart()
    {
        StartCoroutine(MarkUp());
    }

    private IEnumerator MarkUp()
    {
        while (true)
        {
            int index = Random.Range(0, 3);
            if (_musicMarks[index].color.a <= 0)
            {
                StartCoroutine(MoveAndFade(index));
                
            }
            yield return new WaitForSeconds(Random.Range(0.3f, 0.8f));
        }
    }

    private IEnumerator MoveAndFade(int index)
    {
        Vector3 markPosition = _musicMarks[index].transform.position;
        float firstPositionY = markPosition.y;
        float destination = markPosition.y + _moveDistance;
        float timer = 0;
        
        FadeUtlity.Instance.CallFade(_fadeTime, _musicMarks[index].gameObject, EGameObjectType.GameObject, EFadeType.FadeOut);
        while (_musicMarks[index].color.a < 1)
        {
            timer += Time.deltaTime;
            markPosition.y = Mathf.Lerp(firstPositionY, destination, timer / _moveSpeed);
            _musicMarks[index].transform.position = markPosition;
            yield return null;
        }
        
        FadeUtlity.Instance.CallFade(_fadeTime, _musicMarks[index].gameObject, EGameObjectType.GameObject, EFadeType.FadeIn);
        while (_musicMarks[index].color.a > 0)
        {
            yield return null;
        }

        markPosition.y = firstPositionY;
        _musicMarks[index].transform.position = markPosition;
    }
}
