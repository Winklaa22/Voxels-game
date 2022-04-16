using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using _3D.Mathf2;
using Blocks.Type;
using Controllers.Player;
using Management.ChunkManagement;
using Management.UI;
using Management.VoxelManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Management.WorldManagement
{
    public sealed class WorldGenerator : MonoBehaviour
    {
        public static WorldGenerator Instance;
        [SerializeField] private int _seed;
        [SerializeField] private int _worldSize = 25;
        [SerializeField] private int _renderDistance = 5;
        [SerializeField] private PlayerController _player;
        [SerializeField] private int _atlasSize = 4;
        private List<ChunkCoord> _chunksToGenerate = new List<ChunkCoord>();
        private ChunkCoord _previousPlayerCoords = new ChunkCoord(0, 0);
        public int AtlasSize
        {
            get { return _atlasSize; }
        }

        public float BlockOnAtlasSize
        {
            get { return 1 / (float)_atlasSize; }
        }

        private List<ChunkCoord> _activeChunks = new List<ChunkCoord>();

        [SerializeField] private IntVector _chunkSize;

        public IntVector ChunkSize
        {
            get
            {
                return _chunkSize;
            }
        }
        

        private int WorldSizeInVoxels
        {
            get
            {
                return _worldSize * _chunkSize.x;
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

        [SerializeField] private VoxelType[] _blockTypes;

        public VoxelType[] BlockTypes
        {
            get
            {
                return _blockTypes;
            }
        }

        private Chunk[,] _chunks;

        public delegate void WorldIsGenerated();
        public event WorldIsGenerated OnWorldIsGenerated;
        
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            Instance = this;
            _blockTypes = Resources.LoadAll<VoxelType>("TextureTypes");
            _chunks = new Chunk[_worldSize, _worldSize];
        }

        private void Start()
        {
            Random.InitState(_seed);

            _previousPlayerCoords = PlayerCoords();
            SpawnPlayer();
            StartCoroutine(GenerateWorld());
        }

        private void SpawnPlayer()
        {
            var spawnPos = new Vector3(_worldSize * .5f, 40, _worldSize * .5f);
            _player.transform.position = spawnPos;
        }

        private void CreateEmptyChunk(int x, int z)
        {
            CreateChunk(x, z);
            _chunksToGenerate.Add(new ChunkCoord(x, z));
        }
        
        private ChunkCoord PlayerCoords() => GetChunkCoords(_player.transform.position);
        
        private IEnumerator GenerateWorld()
        {
            var chunksToRenderCount = Mathf.Pow(_renderDistance * 2, 2);;
            var generatedChunks = 0.0f;
            for (int x = PlayerCoords().x - _renderDistance; x < PlayerCoords().x + _renderDistance; x++)
            {
                for (int z = PlayerCoords().z - _renderDistance; z < PlayerCoords().z + _renderDistance; z++)
                {
                    CreateEmptyChunk(x, z);
                    generatedChunks++;
                    UIManager.Instance.SetRenderBarValue(generatedChunks/chunksToRenderCount);
                    yield return TryToGenerate();
                }
            }
            
            OnWorldIsGenerated?.Invoke();
            yield return null;
        }
        


        private IEnumerator TryToGenerate()
        {
            while (_chunksToGenerate.Count > 0)
            {
                _chunks[_chunksToGenerate[0].x, _chunksToGenerate[0].z].IsGenerated = true;
                _chunksToGenerate.RemoveAt(0);
            }

            yield return null;
        }

        private ChunkCoord GetChunkCoords(Vector3 pos)
        {
            var x = Mathf.FloorToInt(pos.x / _chunkSize.x);
            var z = Mathf.FloorToInt(pos.z / _chunkSize.x);
            return new ChunkCoord(x, z);
        }
        
        public Chunk GetChunkFromVector3 (Vector3 pos) {

            var x = Mathf.FloorToInt(pos.x / _chunkSize.x);
            var z = Mathf.FloorToInt(pos.z / _chunkSize.x);
            
            return _chunks[x, z];

        }


        public IEnumerator UpdateRenderChunks()
        {
            if (PlayerCoords().Equals(_previousPlayerCoords)) 
                yield break;

            _previousPlayerCoords = PlayerCoords();
            var previouslyActiveChunks = new List<ChunkCoord>(_activeChunks);

            for (int x = PlayerCoords().x - _renderDistance; x < PlayerCoords().x + _renderDistance; x++)
            {
                for (int z = PlayerCoords().z - _renderDistance; z < PlayerCoords().z + _renderDistance; z++)
                {
                    if (_chunks[x, z] is null)
                    {
                        CreateEmptyChunk(x, z);
                        StartCoroutine(TryToGenerate());
                        yield return null;
                    }


                    _chunks[x, z].IsActive = true;

                    for (int i = 0; i < previouslyActiveChunks.Count; i++) {

                        if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                            previouslyActiveChunks.RemoveAt(i);
                       
                    }

                }
            }

            foreach (var chunk in previouslyActiveChunks)
            {
                _chunks[chunk.x, chunk.z].IsActive = false;
                yield return null;
            }
        }

        private void CreateChunk(int x, int z)
        {
            var coord = new ChunkCoord(x, z);
            
            var newChunk = new GameObject()
            {
                tag = "Chunk",

                transform =
                {
                    name = "Chunk " + coord.x + ", " + coord.z,
                    parent = transform,
                    position = new Vector3(coord.x * _chunkSize.x, 0,
                        coord.z * _chunkSize.x)
                }
            };
            
            _chunks[x, z] = newChunk.AddComponent<Chunk>();
            _activeChunks.Add(coord);
        }

        public bool CheckForVoxel (Vector3 pos) {

            var thisChunk = new ChunkCoord(pos);

            if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > _chunkSize.y)
                return false;

            if (_chunks[thisChunk.x, thisChunk.z] != null && _chunks[thisChunk.x, thisChunk.z].IsVoxelMapPopulated)
                return _blockTypes[_chunks[thisChunk.x, thisChunk.z].GetVoxelType(_chunks[thisChunk.x, thisChunk.z].GetVoxel(pos))].IsSolid;


            return _blockTypes[GetVoxelByPosition(pos)].IsSolid;

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

            if(pos.x < (PlayerCoords().x + _renderDistance) - 1 && pos.x >  (PlayerCoords().x - _renderDistance) - 1 && pos.z < (PlayerCoords().z + _renderDistance) - 1 && pos.z >  (PlayerCoords().z - _renderDistance) - 1)
            {
                if (!IsVoxelInTheWorld(pos))
                    return VoxelData.GetMaterialIndexFromType(MaterialType.AIR);
            }
            
            if (y.Equals(0))
                return VoxelData.GetMaterialIndexFromType(MaterialType.BEDROCK);

            var terrainHeight = Mathf.FloorToInt(_chunkSize.y * GetNoiseMap(new Vector3(pos.x, pos.z), .25f, 0));

            if (y.Equals(terrainHeight))
                return VoxelData.GetMaterialIndexFromType(MaterialType.GRASS);

            return y <= terrainHeight ? VoxelData.GetMaterialIndexFromType(MaterialType.DIRT) : VoxelData.GetMaterialIndexFromType(MaterialType.AIR);

        }

        private float GetNoiseMap(Vector3 pos, float scale, float offset)
        {
            var noise = Mathf.PerlinNoise(((pos.x + .1f) / _chunkSize.x * scale + offset), (pos.y + .1f) / _chunkSize.x * scale + offset);
            return noise;
        }

        private bool IsChunkInWorld(ChunkCoord coord)
        {
            return coord.x > 0 && coord.x < _worldSize - 1 && coord.z > 0 && coord.z < _worldSize - 1;
        }

        private bool IsVoxelInTheWorld(Vector3 pos)
        {
            return pos.x >= 0 && pos.x < WorldSizeInVoxels - 1 && pos.y >= 0 && pos.y < _chunkSize.y && pos.z >= 1 && pos.z < WorldSizeInVoxels - 1;
        }
    }
}
