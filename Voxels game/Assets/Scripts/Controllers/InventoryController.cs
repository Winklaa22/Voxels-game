using System;
using Inventory;
using Management.UI;
using UnityEngine;

namespace Controllers
{
    public class InventoryController : MonoBehaviour
    {
        private int _scrollIndex;
        private ActionsManager _inputs;

        private void Start()
        {
            UIManager.Instance.SetSlot(InventoryManager.Instance.CurrentSlot);
            var slots = InventoryManager.Instance.Slots;
            for (int i = 0; i < slots.Length; i++)
            {
                UIManager.Instance.SetInventoryImageActive(i, slots[i].ItemImage);
            }

            _inputs = new ActionsManager();
            _inputs.Enable();
        }


        private void Update()
        {
            UpdateScroll();
        }

        private void UpdateScroll()
        {
            var scrollValue = _inputs.Player.Scrolling.ReadValue<float>();
            
            if(scrollValue > 0f && _scrollIndex < 6)
            {
                _scrollIndex++;
                SetSlot(_scrollIndex);
            }

            if (scrollValue < 0f && _scrollIndex > 0)
            {
                _scrollIndex--;
                SetSlot(_scrollIndex);
            }
        }

        private void SetSlot(int slot)
        {
            InventoryManager.Instance.CurrentSlot = slot;
            UIManager.Instance.SetSlot(slot);
        }
    }
}
