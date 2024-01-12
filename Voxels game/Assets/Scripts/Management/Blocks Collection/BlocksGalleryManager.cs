using Assets.Scripts.Pickable;
using Blocks;
using UnityEngine;

namespace Assets.Scripts.Management.Blocks_Collection
{
    public class BlocksGalleryManager : MonoBehaviour
    {
        public static BlocksGalleryManager Instance;
        
        [SerializeField] private Block[] _items;

        public Block[] Items
        {
            get
            {
                return _items;
            }
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}
