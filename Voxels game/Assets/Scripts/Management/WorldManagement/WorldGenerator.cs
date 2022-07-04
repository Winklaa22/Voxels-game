using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using _3D.Mathf2;
using Assets.Scripts.Chunks;
using Blocks.Type;
using Controllers.Player;
using Management.ChunkManagement;
using Management.Particles;
using Management.UI;
using Management.VoxelManagement;
using Types.Particles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Management.WorldManagement
{
    public sealed class WorldGenerator : MonoBehaviour
    {
        public static WorldGenerator Instance;
        
        
        [Header("Terrain")]
        [SerializeField] private float _terrainHeight;
        [SerializeField] private int _seed;
        [SerializeField] private float _noiseIncent;
        
        [Header("World data")]
        [SerializeField] private int _worldSize = 25;
        [SerializeField] private int _renderDistance = 5;
        
        [Header("Atlas texture")]
        [SerializeField] private int _atlasSize = 4;
        
        public int AtlasSize
        {
            get { return _atlasSize; }
        }
        
        public float BlockOnAtlasSize
        {
            get { return 1 / (float)_atlasSize; }
        }

        [Header("Caves")] 
        [SerializeField] private float _cavesNoiseScale = .2f;
        [SerializeField] private float _thresholdNoiseCaves = .5f;
        
        [Header("Chunks")] 
        [SerializeField] private IntVector _chunkSize;
        private List<ChunkCoord> _chunksToGenerate = new List<ChunkCoord>();
        private Queue<ChunkData> _chunksToRender = new Queue<ChunkData>();

        public Queue<ChunkData> ChunksToRender
        {
            get
            {
                return _chunksToRender;
            }
        }

        private List<ChunkCoord> _activeChunks = new List<ChunkCoord>();

        
        
        [Header("Player")]
        [SerializeField] private PlayerController _player;
        private ChunkCoord _previousPlayerCoords = new ChunkCoord(0, 0);
        private ChunkCoord _playerCoordinates;
        


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

        private Block[] _blockTypes;

        public Block[] BlockTypes
        {
            get
            {
                return _blockTypes;
            }
        }

        private ChunkData[,] _chunks;

        public delegate void WorldIsGenerated();
        public event WorldIsGenerated OnWorldIsGenerated;
        
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            Instance = this;
            _blockTypes = Resources.LoadAll<Block>("TextureTypes");
            _chunks = new ChunkData[_worldSize, _worldSize];
        }

        private void Start()
        {
            _seed = Random.Range(0, 1000);
            SpawnPlayer();
            StartCoroutine(GenerateWorld());
        }

        private void SpawnPlayer()
        {
            var spawnPos = new Vector3(_worldSize * .5f, _chunkSize.y, _worldSize * .5f);
            _player.transform.position = spawnPos;
            _playerCoordinates = GetChunkCoords(_player.transform.position);
            _previousPlayerCoords = GetChunkCoords(_player.transform.position);
        }

        private void CreateEmptyChunk(int x, int z)
        {
            CreateChunk(x, z);
            _chunksToGenerate.Add(new ChunkCoord(x, z));
        }

        private IEnumerator GenerateWorld()
        {
            var chunksToRenderCount = Mathf.Pow(_renderDistance * 2, 2);
            var generatedChunks = 0.0f;
            for (int x = _playerCoordinates.x - _renderDistance; x < _playerCoordinates.x + _renderDistance; x++)
            {
                for (int z = _playerCoordinates.z - _renderDistance; z < _playerCoordinates.z + _renderDistance; z++)
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

        private void DrawAllChunks()
        {
            if (_chunksToRender.Count <= 0) 
                return;
            
            lock(_chunksToRender)
            {
                if (_chunksToRender.Peek().CanModify)
                {
                    _chunksToRender.Dequeue().CreateMesh();
                }
                    
            }
        }

        private IEnumerator TryToGenerate()
        {
            while (_chunksToGenerate.Count > 0)
            {
                var chunk = _chunks[_chunksToGenerate[0].x, _chunksToGenerate[0].z];
                chunk.IsGenerated = true;
                
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
        
        public ChunkData GetChunkFromVector3 (Vector3 pos) {

            var x = Mathf.FloorToInt(pos.x / _chunkSize.x);
            var z = Mathf.FloorToInt(pos.z / _chunkSize.x);
            
            return _chunks[x, z];

        }


        private void Update()
        {
            DrawAllChunks();
        }

        public IEnumerator UpdateRenderChunks()
        {
            if (_playerCoordinates.Equals(_previousPlayerCoords)) 
                yield break;

            _previousPlayerCoords = _playerCoordinates;
            var previouslyActiveChunks = new List<ChunkCoord>(_activeChunks);

            for (int x = _playerCoordinates.x - _renderDistance; x < _playerCoordinates.x + _renderDistance; x++)
            {
                for (int z = _playerCoordinates.z - _renderDistance; z < _playerCoordinates.z + _renderDistance; z++)
                {
                    if (_chunks[x, z] is null)
                    {
                        CreateEmptyChunk(x, z);
                        StartCoroutine(TryToGenerate());
                        yield return null;
                    }

                    _chunks[x, z].IsActive = true;

                    for (int i = 0; i < previouslyActiveChunks.Count; i++)
                    {

                        if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                            previouslyActiveChunks.RemoveAt(i);
                    }

                    // if (x > PlayerCoords().x - 2 && x < PlayerCoords().x + 2 && z > PlayerCoords().z - 2 && z < PlayerCoords().x + 2)
                    // {
                    //     _chunks[x, z].SetActive(true);
                    // }
                    // else
                    // {
                    //     _chunks[x, z].SetActive(false);
                    // }
                    
                }
            }

            foreach (var chunk in previouslyActiveChunks)
            {
                _chunks[chunk.x, chunk.z].IsActive = false;
                yield return null;
            }
        }

        private void FixedUpdate()
        {
            _playerCoordinates = GetChunkCoords(_player.transform.position);
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
            
            _chunks[x, z] = newChunk.AddComponent<ChunkData>();
            _activeChunks.Add(coord);
        }



        public Block GetVoxelType(Vector3 pos)
        {
            return _blockTypes[GetChunkFromVector3(pos).GetVoxelID(pos)];
        }

        public bool IsVoxelExist(Vector3 pos)
        {
            var thisChunk = new ChunkCoord(pos);

            if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > _chunkSize.y || _chunks[thisChunk.x, thisChunk.z])
                return false;

            var type = _blockTypes[
                _chunks[thisChunk.x, thisChunk.z].GetVoxelType(_chunks[thisChunk.x, thisChunk.z].GetVoxel(pos))];

            return _chunks[thisChunk.x, thisChunk.z] != null ? type.IsSolid && !type.IsTransparent : _blockTypes[GetVoxelByPosition(pos)].IsSolid;
        }
        
        public bool CheckForVoxel (Vector3 pos) {

            var thisChunk = new ChunkCoord(pos);

            if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > _chunkSize.y)
                return false;

            if (_chunks[thisChunk.x, thisChunk.z] != null && _chunks[thisChunk.x, thisChunk.z].CanModify)
                return _blockTypes[_chunks[thisChunk.x, thisChunk.z].GetVoxelType(_chunks[thisChunk.x, thisChunk.z].GetVoxel(pos))].IsSolid;


            return _blockTypes[GetVoxelByPosition(pos)].IsSolid;

        }


        public void CreateDestroyParticle(Vector3 pos)
        {
            var blockType = _blockTypes[GetChunkFromVector3(pos).GetVoxelID(pos)];
            var particleType = ParticlesManager.Instance.GetParticle(blockType.DestroyParticles);
            particleType.ParticleMaterial.mainTexture = blockType.BlockProfile;
            Instantiate(particleType.System, pos, Quaternion.identity);
        }

        public void SetVoxel(ChunkData chunkData ,Vector3 pos, byte type)
        {
            if (pos.y < 0 || pos.y > _chunkSize.y)
                return;

            var voxel = chunkData.GetVoxel(pos);
            chunkData.SetVoxel(voxel, type);
        }

        public byte GetVoxelByPosition(Vector3 pos)
        {
            var y = Mathf.RoundToInt(pos.y);
            var terrainHeight = Mathf.FloorToInt(_chunkSize.y * .25f) +  Mathf.FloorToInt(_terrainHeight * GetNoiseMap(new Vector3(pos.x, pos.z), _noiseIncent, _seed));

            if(AxisFitsToRenderDistance(pos.x) && AxisFitsToRenderDistance(pos.z) && !IsVoxelInTheWorld(pos))
                return VoxelData.GetMaterialIndexFromType(MaterialType.AIR);
            
            
            if (y.Equals(0))
                return VoxelData.GetMaterialIndexFromType(MaterialType.BEDROCK);
            
            
            if (Get3DNoiseMap(pos, 100, _cavesNoiseScale,  _thresholdNoiseCaves))
            {
                return VoxelData.GetMaterialIndexFromType(MaterialType.AIR);
            }
            

            if (y == terrainHeight)
            {
                if (GetNoiseMap(new Vector2(pos.x, pos.z), _noiseIncent, _seed) > .5f)
                {
                    return VoxelData.GetMaterialIndexFromType(MaterialType.STONE);
                }
                
                return VoxelData.GetMaterialIndexFromType(MaterialType.GRASS);
            }

            if (y > terrainHeight && y < Mathf.FloorToInt(_chunkSize.y * .32f))
            {
                if (GetNoiseMap(new Vector2(pos.x, pos.z), _noiseIncent, _seed) < .2)
                {
                    return VoxelData.GetMaterialIndexFromType(MaterialType.WATER);
                }
            }
            

            if (y < terrainHeight)
            {
                if (y < terrainHeight - terrainHeight * 0.05f)
                {
                    
                    
                    return VoxelData.GetMaterialIndexFromType(MaterialType.STONE);
                }
                    
                
                return VoxelData.GetMaterialIndexFromType(MaterialType.DIRT);
            }
                

            return VoxelData.GetMaterialIndexFromType(MaterialType.AIR);

        }

        private bool AxisFitsToRenderDistance(float value)
        {
            return value < (_playerCoordinates.x + _renderDistance) - 1 && value > (_playerCoordinates.x - _renderDistance) - 1;;
        }

        private float GetNoiseMap(Vector3 pos, float increment, float offset)
        {
            var noise = Mathf.PerlinNoise(((pos.x + .1f) / _chunkSize.x * increment + offset), (pos.y + .1f) / _chunkSize.x * increment + offset);
            return noise;
        }

        private bool Get3DNoiseMap(Vector3 position, float offset, float scale, float threshold)
        {
            float x = (position.x + offset + 0.1f) * scale;
            float y = (position.y + offset + 0.1f) * scale;
            float z = (position.z + offset + 0.1f) * scale;

            float AB = Mathf.PerlinNoise(x, y);
            float BC = Mathf.PerlinNoise(y, z);
            float AC = Mathf.PerlinNoise(x, z);
            float BA = Mathf.PerlinNoise(y, x);
            float CB = Mathf.PerlinNoise(z, y);
            float CA = Mathf.PerlinNoise(z, x);

            return (AB + BC + AC + BA + CB + CA) / 6f > threshold;

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
