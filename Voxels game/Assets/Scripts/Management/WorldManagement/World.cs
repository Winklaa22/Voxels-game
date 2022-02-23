using System;
using System.Collections.Generic;
using System.Xml;
using Blocks.Type;
using Management.ChunkManagement;
using Management.VoxelManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Management.WorldManagement
{
    public class World : MonoBehaviour
    {
        [SerializeField] private int _seed;
        [SerializeField] private int _worldSize = 25;
        [SerializeField] private int _viewDistance = 5;
        [SerializeField] private Transform _player;
        private List<ChunkCoord> _activeChunks = new List<ChunkCoord>();

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
            Random.InitState(_seed);
            GenerateWorld();
        }

        private void Update()
        {
            CheckViewDistance();
        }

        private void GenerateWorld()
        {
            var halfSize = (int)(_worldSize * .5f);
            for (int x = halfSize - _viewDistance; x < halfSize + _viewDistance; x++)
            {
                for (int z = halfSize - _viewDistance; z < halfSize + _viewDistance; z++)
                {
                    CreateChunk(x, z);
                }
            }
        }

        private ChunkCoord GetChunkCoords(Vector3 pos)
        {
            var x = Mathf.FloorToInt(pos.x / VoxelData.ChunkSize[0]);
            var z = Mathf.FloorToInt(pos.z / VoxelData.ChunkSize[0]);
            return new ChunkCoord(x, z);
        }

        private void CheckViewDistance()
        {
            var coords = GetChunkCoords(_player.position);

            var activeChunks = new List<ChunkCoord>(_activeChunks);
            
            for (int x = coords.X - _viewDistance; x < coords.X + _viewDistance ; x++)
            {
                for (int z = coords.Z - _viewDistance; z < coords.Z + _viewDistance; z++)
                {
                    if (IsChunkInWorld(new ChunkCoord(x, z)))
                    {
                        if (_chunks[x, z] is null)
                            CreateChunk(x, z);
                        else if (!_chunks[x, z].IsActive)
                        {
                            _chunks[x, z].IsActive = true;
                            _activeChunks.Add(new ChunkCoord(x, z));
                        }
                    }

                    for (var i = 0; i < activeChunks.Count; i++)
                    {
                        if (activeChunks[i].X == x && activeChunks[i].Z == z)
                            activeChunks.RemoveAt(i);
                    }
                }
            }

            foreach (var chunk in activeChunks)
            {
                _chunks[chunk.X, chunk.Z].IsActive = false;
                _activeChunks.Remove(new ChunkCoord(chunk.X, chunk.Z));
            }
        }

        private void CreateChunk(int x, int z)
        {
            _chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
            _activeChunks.Add(new ChunkCoord(x, z));
        }

        public byte GetVoxel(Vector3 pos)
        {
            var y = Mathf.FloorToInt(pos.y);

            if (!IsVoxelInTheWorld(pos))
                return 0;

            if (y.Equals(0))
                return 1;

            var terrainHeight = Mathf.FloorToInt(VoxelData.ChunkSize[1] * PerlinNoise.GetNoiseMap(new Vector3(pos.x, pos.z), .25f, 500));

            if (y.Equals(terrainHeight))
                return 3;

            return y < terrainHeight ? (byte) 2 : (byte) 0;

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
