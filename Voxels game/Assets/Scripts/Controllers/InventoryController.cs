using System;
using Inventory;
using Management.Cursor;
using Management.Game;
using Management.UI;
using UnityEngine;

namespace Controllers
{
    public class InventoryController : MonoBehaviour
    {
        private int _scrollIndex;
        private ActionsManager _inputs;
        private bool _active;

        private void Awake()
        {
            _inputs = new ActionsManager();
            _inputs.Enable();
            
            _inputs.Player.InventoryActive.started += ctx => ChangeActive();
        }

        private void Start()
        {
            UIManager.Instance.SetSlot(0);
            var slots = InventoryManager.Instance.Slots;
            for (int i = 0; i < slots.Length; i++)
            {
                UIManager.Instance.SetInventoryImageActive(i, slots[i].ItemImage);
            }
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

        private void ChangeActive()
        {
            _active = !_active;
            Cursor.visible = _active;
            Cursor.lockState = _active ? CursorLockMode.None : CursorLockMode.Locked;
            var type = _active ? ScreenType.INVENTORY : ScreenType.HUD;
            UIManager.Instance.SetScreen(type);
            GameManager.Instance.MainPlayer.SetActive(!_active);
        }

        private void SetSlot(int slot)
        {
            InventoryManager.Instance.CurrentSlot = slot;
            UIManager.Instance.SetSlot(slot);
        }
    }
}
