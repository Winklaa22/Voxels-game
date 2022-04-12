using Inventory._Item._ItemProperties;
using Inventory._Item._Type;
using UnityEngine;

namespace Inventory._Item
{
    [System.Serializable]
    public class Item
    {
        [SerializeField] private string Name;
        public Texture2D ItemImage;
        [SerializeField] private BlockItem _blockProperties;
        public BlockItem BlockProperties => _blockProperties;
    }
}
