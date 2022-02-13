using Blocks.Textures;

namespace Blocks.Type
{
    [System.Serializable]
    public class BlockType
    {
        public string Name = "Default";
        public bool IsSolid;
        public BlockTexture[] Textures;


        public int GetTextureIDFromSide(BlockSide side)
        {
            for (int i = 0; i < Textures.Length; i++)
            {
                if (Textures[i].BlockSide.Equals(side))
                    return Textures[i].ID;
            }
            
            return 0;
        }
    }
}
