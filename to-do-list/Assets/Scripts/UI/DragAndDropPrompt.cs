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

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPosition = _rectTransform.anchoredPosition;
            _canvasGroup.alpha = 0.6f;
            _canvasGroup.blocksRaycasts = false;
            _originalParent = _rectTransform.parent;
            _rectTransform.SetParent(_originalParent.parent.root);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / GetCanvasScaleFactor();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            
            if (!IsPointerOverDropZone(eventData))
            {
                _rectTransform.SetParent(_originalParent);
                _rectTransform.anchoredPosition = _startPosition;
            }
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