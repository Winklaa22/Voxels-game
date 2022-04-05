using System;
using UnityEngine;
using UnityEngine.UI;

namespace Management.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        
        [Header("Inventory")]
        [SerializeField] private RawImage[] _inventoryImages = new RawImage[6];

        private void Awake()
        {
            Instance = this;
        }

        public void SetInventoryImageActive(int index, bool active, Texture2D image)
        {
            _inventoryImages[index].enabled = active;
            
            if(active)
                _inventoryImages[index].texture = image;
        }
    }
}