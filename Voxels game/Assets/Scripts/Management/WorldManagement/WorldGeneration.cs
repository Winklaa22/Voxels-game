using System.Collections.Generic;
using Blocks.Type;
using Management.ChunkManagement;
using NewMathf;
using UnityEngine;

namespace Management.WorldManagement
{
    public abstract class WorldGeneration
    {
        private int _worldSize;
        private IntVector _chunkSize;
        private GameObject _worldObject;
        private Transform _player;
        private int _viewDistance;
        private List<Coordinates> _activeChunks;
        private Chunk[,] _chunks;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            
        }

        private void Start()
        {
            GenerateWorld();
        }

        private void Update()
        {
            CheckViewDistance();
        }

        public WorldGeneration()
        {
            _chunks = new Chunk[_worldSize, _worldSize];
            
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

        private Coordinates GetChunkCoords(Vector3 pos)
        {
            var x = Mathf.FloorToInt(pos.x / _chunkSize.x);
            var z = Mathf.FloorToInt(pos.z / _chunkSize.x);
            return new Coordinates(x, z);
        }
        
        public Chunk GetChunkFromVector3 (Vector3 pos) {

            var x = Mathf.FloorToInt(pos.x / _chunkSize.x);
            var z = Mathf.FloorToInt(pos.z / _chunkSize.x);
            
            return _chunks[x, z];

        }

        private void CheckViewDistance()
        {
            var coords = GetChunkCoords(_player.position);

            var activeChunks = new List<Coordinates>(_activeChunks);
            
            for (int x = coords.x - _viewDistance; x < coords.x + _viewDistance ; x++)
            {
                for (int z = coords.z - _viewDistance; z < coords.z + _viewDistance; z++)
                {
                    if (IsChunkInWorld(new Coordinates(x, z)))
                    {
                        if (_chunks[x, z] is null)
                            CreateChunk(x, z);
                        
                        else if (!_chunks[x, z].IsActive)
                        {
                            _chunks[x, z].IsActive = true;
                            _activeChunks.Add(new Coordinates(x, z));
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
                _activeChunks.Remove(new Coordinates(chunk.x, chunk.z));
            }
        }

        private void CreateChunk(int x, int z)
        {
            var coord = new Coordinates(x, z);
            
            var newChunk = new GameObject()
            {
                tag = "Chunk",

                transform =
                {
                    name = "Chunk " + coord.x + ", " + coord.z,
                    parent = _worldObject.transform,
                    position = new Vector3(coord.x * _chunkSize.x, 0,
                        coord.z * _chunkSize.x)
                }
            };
            
            _chunks[x, z] = newChunk.AddComponent<Chunk>();
            _activeChunks.Add(coord);
        }

        public bool CheckForVoxel (Vector3 pos) {

            // var thisChunk = new Coordinates(pos);
            //
            // if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > _chunkSize.y)
            //     return false;
            //
            // if (_chunks[thisChunk.x, thisChunk.z] != null && _chunks[thisChunk.x, thisChunk.z].IsVoxelMapSetted)
            // {
            //     var voxelCoord = _chunks[thisChunk.x, thisChunk.z].GetVoxel(pos);
            //     return _blockTypes[_chunks[thisChunk.x, thisChunk.z].GetVoxelType(voxelCoord)].IsSolid;
            // }
            //     
            //
            // return _blockTypes[GetVoxelByPosition(pos)].IsSolid;
            return true;

        }

        public void SetVoxel(Chunk chunk ,Vector3 pos, byte type)
        {

            if (pos.y < 0 || pos.y > _chunkSize.y)
                return;

            var voxel = chunk.GetVoxel(pos);
            chunk.SetVoxel(voxel, type);
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

            var terrainHeight = Mathf.FloorToInt(_chunkSize.y * GetNoiseMap(new Vector3(pos.x, pos.z), .15f, 0) + _chunkSize.y * .2f);

            if (y.Equals(terrainHeight))
                return 3;

            return y <= terrainHeight ? (byte) 2 : (byte) 0;

        }
        
        private float GetNoiseMap(Vector3 pos , float scale, float offset)
        {
            var noise = Mathf.PerlinNoise(((pos.x + .1f) / _chunkSize.x * scale + offset), (pos.y + .1f) / _chunkSize.x * scale + offset);
            return noise;
        }

        private bool IsChunkInWorld(Coordinates coord)
        {
            return coord.x > 0 && coord.x < _worldSize - 1 && coord.z > 0 && coord.z < _worldSize - 1;
        }

        private bool IsVoxelInTheWorld(Vector3 pos)
        {
            return pos.x >= 0 && pos.x < _worldSize - 1 && pos.y >= 0 && pos.y < _chunkSize.y && pos.z >= 1 && pos.z < _worldSize - 1;
        }
    }
}
