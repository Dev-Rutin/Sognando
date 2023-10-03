using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CarouselMenu : MonoBehaviour
{
    [SerializeField] private GameObject scrollbar;
    [SerializeField] private GameObject selectButton;
    [SerializeField] HorizontalLayoutGroup layout;
    private float _scrollPos = 0;
    private bool _selectedBtn = false;
    private float[] _pos;
    private Scrollbar _scrollbar;
    private List<RectTransform> _buttons;
    private RectTransform _layoutRect;
    
    void Start()
    {
        _scrollbar = scrollbar.GetComponent<Scrollbar>();
        _buttons = new List<RectTransform>();
        _layoutRect = layout.GetComponent<RectTransform>();
        int i = 0;
        foreach (RectTransform child in _layoutRect)
        {
            if (!child.gameObject.activeInHierarchy) continue;

            int index = i;
            Button button = child.GetComponent<Button>();
            button?.onClick.AddListener(() => { SelectButton(index); });
            i++;
            _buttons.Add(child);
        }
    }

    void Update()
    {
        _pos = new float[transform.childCount];
        float distacne = 1f / (_pos.Length - 1);
        for(int i = 0; i< _pos.Length; i++)
        {
            _pos[i] = distacne * i;
        }

        if (Input.GetMouseButton(0))
        {
            _scrollPos = _scrollbar.value;
        }
        else
        {
            if (!_selectedBtn)
            {
                for (int i = 0; i < _pos.Length; i++)
                {
                    if (_scrollPos < _pos[i] + (distacne / 2) && _scrollPos > _pos[i] - (distacne / 2))
                    {
                        _scrollbar.value = Mathf.Lerp(_scrollbar.value, _pos[i], 0.1f);
                    }
                }
            }
        }

        for (int i = 0; i < _pos.Length; i++)
        {
            if (_scrollPos < _pos[i] + (distacne / 2) && _scrollPos > _pos[i] - (distacne / 2))
            {
                transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1f, 1f), 0.1f);
                for(int j = 0; j < _pos.Length; j++)
                {
                    if(j != i)
                    {
                        transform.GetChild(j).localScale = Vector2.Lerp(transform.GetChild(j).localScale, new Vector2(0.8f, 0.8f), 0.1f);
                    }
                }

                /*selectButton.transform.GetChild(i).localScale = Vector2.Lerp(selectButton.transform.GetChild(i).localScale, new Vector2(1f, 1f), 0.1f);
                for (int k = 0; k < selectButton.transform.childCount; k++)
                {
                    if (k != i)
                        selectButton.transform.GetChild(k).localScale = Vector2.Lerp(selectButton.transform.GetChild(k).localScale, new Vector2(0.7f, 0.7f), 0.1f);
                }*/
            }
        }    
    }
    
    private void SelectButton(float index)
    {
        Debug.Log("SelectBtn");
        _selectedBtn = true;
        while (true)
        {
            
            _scrollbar.value = Mathf.Lerp(_scrollbar.value, index, 0.1f);
            if (Mathf.Abs(_scrollbar.value - index) <= 0.1f)
            {
                _scrollPos = _scrollbar.value;
                _selectedBtn = false;
                break;
            }
        }
    }
}