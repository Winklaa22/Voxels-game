using Blocks.Textures;
using UnityEngine;

namespace Blocks.Type
{
    [CreateAssetMenu(menuName = "Voxel type", fileName = "newVoxelType")]
    public class Block : ScriptableObject
    {
        [SerializeField] private MaterialType _type;
        [SerializeField] private bool _isTransparent;
        [SerializeField] private TextureType[] _textures = new TextureType[6];
        [SerializeField] private MeshData _meshData;
        [SerializeField] private Texture2D _blockProfile;
        public Texture2D BlockProfile
        {
            get
            {
                return _blockProfile;
            }
        }

        public MaterialType Type
        {
            get
            {
                return _type;
            }
        }

        public MeshData MeshData
        {
            get
            {
                return _meshData;
            }
        }

        public bool IsSolid
        {
            get
            {
                return _type != MaterialType.AIR;
            }
        }

        public bool IsTransparent
        {
            get
            {
                return _isTransparent;
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
