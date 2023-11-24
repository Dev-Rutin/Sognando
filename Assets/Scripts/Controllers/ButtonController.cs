using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, ISelectHandler
{
    [SerializeField] private GameObject _cursor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelect(BaseEventData eventData)
    {
        Vector3 target = _cursor.transform.position;
        target.y = transform.position.y;
        _cursor.transform.position = target;
    }
}
