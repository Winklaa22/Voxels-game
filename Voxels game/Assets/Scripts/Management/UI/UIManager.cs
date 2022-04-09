using System;
using UnityEngine;
using UnityEngine.UI;

namespace Management.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("Screens")] 
        [SerializeField] private ScreenUI[] _screens;

        [Header("Rendering chunks")] 
        [SerializeField] private Slider _renderingBar;

        [Header("Inventory")]
        [SerializeField] private RawImage[] _inventoryImages = new RawImage[6];

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SetScreen(ScreenType.RENDER);
        }


        public void SetRenderBarRange(int min, int max)
        {
            _renderingBar.minValue = min;
            _renderingBar.maxValue = max;
        }

        public void SetScreen(ScreenType type)
        {
            foreach (var screen in _screens)
            {
                var active = screen.Type.Equals(type);
                
                screen.SetActive(active);
            }
        }

        public void SetRenderBarValue(int value) => _renderingBar.value = value;


        public void SetInventoryImageActive(int index, bool active, Texture2D image)
        {
            _inventoryImages[index].enabled = active;
            
            if(active)
                _inventoryImages[index].texture = image;
        }
    }
}