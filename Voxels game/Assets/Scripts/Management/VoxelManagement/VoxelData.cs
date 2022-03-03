using Blocks;
using UnityEngine;

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
            new Vector3(0, 0, 1),
            new Vector3(0, 0, -1),
            new Vector3(0, 1, 0),
            new Vector3(0, -1, 0),
            new Vector3(-1, 0, 0),
            new Vector3(1, 0, 0)
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
        
    }
}
