using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MoonController : MonoBehaviour
{
    [SerializeField] private Vector2[] _stage;
    [SerializeField] private float _moveTime;

    private Vector3 _originPosition;
    public bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void MoveToLevel(int stage)
    {
        StartCoroutine(MoveStart(stage - 1));
    }

    private IEnumerator MoveStart(int index)
    {
        _originPosition = gameObject.transform.localPosition;
        Vector3 movePosition = _originPosition;
        Vector3 destination = _stage[index];
        isMoving = true;
        float timer = 0;
        while (isMoving)
        {
            timer += Time.deltaTime;
            movePosition.y = Mathf.Lerp(_originPosition.y, destination.y, timer / _moveTime);
            movePosition.x = Mathf.Lerp(_originPosition.x, destination.x, timer / _moveTime);
            if (Mathf.Abs(Mathf.Abs(movePosition.x) - Mathf.Abs(destination.x)) < 0.003f && Mathf.Abs(Mathf.Abs(movePosition.y) - Mathf.Abs(destination.y)) < 0.003f)
            {
                movePosition = destination;
                isMoving = false;
            }

            gameObject.transform.localPosition = movePosition;
            yield return null;
        }
        
    }
}
