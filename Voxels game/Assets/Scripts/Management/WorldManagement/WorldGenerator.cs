using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using _3D.Mathf2;
using Biomes._Terrain;
using Blocks;
using Blocks.Block_material;
using Blocks.Textures;
using Blocks.Type;
using Chunks;
using Controllers.Player;
using Management.Particles;
using Management.Save;
using Management.UI;
using Management.VoxelManagement;
using Types.Particles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Management.WorldManagement
{
    public sealed class WorldGenerator : MonoBehaviour, ISaveable
    {
        public static WorldGenerator Instance;

        [SerializeField] private string _worldName;

        public string WorldName => _worldName;

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

        [Header("Terrain values")] 
        [SerializeField] private PartOfTheTerrain[] _terrainParts;

        [Header("Caves")] 
        [SerializeField] private float _cavesNoiseScale = .2f;
        [SerializeField] private float _thresholdNoiseCaves = .5f;
        
        [Header("Chunks")] 
        [SerializeField] private IntVector _chunkSize;
        private List<ChunkCoord> _chunksToGenerate = new List<ChunkCoord>();
        [SerializeField] private Dictionary<string, ChunkData> _modifiedChunks = new Dictionary<string, ChunkData>();

        private Queue<Chunk> _chunksToRender = new Queue<Chunk>();

        public Queue<Chunk> ChunksToRender
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

        [SerializeField] private Block[] _blockTypes;

        public Block[] BlockTypes
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
            _player.OnSpawnEntity += () => {StartCoroutine(GenerateWorld());};
            _chunks = new Chunk[_worldSize, _worldSize];
        }

        private void Start()
        {
            _seed = Random.Range(0, 99999);
        }

        public void SetPlayersCoord(Vector3 pos)
        {
            _player.transform.position = pos;
            _playerCoordinates = GetChunkCoords(_player.transform.position);
            _previousPlayerCoords = GetChunkCoords(_player.transform.position);
        }

        public Vector3 GetSpawnPosition()
        {
            var halfSize = _worldSize * .5f;
            return new Vector3(halfSize, GetNoiseHeight(new Vector3(halfSize, 0 ,halfSize)) + 2, halfSize);
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

        public bool CheckSavedVoxel(IntVector pos)
        {
            var chunk = GetChunkFromVector3(pos.ToVector3());
            
            return chunk.Data.ModifiedVoxels.ContainsKey(pos.ToString());
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
                chunk.CanBeGenerate = true;

                if (_modifiedChunks.ContainsKey(chunk.Data.Name))
                {
                    chunk.Data.LoadData(_modifiedChunks[chunk.Data.Name].ModifiedVoxels);
                }
                    

                _chunksToGenerate.RemoveAt(0);
            }

            yield return null;
        }
        
        public ChunkCoord GetChunkCoords(Vector3 pos)
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
            
            _chunks[x, z] = newChunk.AddComponent<Chunk>();
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

        public void SetVoxel(Chunk chunk ,Vector3 pos, byte type)
        {
            if (pos.y < 0 || pos.y > _chunkSize.y)
                return;

            var voxel = chunk.GetVoxel(pos);
            chunk.SetVoxel(voxel, type);
        }

        public byte GetVoxelByPosition(Vector3 pos)
        {
            var y = Mathf.RoundToInt(pos.y);
            var terrainHeight = GetNoiseHeight(pos);

            if(AxisFitsToRenderDistance(pos.x) && AxisFitsToRenderDistance(pos.z) && !IsVoxelInTheWorld(pos))
                return VoxelProperties.GetMaterialIndexFromType(MaterialType.AIR);
            
            // Bedrock
            if (y.Equals(0))
                return VoxelProperties.GetMaterialIndexFromType(MaterialType.BEDROCK);
            
            // Caves
            if (Get3DNoiseMap(pos, 100, _cavesNoiseScale, _thresholdNoiseCaves))
            {
                return VoxelProperties.GetMaterialIndexFromType(MaterialType.AIR);
            }
            
            
            foreach (var part in _terrainParts)
            {
                if (y == terrainHeight)
                {
                    if (GetNoiseMap(new Vector2(pos.x, pos.z), _noiseIncent, _seed) > part.Threshold)
                    {
                        return VoxelProperties.GetMaterialIndexFromType(part.GroundMaterial);
                    }
                    
                    return VoxelProperties.GetMaterialIndexFromType(MaterialType.GRASS);
                }
            }
            
            if (y < terrainHeight)
            {
                if (y < terrainHeight - terrainHeight * 0.05f)
                {
                    return VoxelProperties.GetMaterialIndexFromType(MaterialType.STONE);
                }
                
                return VoxelProperties.GetMaterialIndexFromType(MaterialType.DIRT);
            }

            return VoxelProperties.GetMaterialIndexFromType(MaterialType.AIR);

        }

        private int GetNoiseHeight(Vector3 pos)
        {
            return Mathf.FloorToInt(_chunkSize.y * .25f) +  Mathf.FloorToInt(_terrainHeight * GetNoiseMap(new Vector3(pos.x, pos.z), _noiseIncent, _seed));
        }

        public void ModifyChunk(string name, ChunkData data)
        {
            if(!_modifiedChunks.ContainsKey(name))
                _modifiedChunks.Add(name, data);
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

        public object CaptureState()
        {
            return new SaveData
            {
                Seed = _seed,
                ChunksToSave = _modifiedChunks
            };
        }
        

        public void RestoreState(object state)
        {
            var saveData = (SaveData) state;
            _seed = saveData.Seed;
            _modifiedChunks = saveData.ChunksToSave;

            // foreach (var chunk in _modifiedChuks)
            // {
            //     Debug.Log(chunk.Name);
            // }
        }
        
        [System.Serializable]
        private struct SaveData
        {
            public int Seed;
            public Dictionary<string, ChunkData> ChunksToSave;
        }
    }
}
