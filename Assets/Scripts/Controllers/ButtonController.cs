using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, ISelectHandler
{
    [SerializeField] private GameObject selectArrow;

    public void OnSelect(BaseEventData eventData)
    {
        Vector3 target = selectArrow.transform.position;
        target.y = transform.position.y;
        selectArrow.transform.position = target;
    }
}
