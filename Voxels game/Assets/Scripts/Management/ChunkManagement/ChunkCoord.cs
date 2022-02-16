using UnityEngine;

namespace Management.ChunkManagement
{
    public class ChunkCoord
    {
        public int X, Z;

        public ChunkCoord(int x, int z)
        {
            X = x;
            Z = z;
        }
        
        public bool Equals(ChunkCoord coord)
        {
            return coord == new ChunkCoord(X, Z);
        }

    }
}
