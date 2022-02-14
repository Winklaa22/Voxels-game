using System;
using Blocks.Type;
using Management.ChunkManagement;
using Management.VoxelManagement;
using UnityEngine;

namespace Management.WorldManagement
{
    public class World : MonoBehaviour
    {
        [SerializeField] private int _worldSize = 5;

        private int WorldSizeInVoxels
        {
            get
            {
                return _worldSize * VoxelData.ChunkSize[0];
            }
        }
        
        [SerializeField] private Material _material;

        public Material WorldMaterial
        {
            get
            {
                return _material;
            }   
        }

        [SerializeField] private BlockType[] _blockTypes;

        public  BlockType[] BlockTypes
        {
            get
            {
                return _blockTypes;
            }
        }

        private Chunk[,] _chunks;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            _chunks = new Chunk[_worldSize, _worldSize];
        }

        private void Start()
        {
            GenerateWorld();
        }

        private void GenerateWorld()
        {
            for (int x = 0; x < _worldSize; x++)
            {
                for (int z = 0; z < _worldSize; z++)
                {
                    CreateChunk(x, z);
                }
            }
        }

        private void CreateChunk(int x, int z)
        {
            _chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
        }

        public byte GetVoxel(Vector3 pos)
        {
            if (!IsVoxelInTheWorld(pos))
                return 0;
            
            return (byte) (pos.y < 1 
                ? 1 
                : pos.y.Equals(VoxelData.ChunkSize[1] - 1) 
                    ? 3 
                    : 2);
        }

        private bool IsChunkInWorld(ChunkCoord coord)
        {
            return coord.X > 0 && coord.X < _worldSize - 1 && coord.Z > 0 && coord.Z < _worldSize - 1;
        }

        private bool IsVoxelInTheWorld(Vector3 pos)
        {
            return pos.x >= 0 && pos.x < WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkSize[1] && pos.z >= 0 && pos.z < WorldSizeInVoxels;
        }
    }
}
