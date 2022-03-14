using Management.WorldManagement;
using UnityEngine;

namespace Management.ChunkManagement
{
    public class ChunkCoord
    {
        public int x, z;

        public ChunkCoord(int x, int z)
        {
            this.x = x;
            this.z = z;
        }
        
        public ChunkCoord (Vector3 pos) {

            int xCheck = Mathf.FloorToInt(pos.x);
            int zCheck = Mathf.FloorToInt(pos.z);

            x = xCheck / WorldManager.Instance.ChunkWidth;
            z = zCheck / WorldManager.Instance.ChunkWidth;

        }
        
        public bool Equals(ChunkCoord coord)
        {
            return coord == new ChunkCoord(x, z);
        }

    }
}
