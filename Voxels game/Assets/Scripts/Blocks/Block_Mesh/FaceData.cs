using UnityEngine;

namespace Blocks.Block_Mesh
{
    [System.Serializable]
    public class FaceData
    {
        public string Name;
        public Vector3 Value;
        public int[] Triangles = new int[6];
        public Vector2 UV;
    }
}