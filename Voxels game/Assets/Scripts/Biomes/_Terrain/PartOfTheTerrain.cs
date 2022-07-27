using Blocks.Block_material;
using UnityEngine;

namespace Biomes._Terrain
{
    [System.Serializable]
    public class PartOfTheTerrain
    {
        public float Threshold;
        public float Increment;

        [Header("Ground")] 
        public MaterialType GroundMaterial;

        [Header("Underground")] 
        public MaterialType UndergroundMaterial;
    }
}
