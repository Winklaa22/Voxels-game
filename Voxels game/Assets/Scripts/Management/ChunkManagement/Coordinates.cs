using Management.WorldManagement;
using NewMathf;
using UnityEngine;

namespace Management.ChunkManagement
{
    public class Coordinates
    {
        public int x, z;

        public Coordinates(int x, int z)
        {
            this.x = x;
            this.z = z;
        }
        
        public Coordinates (Vector3 pos)
        {
            var position = new IntVector(pos);

            x = position.x /2;
            z = position.z / 2;

        }
        
        public bool Equals(Coordinates coord)
        {
            return coord.Equals(new Coordinates(x, z));
        }

    }
}
