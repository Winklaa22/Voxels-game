using System;
using Inventory._Item;
using Inventory._Item._Type;
using Management.VoxelManagement;
using Management.WorldManagement;
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

        [SerializeField] private Item[] _slots = new Item[6];
        public Item[] Slots
        {
            get => _slots;
            set => _slots = value;
        }


        private void Awake()
        {
            Instance = this;
        }

        public byte GetBlockIndex()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if(_slots[i] is null || !i.Equals(CurrentSlot))
                    continue;

                return VoxelData.GetMaterialIndexFromType(_slots[i].BlockProperties.Type);
            }

            return 0;
        }
    }
}
