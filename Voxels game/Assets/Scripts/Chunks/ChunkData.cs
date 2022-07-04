using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _3D.Mathf2;
using Blocks.Type;
using Management.ChunkManagement;
using Management.VoxelManagement;
using Management.WorldManagement;
using UnityEngine;

namespace Assets.Scripts.Chunks
{
    public class ChunkData : MonoBehaviour
    {
        private ChunkCoord _coordinates;
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private MeshCollider _collider;
        private int _texture;
        private int _vertexIndex = 0;

        [Header("Transform")] 
        private Vector3 _position;

        [Header("Mesh")]
        [SerializeField] private List<Vector3> _vertices = new List<Vector3>();
        [SerializeField] private List<int> _triangles = new List<int>();
        private Mesh _mesh;
        private List<Vector2> _uvs = new List<Vector2>();

        [Header("Map")] 
        private byte[,,] _voxelMap;

        [Header("Threading")] 
        [SerializeField] private bool _threadingIsLocked;

        private WorldGenerator _worldGenerator;
        private bool _isVoxelMapPopulated = false;
        [SerializeField] private bool _isGenerated;

        public bool IsGenerated
        {
            set
            {
                _isGenerated = value;
            }
        }

        private IntVector _size
        {
            get
            {
                return WorldGenerator.Instance.ChunkSize;
            }
        }

        public bool CanModify
        {
            get
            {
                return !_threadingIsLocked && _isVoxelMapPopulated;
            }
        }

        public bool IsActive
        {
            get { return gameObject.activeSelf; }

            set { gameObject.SetActive(value); }
        }

        private void Start()
        {
            _voxelMap = new byte[_size.x, _size.y, _size.z];
            _collider = gameObject.AddComponent<MeshCollider>();
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _worldGenerator = WorldGenerator.Instance;
            _meshRenderer.material = _worldGenerator.WorldMaterial;
            _position = transform.position;
            
            StartCoroutine(TryToCreate());
        }

        public void SetActive(bool active)
        {
            _collider.enabled = active;
        }

        private IEnumerator TryToCreate()
        {
            if (!_isGenerated) 
                yield break;
            
            Initialize();
            yield return null;
        }

        private void Initialize()
        {
            PopulateVoxelMap();
            CreateChunk();
        }

        private void CreateChunk()
        {
            var thread = new Thread(() => ThreadingCreateChunk());
            thread.Start();
        }

        private void ThreadingCreateChunk()
        {
            _threadingIsLocked = true;
            
            for (int y = 0; y < WorldGenerator.Instance.ChunkSize.y; y++)
            {
                for (int x = 0; x < WorldGenerator.Instance.ChunkSize.x; x++)
                {
                    for (int z = 0; z < WorldGenerator.Instance.ChunkSize.z; z++)
                    {
                        if (!_worldGenerator.BlockTypes[_voxelMap[x, y, z]].IsSolid)
                            continue;

                        UpdateMeshData(new Vector3(x, y, z), true);
                    }
                    
                }
            }

            lock (_worldGenerator.ChunksToRender)
            {
                _worldGenerator.ChunksToRender.Enqueue(this);
            }

            _threadingIsLocked = false;
        }
        

        public byte GetVoxelID(Vector3 pos)
        {
            var coords = GetVoxel(pos);
            return _voxelMap[coords.x, coords.y, coords.z];
        }

        public IntVector GetVoxel(Vector3 pos)
        {
            var checkVector = new IntVector(pos);
            var chunkPos = new IntVector(_position);

            checkVector.x -= chunkPos.x;
            checkVector.z -= chunkPos.z;
            checkVector.y = checkVector.y < 0 ? 0 : checkVector.y;
            return new IntVector(checkVector.x, checkVector.y, checkVector.z);
        }


        public void SetVoxel(IntVector voxelCoord, byte id)
        {
            if (!Math3D.IsInsideTheObject(voxelCoord, WorldGenerator.Instance.ChunkSize))
            {
                _voxelMap[voxelCoord.x, voxelCoord.y, voxelCoord.z] = id;
                UpdateChunk();
                UpdateNearestVoxel(voxelCoord);
            }
            else
            {
                var chunk = WorldGenerator.Instance.GetChunkFromVector3(voxelCoord.ToVector3() + _position);
                var voxelPosition = chunk.GetVoxel(voxelCoord.ToVector3() + _position);
                chunk.SetVoxel(voxelPosition , id);
            }
        }

        private void UpdateNearestVoxel(IntVector coord)
        {
            for (int i = 0; i < 6; i++)
            {
                var nearestVoxelPosition = new IntVector(coord.ToVector3() + VoxelData.FaceCheck[i]);
                if (!Math3D.IsInsideTheObject(nearestVoxelPosition, WorldGenerator.Instance.ChunkSize))
                    continue;
                
                var chunk = WorldGenerator.Instance.GetChunkFromVector3(nearestVoxelPosition.ToVector3() + _position);
                chunk.UpdateChunk();
            }
        }


        private void ThreadingUpdateChunk()
        {
            _threadingIsLocked = true;



            _threadingIsLocked = false;
        }

        private void UpdateChunk()
        {
            ClearMesh();

            for (int y = 0; y < WorldGenerator.Instance.ChunkSize.y; y++)
            {
                for (int x = 0; x < WorldGenerator.Instance.ChunkSize.x; x++)
                {
                    for (int z = 0; z < WorldGenerator.Instance.ChunkSize.z; z++)
                    {
                        if (!WorldGenerator.Instance.BlockTypes[_voxelMap[x, y, z]].IsSolid)
                            continue;

                        UpdateMeshData(new Vector3(x, y, z), false);
                    }
                }
            }
            
            CreateMesh();
        }

        private void ClearMesh()
        {
            _vertexIndex = 0;
            _vertices.Clear();
            _triangles.Clear();
            _uvs.Clear();
        }

        private void PopulateVoxelMap()
        {
            for (int y = 0; y < WorldGenerator.Instance.ChunkSize.y; y++)
            {
                for (int x = 0; x < WorldGenerator.Instance.ChunkSize.x; x++)
                {
                    for (int z = 0; z < WorldGenerator.Instance.ChunkSize.z; z++)
                    {
                        var voxelPosition = new Vector3(x, y, z) + _position;
                        var voxelIndex = _worldGenerator.GetVoxelByPosition(voxelPosition);

                        _voxelMap[x, y, z] = voxelIndex;
                    }
                }
            }

            _isVoxelMapPopulated = true;
        }

        public byte GetVoxelType(IntVector coords)
        {
            return _voxelMap[coords.x, coords.y, coords.z];
        }

        bool IsVoxelInChunk(int x, int y, int z)
        {
            return !(x < 0 || x > _worldGenerator.ChunkSize.x - 1 || y < 0 || y > _worldGenerator.ChunkSize.y - 1 ||
                     z < 0 || z > _worldGenerator.ChunkSize.x - 1);
        }

        private bool CanRenderSide(Vector3 pos, bool firstGenerate)
        {
            var intPos = new IntVector(pos);
            var blocks = _worldGenerator.BlockTypes;

            if (!IsVoxelInChunk(intPos.x, intPos.y, intPos.z))
            {
                return !firstGenerate
                    ? _worldGenerator.IsVoxelExist(pos + _position)
                    : _worldGenerator.CheckForVoxel(pos + _position);
            }

            return blocks[_voxelMap[intPos.x, intPos.y, intPos.z]].IsSolid && !blocks[_voxelMap[intPos.x, intPos.y, intPos.z]].IsTransparent;
        }

        private void UpdateMeshData(Vector3 pos, bool firstGenerate)
        {
            var voxelPos = new IntVector(pos);
            var id = _voxelMap[voxelPos.x, voxelPos.y, voxelPos.z];
            var type = _worldGenerator.BlockTypes[id];

            for (int i = 0; i < type.MeshData.Faces.Length; i++)
            {
                if (CanRenderSide(pos + type.MeshData.Faces[i].Value, firstGenerate) )
                    continue;
                
                AddTexture(type, type.GetTextureIDFromSide(VoxelData.Sides[i]), ref _uvs);
                
                CreateFaceData(type, pos, i);
            }
        }
        
        

        private void CreateFaceData(Block type, Vector3 pos, int face)
        {
            // var index = new NativeArray<int>(1, Allocator.TempJob);
            // index[0] = _vertexIndex;
            //
            // var nativeVertices = new NativeList<Vector3>(Allocator.Persistent);
            // var nativeTriangles = new NativeList<int>(Allocator.Persistent);
            //
            // var nativePos = new NativeArray<Vector3>(1, Allocator.TempJob);
            // nativePos[0] = pos;
            //
            // var verData = new NativeArray<Vector3>(type.MeshData.Vertices.Length, Allocator.Persistent);
            // verData.CopyFrom(type.MeshData.Vertices);
            //
            // var trianData = new NativeArray<int>(type.MeshData.Faces[face].Triangles.Length, Allocator.Persistent);
            // trianData.CopyFrom(type.MeshData.Faces[face].Triangles);
            //
            // var job = new GeneratingDataJob()
            // {
            //     vertices = nativeVertices,
            //     triangles = nativeTriangles,
            //     verticesData = verData,
            //     trianglesData = trianData,
            //     position = nativePos,
            //     index = index
            // };
            //
            // var handle = job.Schedule();
            // handle.Complete();
            //
            // nativeTriangles.Dispose();
            // nativeVertices.Dispose();
            // trianData.Dispose();
            // index.Dispose();
            // nativePos.Dispose();
            // verData.Dispose();
            

            for (int j = 0; j < type.MeshData.Faces.Length; j++)
            {
                var trisIndex = type.MeshData.Faces[face].Triangles[j];
                _vertices.Add(type.MeshData.Vertices[trisIndex] + pos);
                _triangles.Add(_vertexIndex);
                _vertexIndex++;
            }
        }

        public void CreateMesh()
        {
            _mesh = new Mesh
            {
                vertices = _vertices.ToArray(),
                triangles = _triangles.ToArray(),
                uv = _uvs.ToArray()
            };
            
            _mesh.RecalculateNormals();
            
            _meshFilter.mesh = _mesh;
            _collider.sharedMesh = _meshFilter.mesh;
        }

        private static void AddTexture(Block type, int textureID, ref List<Vector2> uvs)
        {
            var textureSize = WorldGenerator.Instance.BlockOnAtlasSize;
            float y = textureID / WorldGenerator.Instance.AtlasSize;
            var x = textureID - (y * WorldGenerator.Instance.AtlasSize);

            x *= textureSize;
            y *= textureSize;

            y = 1f - y - textureSize;

            for (int i = 0; i < type.MeshData.Faces.Length; i++)
            {
                var xOffset = type.MeshData.Faces[i].UV.x.Equals(1) ? textureSize : 0;
                var yOffset = type.MeshData.Faces[i].UV.y.Equals(1) ? textureSize : 0;

                uvs.Add(new Vector2(x + xOffset, y + yOffset));
            }
        }
    }
}
