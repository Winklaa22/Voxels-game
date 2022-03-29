using Blocks.Textures;
using UnityEngine;

namespace Blocks.Type
{
    [CreateAssetMenu(menuName = "Voxel type", fileName = "newVoxelType")]
    public class VoxelType : ScriptableObject
    {
        [SerializeField] private MaterialType _type;
        [SerializeField] private bool _isSolid;

        [SerializeField] private TextureType[] _textures = new TextureType[6];

        public MaterialType Type
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
