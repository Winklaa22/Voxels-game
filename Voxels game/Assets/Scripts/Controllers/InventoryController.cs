using System;
using Inventory;
using Management.UI;
using UnityEngine;

namespace Controllers
{
    public class InventoryController : MonoBehaviour
    {
        private int _scrollIndex;

        private void Start()
        {
            UIManager.Instance.SetSlot(InventoryManager.Instance.CurrentSlot);
            var slots = InventoryManager.Instance.Slots;
            for (int i = 0; i < slots.Length; i++)
            {
                UIManager.Instance.SetInventoryImageActive(i, slots[i].ItemImage);
            }
        }

        private void Update()
        {
            if(Input.GetAxis("Mouse ScrollWheel") > 0f && _scrollIndex < 6)
            {
                _scrollIndex++;
                SetSlot(_scrollIndex);
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0f && _scrollIndex > 0)
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
