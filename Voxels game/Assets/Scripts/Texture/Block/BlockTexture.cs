using Block;
using UnityEngine;

namespace Texture.Block
{
    [System.Serializable]
    public class BlockTexture
    {
        public BlockType Type;
        public Texture2D[] Textures = new Texture2D[1];
        public bool IsSolid;
    }
}
