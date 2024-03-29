using System.Linq;
using Blocks;
using Blocks.Side;
using Management.WorldManagement;
using UnityEngine;
using MaterialType = Blocks.Block_material.MaterialType;

namespace Management.VoxelManagement
{
    public static class VoxelProperties
    {
        public static readonly BlockSide[] Sides = new BlockSide[6]
        {
            BlockSide.FRONT,
            BlockSide.BACK,
            BlockSide.TOP,
            BlockSide.BOTTOM,
            BlockSide.LEFT,
            BlockSide.RIGHT
        };

        public static readonly Vector3[] FaceCheck = new Vector3[6]
        {
            Vector3.forward,
            Vector3.back, 
            Vector3.up, 
            Vector3.down, 
            Vector3.left, 
            Vector3.right
        };

        public static Block GetBlockByType(MaterialType type)
        {
            var types = WorldGenerator.Instance.BlockTypes;
            return types.First(p => p.Type.Equals(type));
        }
        
        public static byte GetMaterialIndexFromType(MaterialType type)
        {
            var types = WorldGenerator.Instance.BlockTypes;
            
            for (byte i = 0; i < types.Length; i++)
            {
                if(!type.Equals(types[i].Type))
                    continue;

                return i;
            }
            
            return 0;
        }
    }
}
