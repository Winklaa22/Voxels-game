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

            var xCheck = Mathf.FloorToInt(pos.x);
            var zCheck = Mathf.FloorToInt(pos.z);

            x = xCheck / WorldManager.Instance.ChunkSize.x;
            z = zCheck / WorldManager.Instance.ChunkSize.x;

        }
        
        public bool Equals(ChunkCoord coord)
        {
            return coord == new ChunkCoord(x, z);
        }

    }
}
