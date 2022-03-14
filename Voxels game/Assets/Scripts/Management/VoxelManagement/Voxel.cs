using UnityEngine;

namespace Management.VoxelManagement
{
    [System.Serializable]
    public class Voxel
    {
        public byte ID = 0;
        public Vector3 position;

        public Voxel(byte id, Vector3 pos)
        {
            ID = id;
            position = pos;
        }
    }
}
