using System;
using System.Collections;
using System.Collections.Generic;
using Blocks.Block_material;
using Inventory;
using Management.Save;
using UnityEngine;

public class InventoryData : MonoBehaviour, ISaveable
{
    public object CaptureState()
    {
        var manager = InventoryManager.Instance;

        var slots = new List<int>();

        for (var i = 0; i <= 5; i++)
        {
            var slot = (int) manager.Slots[i];
            slots.Add(slot);
        }

        return new SaveData
        {
            CurrentSlot = manager.CurrentSlot,
            Slots = slots.ToArray()
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData) state;
        InventoryManager.Instance.CurrentSlot = saveData.CurrentSlot;

        for (var i = 0; i <= 5; i++)
        {
            InventoryManager.Instance.Slots[i] = (MaterialType) saveData.Slots[i];
        }
    }
    
    [Serializable]
    public struct SaveData
    {
        public int CurrentSlot;
        public int[] Slots;
    }
}
