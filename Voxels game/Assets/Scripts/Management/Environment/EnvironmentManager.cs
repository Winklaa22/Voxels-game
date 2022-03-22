using Blocks.Type;
using NewMathf;
using UnityEngine;

namespace Management.Environment
{
    public class EnvironmentManager : MonoBehaviour
    {
        [SerializeField] private int _seed;
        [SerializeField] private int _worldSize = 500;
        [SerializeField] private int _viewDistance = 5;
        [SerializeField] private Transform _player;
        [SerializeField] private int _atlasSize = 4;
        [SerializeField] private TextureType[] _blockTypes;
        
        public  TextureType[] BlockTypes
        {
            get
            {
                return _blockTypes;
            }
        }
        
        public int AtlasSize
        {
            get { return _atlasSize; }
        }

        public float BlockOnAtlasSize
        {
            get { return 1 / (float)_atlasSize; }
        }

        [Header("Chunk Size")] 
        [SerializeField] private IntVector _chunkSize;

        public IntVector ChunkSize
        {
            get
            {
                return _chunkSize;
            }
        }
    }
}
