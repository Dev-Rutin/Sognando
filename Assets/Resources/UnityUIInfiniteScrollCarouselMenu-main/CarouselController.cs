using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class CarouselController : MonoBehaviour/*, IInitializePotentialDragHandler, IEndDragHandler, IDragHandler*/
{
    //private const float MAX_VELOCITY = 13; //최대 드래그 속도
    private const float MIN_VELOCITY = 0.8f; //최소 드래그 속도
    private const float TIME_TO_CENTER = 0.5f; //버튼 클릭 시, 중앙으로 오는 시간
    private const float DECELERATION_TIME = 1.0f;// 감속에 걸리는 시간

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private HorizontalLayoutGroup layout;
    [SerializeField] private AnimationCurve centeringCurve;
    [SerializeField] private GameObject _settingCanvas;

    private int _leftMostIndex; // 가장 왼쪽 버튼
    private int _rightMostIndex; // 가장 오른쪽 버튼
    private int _centerIndex; // 중앙 버튼
    private int _selectedIndex; // 선택된 버튼
    [SerializeField] private float _currentPosition;
    private float _previousPosition;
    private float _initialPosition;
    private float _targetPosition;
    private float _moveTimer;
    private float _dragVelocity;
    private float _currentVelocity;
    private float _thumbnailWidth;
    private float _gap;
    private bool _scrollControlsDragging;
    private bool _isDragging;
    private bool _isCentering;
    private RectTransform _layoutRect;
    private RectTransform _selectedItem;
    private List<RectTransform> _items;
    private Vector2 _lastDragPosition;
    private Vector3 _normalScale;

    enum EButtonName
    {
        Missing = 5,
        Setting,
        Start,
        Continue,
        Archive
    }

    public void Start()
    {
        _layoutRect = layout.GetComponent<RectTransform>();
        _items = new List<RectTransform>();
        _normalScale = new Vector3(1f, 1f, 1f);
        int i = 0;
        foreach (RectTransform child in _layoutRect)
        {
            if (!child.gameObject.activeInHierarchy) continue;

            int index = i;
            Button button = child.GetComponent<Button>();
            button.onClick.AddListener(() => { OnSelectedItem(index); });
            i++;
            _items.Add(child);
        }

        _thumbnailWidth = _items[0].GetComponent<RectTransform>().rect.width;
        _gap = layout.spacing;
        _leftMostIndex = 0;
        _rightMostIndex = _items.Count - 1;
        _centerIndex = ((_items.Count) / 2) - 1;
        _targetPosition = (_items.Count % 2 == 0) ? _thumbnailWidth * 0.5f : 0;
        _currentPosition = _targetPosition;

        _scrollControlsDragging = _items.Count * (_thumbnailWidth + _gap) < viewport.rect.width;
        scrollRect.enabled = _scrollControlsDragging;
    }

    void Update()
    {
        //LayoutRebuilder.MarkLayoutForRebuild(layoutRect); // USE THIS IF YOU ARE SCALING THE ELEMENTES TO MAINTING THE RIGHT GAP

        if (!_scrollControlsDragging)
        {
            _layoutRect.anchoredPosition = Vector2.right * _currentPosition;
            if (_currentPosition + _items[_rightMostIndex].anchoredPosition.x + _items[_rightMostIndex].rect.xMin > _layoutRect.rect.width - _gap)
            {
                SwitchRightMost();
            }
            if (_currentPosition + _items[_leftMostIndex].anchoredPosition.x + _items[_leftMostIndex].rect.xMax < _gap)
            {
                SwitchLeftMost();
            }
        }

        if (_isCentering)
        {
            _moveTimer += Time.deltaTime;
            if (_scrollControlsDragging)
            {
                scrollRect.horizontalNormalizedPosition = Mathf.Lerp(_initialPosition, _targetPosition, centeringCurve.Evaluate(_moveTimer / TIME_TO_CENTER));
                if (_moveTimer > TIME_TO_CENTER)
                {
                    _isCentering = false;
                    scrollRect.enabled = true;
                }
            }
            else
            {
                if (_moveTimer < TIME_TO_CENTER)
                {
                    _currentPosition = Mathf.Lerp(_initialPosition, _targetPosition, centeringCurve.Evaluate(_moveTimer / TIME_TO_CENTER));
                }
                else
                {
                    _isCentering = false;
                }
            }
        }
        else if (!_scrollControlsDragging)
        {
            if (_isDragging)
            {
                _dragVelocity = (_currentPosition - _previousPosition) / Time.deltaTime;
                _previousPosition = _currentPosition;
            }
            else
            {
                _moveTimer += Time.deltaTime;
                if (_moveTimer < DECELERATION_TIME)
                {
                    _currentVelocity = Mathf.Lerp(_dragVelocity, 0, _moveTimer / DECELERATION_TIME);
                    _currentPosition += _currentVelocity;
                    if (Mathf.Abs(_currentVelocity) < MIN_VELOCITY)
                    {
                        _moveTimer = DECELERATION_TIME;
                    }
                }
            }
        }
    }

    public void Select(int index)
    {
        OnSelectedItem(index);
    }

    public void ScrollToSelected()
    {
        OnSelectedItem(_selectedIndex);
    }

    private void OnSelectedItem(int index)
    {
        if (_items.Count == 0)
        {
            return;
        }

        index = Mathf.Clamp(index, 0, _items.Count - 1);

        if (_selectedItem == _items[index])
        {
            switch (index)
            {
                case (int)EButtonName.Missing:
                    // something?
                    break;
                case (int)EButtonName.Setting:
                    // enable setting UI
                    _settingCanvas.SetActive(!_settingCanvas.activeSelf);
                    break;
                case (int)EButtonName.Continue:
                    // send data script
                    Debug.Log("Continue Button");
                    break;
                case (int)EButtonName.Start:
                    Debug.Log("Start Button");
                    break;
                case (int)EButtonName.Archive:
                    // enable Archive UI
                    break;
                default:
                    break;
            }
            
            return;
        }

        _selectedIndex = index;
        _selectedItem = _items[index];

        _isCentering = true;
        _moveTimer = 0f;
        if (_scrollControlsDragging)
        {
            _initialPosition = scrollRect.horizontalNormalizedPosition;
            _targetPosition = (float)index / (float)(_items.Count - 1);
            scrollRect.enabled = false;
        }
        else
        {
            _initialPosition = _currentPosition;
            _targetPosition = _layoutRect.rect.width * 0.5f - _selectedItem.anchoredPosition.x;// + 0.5f * thumbnailWidth;
        }

        float resize = 1.25f;
        foreach (var item in _items)
        {
            if (_selectedItem == item)
            {
                StartCoroutine(ResizeButton(item, _normalScale * resize));
                continue;
            }
            StartCoroutine(ResizeButton(item, _normalScale));
        }
    }

    private void SwitchLeftMost()
    {
        _items[_leftMostIndex].transform.SetAsLastSibling();
        float offset = _thumbnailWidth + _gap;
        _currentPosition += offset;
        _initialPosition += offset;
        _targetPosition += offset;
        _layoutRect.anchoredPosition = Vector2.right * _currentPosition;
        _rightMostIndex = _leftMostIndex;
        _leftMostIndex = (_leftMostIndex < _items.Count - 1) ? _leftMostIndex + 1 : 0;
        _centerIndex = (_centerIndex < _items.Count - 1) ? _centerIndex + 1 : 0;
    }

    private void SwitchRightMost()
    {
        _items[_rightMostIndex].transform.SetAsFirstSibling();
        float offset = _thumbnailWidth + _gap;
        _currentPosition -= offset;
        _initialPosition -= offset;
        _targetPosition -= offset;
        _layoutRect.anchoredPosition = Vector2.right * _currentPosition;
        _leftMostIndex = _rightMostIndex;
        _rightMostIndex = (_rightMostIndex > 0) ? _rightMostIndex - 1 : _items.Count - 1;
        _centerIndex = (_centerIndex > 0) ? _centerIndex - 1 : _items.Count - 1;
    }

    private IEnumerator ResizeButton(RectTransform item, Vector3 end)
    {
        while (item.localScale != end)
        {
            item.localScale = Vector3.Lerp(item.localScale, end, centeringCurve.Evaluate(_moveTimer / TIME_TO_CENTER));
            yield return null;
        }

    }

    /*public void OnBeginDrag(PointerEventData eventData)
    {
        if (scrollControlsDragging || isCentering || eventData.button != PointerEventData.InputButton.Left)
            return;

        isDragging = true;
        lastDragPosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, eventData.position, eventData.pressEventCamera, out lastDragPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (scrollControlsDragging || !isDragging || eventData.button != PointerEventData.InputButton.Left)
            return;

        isDragging = false;
        moveTimer = 0;
        dragVelocity = Mathf.Clamp(dragVelocity * Time.deltaTime, -MAX_VELOCITY, MAX_VELOCITY);
        currentVelocity = dragVelocity;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (scrollControlsDragging || !isDragging)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, eventData.position, eventData.pressEventCamera, out Vector2 dragPosition))
            return;

        Vector2 delta = dragPosition - lastDragPosition;
        lastDragPosition = dragPosition;

        currentPosition += delta.x;
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        previousPosition = currentPosition;
        dragVelocity = 0;
    }*/
}
