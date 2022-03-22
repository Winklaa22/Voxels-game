using NewMathf;
using UnityEngine;

namespace Management.VoxelManagement
{
    public class VoxelCoord
    {
        public int x;
        public int y;
        public int z;

        public VoxelCoord(int _x, int _y, int _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public IntVector ToIntVector()
        {
            return new IntVector(x, y, z);
        }
    }
}
