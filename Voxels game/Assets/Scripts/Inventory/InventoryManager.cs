using Assets.Scripts.Blocks.Gallery;
using Blocks.Block_material;
using Management.VoxelManagement;
using UnityEngine;

namespace Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance;
        [SerializeField] private int _currentSlot;
        
        public int CurrentSlot
        {
            get => _currentSlot;
            set => _currentSlot = value;
        }

        private MaterialType _currentGrabbedBlock;

        public MaterialType CurrentGrabbedBlock
        {
            get => _currentGrabbedBlock;
            set => _currentGrabbedBlock = value;
        }
        

        [SerializeField] private MaterialType[] _slots = new MaterialType[6];
        public MaterialType[] Slots
        {
            get => _slots;
            set => _slots = value;
        }

        private CursorBlockState _cursorBlockState;

        public CursorBlockState CursorState => _cursorBlockState;
        public bool CursorOnSlot { get; set; }


        private void Awake()
        {
            Instance = this;
        }


        public void SetSlotByGrabbedBlock(int slot)
        {
            _slots[slot] = _currentGrabbedBlock;
        }
        

        public void GrabBlock(MaterialType type, CursorBlockState state)
        {
            _currentGrabbedBlock = type;
            _cursorBlockState = state;
        }

        public void SetCursorBlockState(CursorBlockState state)
        {
            _cursorBlockState = state;
        }

        public byte GetBlockIndex()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if(_slots[i].Equals(MaterialType.AIR) || !i.Equals(CurrentSlot))
                    continue;

                return VoxelProperties.GetMaterialIndexFromType(_slots[i]);
            }

            return 0;
        }
    }
}
