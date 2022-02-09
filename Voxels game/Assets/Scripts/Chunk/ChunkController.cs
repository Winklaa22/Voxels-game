using System;
using System.Collections;
using Block;
using UnityEngine;

namespace Chunk
{
    public class ChunkController : MonoBehaviour
    {
        [SerializeField] private int _size;
        [SerializeField] private Material _blockMaterial;
        private Voxel[,,] _chunkBlocks;

        private void Start()
        {
            StartCoroutine(GenerateChunk(_size));
        }

        private IEnumerator GenerateChunk(int chunkSize)
        {
            _chunkBlocks = new Voxel[chunkSize, chunkSize, chunkSize];

            for (int x = 0; x < chunkSize; x++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    for (int y = 0; y < chunkSize; y++)
                    {
                        _chunkBlocks[x, y, z] = new Voxel(BlockType.DIRT, transform,new Vector3(x, y, z), _blockMaterial);
                    }
                }
            }
            
            for (int x = 0; x < chunkSize; x++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    for (int y = 0; y < chunkSize; y++)
                    {
                        _chunkBlocks[x, y, z].CreateBlock();
                    }
                }
            }
            
            CombineSides();

            yield return null;
        }
        
        private void CombineSides()
        {
            var meshFilters = GetComponentsInChildren<MeshFilter>();
            var combineInstances = new CombineInstance[meshFilters.Length];
            
            Debug.Log(meshFilters.Length);

            for (int i = 0; i < meshFilters.Length; i++)
            {
                combineInstances[i].mesh = meshFilters[i].sharedMesh;
                combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
                Destroy(meshFilters[i].gameObject);
            }
            
            var voxelFilter = gameObject.AddComponent<MeshFilter>();
            voxelFilter.mesh = new Mesh();
            voxelFilter.mesh.CombineMeshes(combineInstances);

            gameObject.AddComponent<MeshRenderer>().material = _blockMaterial;
        }
    }
}
