using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UI.ProButton
{
    public class ProButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
    {
        [SerializeField] private Ease _ease;
        [SerializeField] private float _duration;
        [SerializeField] private float _pressSize;
        [SerializeField] private float _clickSize;
        [SerializeField] private UnityEvent _onClick;

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetScale(_pressSize);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            
        }

        private void SetScale(float size)
        {
            var sequence = DOTween.Sequence();
            
            sequence.Append(transform.DOScale(size, _duration)).SetEase(_ease);
        }
    }
}
