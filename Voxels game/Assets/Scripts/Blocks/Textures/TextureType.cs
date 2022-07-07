using Blocks.Side;
using Blocks.Type;
using UnityEngine;

namespace Blocks.Textures
{
    [System.Serializable]
    public class TextureType
    {
        [SerializeField] private BlockSide _blockSide;
        public BlockSide BlockSide => _blockSide;

        [SerializeField] private int _id;

        public int ID => _id;

        public TextureType(BlockSide side, int id)
        {
            _blockSide = side;
            _id = id;
        }
        
    }
}
