using Inventory._Item._ItemProperties;
using Inventory._Item._Type;
using UnityEngine;

namespace Inventory._Item
{
    [CreateAssetMenu(fileName = "new item", menuName = "Item")]
    public class Item : ScriptableObject
    {
        public string Name;
        public Texture2D ItemImage;
        
        [SerializeField] private ItemType _type;
        public ItemType Type => _type;

        [SerializeField] private BlockItem _blockProperties;
        public BlockItem BlockProperties => _blockProperties;
    }
}
