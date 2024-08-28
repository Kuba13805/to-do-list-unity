using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class DragAndDropPrompt : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Variables

        private RectTransform _rectTransform;
        private Transform _originalParent;
        private CanvasGroup _canvasGroup;
        private Vector2 _startPosition;

        #endregion
        
        public static event Action OnDragStart;
        public static event Action OnDragEnd;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            OnDragStart += SwitchRaycastBlocks;
            OnDragEnd += SwitchRaycastBlocks;
        }

        private void OnDestroy()
        {
            OnDragStart -= SwitchRaycastBlocks;
            OnDragEnd -= SwitchRaycastBlocks;
        }

        private void SwitchRaycastBlocks()
        {
            _canvasGroup.blocksRaycasts = !_canvasGroup.blocksRaycasts;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPosition = _rectTransform.anchoredPosition;
            _canvasGroup.alpha = 0.6f;
            _originalParent = _rectTransform.parent;
            _rectTransform.SetParent(_originalParent.parent.root);
            OnDragStart?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / GetCanvasScaleFactor();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.alpha = 1f;
            
            if (!IsPointerOverDropZone(eventData))
            {
                _rectTransform.SetParent(_originalParent);
                _rectTransform.anchoredPosition = _startPosition;
            }
            OnDragEnd?.Invoke();
        }
        private static bool IsPointerOverDropZone(PointerEventData eventData)
        {
            return eventData.pointerEnter != null && eventData.pointerEnter.GetComponent<QuestDropZone>() != null;
        }
        
        private float GetCanvasScaleFactor()
        {
            var canvas = GetComponentInParent<Canvas>();
            return canvas != null ? canvas.scaleFactor : 1f;
        }
    }
}