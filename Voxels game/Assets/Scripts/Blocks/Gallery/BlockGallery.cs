using Assets.Scripts.UI.Gallery;
using Blocks;
using DG.Tweening;
using Inventory;
using Management._Cursor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Blocks.Gallery
{
    public enum CursorBlockState
    {
        RETURN, DESTROY
    }
    
    public class BlockGallery : MonoBehaviour
    {
        [Header("Returning to slot")]
        [SerializeField] private float _duration;
        [SerializeField] private Ease _ease;
        
        [Header("Grabbing")]
        [SerializeField] private bool _isGrabbed;
        
        [Header("UI")]
        [SerializeField] private Vector3 _parentPos;
        [SerializeField] private RawImage _image;
        
        [Header("Inputs")]
        [SerializeField] private ActionsManager _inputs;

        private GallerySlot _slot;
        private CursorBlockState _state;
        private Block _block;


        public CursorBlockState State
        {
            set
            {
                _state = value;
            }
        }

        public bool IsGrabbed
        {
            get
            {
                return _isGrabbed;
            }
    }

        private void Start()
        {
            _inputs = new ActionsManager();
            _inputs.Enable();

            _inputs.Inventory.GrabAndPut.canceled += context => Put();
        }

        private void LateUpdate()
        {
            TryFollowCursor();
        }

        private void TryFollowCursor()
        {
            if(!_isGrabbed)
                return;

            transform.position = CursorManager.Instance.CursorPosition();
        }

        public void Grab(Vector3 pos, Block block)
        {
            _image.enabled = true;
            _isGrabbed = true;
            _parentPos = pos;
            _image.texture = block.GetPicture();
            _block = block;
        }

        public void Put()
        {
            if (!_isGrabbed)
                return;
            
            if (InventoryManager.Instance.CursorState.Equals(CursorBlockState.RETURN) || !InventoryManager.Instance.CursorOnSlot)
                ReturnToSlot();
            else 
                DestroyBlock();
        }

        public void ReturnToSlot()
        {
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(_parentPos, _duration));
            sequence.SetEase(_ease);
            _isGrabbed = false;
            sequence.OnComplete(() =>
            {
                if (!_isGrabbed)
                {
                    DestroyBlock();
                }

            });
        }

        public void DestroyBlock()
        {
            _image.enabled = false;
        }
    }
}
