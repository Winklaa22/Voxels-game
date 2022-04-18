using Blocks;
using Management.WorldManagement;
using UnityEngine;
using MaterialType = Blocks.Type.MaterialType;

namespace Management.VoxelManagement
{
    public static class VoxelData
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

        public static readonly Vector3[] Vertices = new Vector3[8]
        {
            new Vector3(-.5f, -.5f, .5f), 
            new Vector3(.5f , -.5f, .5f),
            new Vector3(.5f, .5f, .5f),
            new Vector3(-.5f, .5f, .5f),
            new Vector3(-.5f, -.5f, -.5f),
            new Vector3(.5f, -.5f, -.5f),
            new Vector3(.5f, .5f, -.5f),
            new Vector3(-.5f, .5f, -.5f),
        };


        public static readonly int[,] Triangles = new int[6, 6]
        {
            {1, 3, 0, 2, 3, 1}, // Front
            {4, 6, 5, 7, 6, 4}, // Back
            {2, 7, 3, 6, 7, 2}, // Up
            {0, 5, 1, 4, 5, 0}, // Down
            {0, 7, 4, 3, 7, 0}, // Left
            {5, 2, 1, 6, 2, 5}  // Right
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
        
        public static readonly Vector2[] UVs = new Vector2[6]
        {
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(0, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(1, 0),
        };


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
