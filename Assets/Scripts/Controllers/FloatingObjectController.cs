using System.Collections;
using UnityEngine;

public class FloatingObjectController : MonoBehaviour
{
    [SerializeField] private float _floatDistance;
    [SerializeField] private float _moveTime;
    private Vector3 _originPosition;
    private float _timer;
    
    // Start is called before the first frame update
    void Start()
    {
        _originPosition = gameObject.transform.localPosition;
        if (gameObject.CompareTag("FloatingObject"))
        {
            StartCoroutine(StartFloatingY());
        }
        else if (gameObject.CompareTag("Cloud"))
        {
            StartCoroutine(StartFloatingX());
        }
        
    }


    private IEnumerator StartFloatingY()
    {
        
        Vector3 movePosition = _originPosition;
        while (true)
        {
            _timer = 0;
            while (gameObject.transform.localPosition.y < _originPosition.y + _floatDistance)
            {
                _timer += Time.deltaTime;
                movePosition.y = Mathf.Lerp(gameObject.transform.localPosition.y, _originPosition.y + _floatDistance, _timer / _moveTime);
                if (movePosition.y > _originPosition.y && Mathf.Abs(_originPosition.y + _floatDistance - movePosition.y) < 0.3f)
                {
                    movePosition.y = _originPosition.y + _floatDistance;
                    gameObject.transform.localPosition = movePosition;
                    break;
                }
                gameObject.transform.localPosition = movePosition;
                yield return null;
            }
            
            _timer = 0;
            while (gameObject.transform.localPosition.y > _originPosition.y - _floatDistance)
            {
                _timer += Time.deltaTime;
                movePosition.y = Mathf.Lerp(gameObject.transform.localPosition.y, _originPosition.y - _floatDistance, _timer / _moveTime);
                if (movePosition.y < _originPosition.y && Mathf.Abs(_originPosition.y - _floatDistance - movePosition.y) < 0.3f)
                {
                    movePosition.y = _originPosition.y - _floatDistance;
                    gameObject.transform.localPosition = movePosition;
                    break;
                }
                gameObject.transform.localPosition = movePosition;
                yield return null;
            }

            yield return null;
        }
    }
    
    private IEnumerator StartFloatingX()
    {
        
        Vector3 movePosition = _originPosition;
        while (true)
        {
            _timer = 0;
            while (gameObject.transform.localPosition.x < _originPosition.x + _floatDistance)
            {
                _timer += Time.deltaTime;
                movePosition.x = Mathf.Lerp(gameObject.transform.localPosition.x, _originPosition.x + _floatDistance, _timer / _moveTime);
                if (movePosition.x > _originPosition.x && Mathf.Abs(_originPosition.x + _floatDistance - movePosition.x) < 0.003f)
                {
                    movePosition.x = _originPosition.x + _floatDistance;
                    gameObject.transform.localPosition = movePosition;
                    break;
                }
                gameObject.transform.localPosition = movePosition;
                yield return null;
            }
            yield return new WaitForSeconds(Random.Range(0.0f, 3.0f));
            _timer = 0;
            while (gameObject.transform.localPosition.x > _originPosition.x - _floatDistance)
            {
                _timer += Time.deltaTime;
                movePosition.x = Mathf.Lerp(gameObject.transform.localPosition.x, _originPosition.x - _floatDistance, _timer / _moveTime);
                if (movePosition.x < _originPosition.x && Mathf.Abs(_originPosition.x - _floatDistance - movePosition.x) < 0.003f)
                {
                    movePosition.x = _originPosition.x - _floatDistance;
                    gameObject.transform.localPosition = movePosition;
                    break;
                }
                gameObject.transform.localPosition = movePosition;
                yield return null;
            }
            yield return null;
        }
    }
}
