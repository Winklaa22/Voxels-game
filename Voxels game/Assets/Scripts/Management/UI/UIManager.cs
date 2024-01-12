using System.Linq;
using Assets.Scripts.Blocks.Gallery;
using Assets.Scripts.UI.Gallery;
using Blocks;
using Controllers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Management.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        [SerializeField] private SelfWritingText _textAfterSaving;

        public SelfWritingText TextAfterSaving => _textAfterSaving;
        [SerializeField] private RectTransform _mainCanvas;
        public RectTransform MainCanvas => _mainCanvas;

        [Header("Screens")] 
        [SerializeField] private ScreenUI[] _screens;

        [SerializeField] private ScreenType _currentScreen;
        public ScreenType CurrentScreen
        {
            get
            {
                return _currentScreen;
            }
        }

        [Header("Rendering chunks")] 
        [SerializeField] private TextMeshProUGUI _generationCounter;
        [SerializeField] private Image _generationBar;

        [Header("Inventory")]
        [SerializeField] private RectTransform[] _rectSlots;
        [SerializeField] private RawImage[] _slotsImages = new RawImage[6];
        
        
        [Header("Blocks collection")] 
        [SerializeField] private Transform _galleryContent;
        [SerializeField] private BlockGallery _galleryIcon;
        [SerializeField] private GallerySlot _gallerySlot;
        private BlockGallery _grabbedBlock;

        public BlockGallery GrabbedBlock
        {
            get => _galleryIcon;
        }


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

                _currentScreen = active ? type : _currentScreen;

                if(type == ScreenType.HUD)
                    InventoryController.Instance.RefreshSlots();
            }
        }
        
        public ScreenUI GetScreen(ScreenType type)
        {
            return _screens.First(p => p.Type.Equals(type));
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


        public void AddGallerySlot(Block item)
        {
            var slot = Instantiate(_gallerySlot, _galleryContent) as GallerySlot;
            slot.ChangeBlockPicture(item);
        }

        public void GrabInventoryBlock(Block block, Vector3 pos)
        {
            _galleryIcon.Grab(pos, block);
        }

        public bool IsGalleryBlockGrabbed()
        {
            return _galleryIcon.IsGrabbed;
        }

        public void SetRenderBarValue(float value)
        {
            _generationCounter.text = Mathf.FloorToInt(value * 100) + "%";
            _generationBar.fillAmount = value;
        }


        public void SetInventoryImageActive(int index, Texture2D image)
        {
            Debug.Log("Change slot name");
            
            _slotsImages[index].enabled = true;
            _slotsImages[index].texture = image;
        }
    }
}