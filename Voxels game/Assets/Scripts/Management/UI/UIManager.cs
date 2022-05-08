using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Management.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [SerializeField] private RectTransform _mainCanvas;
        public RectTransform MainCanvas => _mainCanvas;

        [Header("Screens")] 
        [SerializeField] private ScreenUI[] _screens;

        [Header("Rendering chunks")] 
        [SerializeField] private TextMeshProUGUI _generationCounter;
        [SerializeField] private Image _generationBar;

        [Header("Inventory")]
        [SerializeField] private RectTransform[] _rectSlots;
        [SerializeField] private RawImage[] _slotsImages = new RawImage[6];

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SetScreen(ScreenType.RENDER);
        }

        public void SetScreen(ScreenType type)
        {
            foreach (var screen in _screens)
            {
                var active = screen.Type.Equals(type);
                
                screen.SetActive(active);
            }
        }

        public void SetSlot(int value)
        {
            for (var i = 0; i < _rectSlots.Length; i++)
            {
                var sequence = DOTween.Sequence();
                var scaleValue = i != value ? 1 : 1.5f;

                sequence.Append(_rectSlots[i].DOScale(scaleValue, .4f));
            }
        }

        public void SetRenderBarValue(float value)
        {
            _generationCounter.text = Mathf.FloorToInt(value * 100) + "%";
            _generationBar.fillAmount = value;
        }


        public void SetInventoryImageActive(int index, Texture2D image)
        {
            _slotsImages[index].gameObject.SetActive(image != null);
            _slotsImages[index].texture = image;
        }
    }
}