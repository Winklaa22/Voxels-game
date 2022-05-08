using System;
using Management.Cursor;
using Management.UI;
using Types._Cursor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Equipment.Item
{
    public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
        IPointerUpHandler
    {
        [SerializeField] private bool _isGrabbed;

        private void Update()
        {
            if (_isGrabbed)
            {
                transform.position = CursorManager.Instance.CursorPosition();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            CursorManager.Instance.SetCursor(CursorName.Grab);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CursorManager.Instance.SetCursor(CursorName.Normal);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.parent = UIManager.Instance.MainCanvas;
            _isGrabbed = true;
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            _isGrabbed = false;
        }
    }
}
