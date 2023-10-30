using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonController : MonoBehaviour
{
    [SerializeField] private Vector2[] _stage;

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
        yield return null;
    }
}
