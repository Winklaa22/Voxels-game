using Blocks.Textures;
using UnityEngine;

namespace Blocks.Type
{
    [System.Serializable]
    public class TextureType
    {
        [SerializeField] private string _name;
        [SerializeField] private VoxelType _type;
        [SerializeField] private bool _isSolid;

        [SerializeField] private BlockTexture[] _textures = new BlockTexture[6];

        public VoxelType Type
        {
            get
            {
                return _type;
            }
        }

        public bool IsSolid
        {
            get
            {
                return _isSolid;
            }
        }


        public int GetTextureIDFromSide(BlockSide side)
        {
            for (int i = 0; i < _textures.Length; i++)
            {
                if (_textures[i].BlockSide.Equals(side))
                    return _textures[i].ID;
            }
            
            return 0;
        }
    }
}
