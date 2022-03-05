using Blocks.Type;
using UnityEngine;

namespace Blocks.Textures
{
    [System.Serializable]
    public class BlockTexture
    {
        [SerializeField] private BlockSide _blockSide;
        public BlockSide BlockSide => _blockSide;


        [SerializeField] private int _id;

        public int ID => _id;

        public BlockTexture(BlockSide side, int id)
        {
            _blockSide = side;
            _id = id;
        }
        
    }
}
