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
    public class WorldManager : MonoBehaviour
    {
        public static WorldManager Instance;
        [SerializeField] private int _seed;
        [SerializeField] private int _worldSize = 25;
        [SerializeField] private int _viewDistance = 5;
        [SerializeField] private Transform _player;
        [SerializeField] private int _atlasSize = 4;
        public int AtlasSize
        {
            get { return _atlasSize; }
        }

        public float BlockOnAtlasSize
        {
            get { return 1 / (float)_atlasSize; }
        }

        [Header("Chunk Size")]
        [SerializeField] private int _chunkWidth = 6;
        public int ChunkWidth
        {
            get { return _chunkWidth; }
        }

        [SerializeField] private int _chunkHeight = 40;
        public int ChunkHeight
        {
            get { return _chunkHeight; }
        }

        private List<ChunkCoord> _activeChunks = new List<ChunkCoord>();

        private int WorldSizeInVoxels
        {
            get
            {
                return _worldSize * _chunkWidth;
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

        [SerializeField] private TextureType[] _blockTypes;

        public  TextureType[] BlockTypes
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
            Instance = this;
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
            var x = Mathf.FloorToInt(pos.x / _chunkWidth);
            var z = Mathf.FloorToInt(pos.z / _chunkWidth);
            return new ChunkCoord(x, z);
        }
        
        public Chunk GetChunkFromVector3 (Vector3 pos) {

            var x = Mathf.FloorToInt(pos.x / _chunkHeight);
            var z = Mathf.FloorToInt(pos.z / _chunkWidth);
            
            return _chunks[x, z];

        }

        private void CheckViewDistance()
        {
            var coords = GetChunkCoords(_player.position);

            var activeChunks = new List<ChunkCoord>(_activeChunks);
            
            for (int x = coords.x - _viewDistance; x < coords.x + _viewDistance ; x++)
            {
                for (int z = coords.z - _viewDistance; z < coords.z + _viewDistance; z++)
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
                        if (activeChunks[i].x == x && activeChunks[i].z == z)
                            continue;
                        
                        activeChunks.RemoveAt(i);
                    }
                }
            }

            foreach (var chunk in activeChunks)
            {
                _chunks[chunk.x, chunk.z].IsActive = false;
                _activeChunks.Remove(new ChunkCoord(chunk.x, chunk.z));
            }
        }

        private void CreateChunk(int x, int z)
        {
            _chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
            _activeChunks.Add(new ChunkCoord(x, z));
        }

        public bool CheckForVoxel (Vector3 pos) {

            var thisChunk = new ChunkCoord(pos);

            if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > _chunkHeight)
                return false;

            if (_chunks[thisChunk.x, thisChunk.z] != null && _chunks[thisChunk.x, thisChunk.z].IsVoxelMapPopulated)
                return _blockTypes[_chunks[thisChunk.x, thisChunk.z].GetVoxel(pos).ID].IsSolid;

            return _blockTypes[GetVoxelByPosition(pos)].IsSolid;

        }

        public void SetVoxel(Vector3 pos)
        {
            var thisChunk = new ChunkCoord(pos);

            if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > _chunkHeight)
                return;

            var voxel = _chunks[thisChunk.x, thisChunk.z].GetVoxel(pos);
            _chunks[thisChunk.x, thisChunk.z].SetVoxel(voxel, 0);
            Debug.Log(voxel.ID);
        }

        public byte GetVoxelByPosition(Vector3 pos)
        {
            var y = Mathf.FloorToInt(pos.y);

            if(pos.x < (GetChunkCoords(_player.position).x + _viewDistance) - 1 && pos.x >  (GetChunkCoords(_player.position).x - _viewDistance) - 1 && pos.z < (GetChunkCoords(_player.position).z + _viewDistance) - 1 && pos.z >  (GetChunkCoords(_player.position).z - _viewDistance) - 1)
            {
                if (!IsVoxelInTheWorld(pos))
                    return 0;
            }


            if (y.Equals(0))
                return 1;

            var terrainHeight = Mathf.FloorToInt(_chunkHeight * PerlinNoise.GetNoiseMap(new Vector3(pos.x, pos.z), .25f, 0));

            if (y.Equals(terrainHeight))
                return 3;

            return y <= terrainHeight ? (byte) 2 : (byte) 0;

        }



        private bool IsChunkInWorld(ChunkCoord coord)
        {
            return coord.x > 0 && coord.x < _worldSize - 1 && coord.z > 0 && coord.z < _worldSize - 1;
        }

        private bool IsVoxelInTheWorld(Vector3 pos)
        {
            return pos.x >= 0 && pos.x < WorldSizeInVoxels - 1 && pos.y >= 0 && pos.y < _chunkHeight && pos.z >= 1 && pos.z < WorldSizeInVoxels - 1;
        }
    }
}
