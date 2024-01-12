using Assets.Scripts.Blocks.Gallery;
using Assets.Scripts.Controllers.Cursor;
using Blocks;
using Blocks.Block_material;
using Controllers;
using Inventory;
using Management._Cursor;
using Management.UI;
using Management.VoxelManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Gallery
{
    public class GallerySlot : MonoBehaviour
    {
        [SerializeField] private int _id;
        [SerializeField] private bool _hasInfinityBlocks;
        public bool HasInfinityBlocks => _hasInfinityBlocks;

        [SerializeField] private RawImage _blockPicture;
        [SerializeField] private Block _block;
        private bool _cursorEntered;
        private ActionsManager _inputs;

        private void Start()
        {
            _inputs = new ActionsManager();
            _inputs.Enable();

            _inputs.Inventory.GrabAndPut.canceled += context => TakeBlock();
        }

        public void ChangeBlockPicture(Block block)
        {
            _blockPicture.enabled = block != null;
            _block = block;
            
            if(block != null)
                _blockPicture.texture = block.GetPicture();
        }

        public void GrabBlock()
        {
            if(_block is null)
                return;
            
            UIManager.Instance.GrabInventoryBlock(_block, transform.position);
            var blockState = _hasInfinityBlocks ? CursorBlockState.RETURN : CursorBlockState.DESTROY;
            InventoryManager.Instance.GrabBlock(_block.Type, blockState);
            
            if(!_hasInfinityBlocks)
                ChangeBlockPicture(null);
        }

        public void TakeBlock()
        {
            var grabbedBlock = VoxelProperties.GetBlockByType(InventoryManager.Instance.CurrentGrabbedBlock);
            
            if(!_cursorEntered || grabbedBlock.Type.Equals(MaterialType.AIR))
                return;
            
            if(_block is null)
                InventoryManager.Instance.SetCursorBlockState(CursorBlockState.DESTROY);
            
            ChangeBlockPicture(grabbedBlock);
            InventoryManager.Instance.SetSlotByGrabbedBlock(_id);
            InventoryController.Instance.RefreshSlots();
        }

        public void OnCursorEnter()
        {
            InventoryManager.Instance.CursorOnSlot = true;
            _cursorEntered = true;
        }

        public void OnCursorExit()
        {
            InventoryManager.Instance.CursorOnSlot = false;
            _cursorEntered = false;
        }
    }
}
