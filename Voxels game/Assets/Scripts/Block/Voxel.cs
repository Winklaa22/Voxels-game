using System;
using System.Linq;
using UnityEngine;

namespace Block
{
    public class Voxel : MonoBehaviour
    {
        [SerializeField] private Material _material;

        private BlockSide[] _voxelSides = new BlockSide[]
        {
            BlockSide.TOP,
            BlockSide.BOTTOM,
            BlockSide.LEFT,
            BlockSide.RIGHT,
            BlockSide.FRONT,
            BlockSide.BACK
        };

        private Vector3[] _vertices = new Vector3[8]
        {
            new Vector3(-.5f, -.5f, .5f),
            new Vector3(.5f , -.5f, .5f),
            new Vector3(.5f, .5f, .5f),
            new Vector3(-.5f, .5f, .5f),
            new Vector3(-.5f, -.5f, -.5f),
            new Vector3(.5f, -.5f, -.5f),
            new Vector3(.5f, .5f, -.5f),
            new Vector3(-.5f, .5f, -.5f),
        };  

        private Vector2[] _uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            
        };


        private void Start()
        {
            CreateBlock();
            CombineSides();
        }


        private void CreateBlock()
        {
            foreach (var side in _voxelSides)
            {
                CreateBlockSide(side);
            }
        }

        private void CreateBlockSide(BlockSide side)
        {
            var mesh = new Mesh();
            mesh = GenerateBlockSide(mesh, side);

            var blockSide = new GameObject()
            {
                transform =
                {
                    name = side.ToString(),
                    parent = transform
                }
            };

            var meshFilter = blockSide.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
        }

        private Vector3[] GetNormals(Vector3 direction)
        {
            Vector3[] normals = new Vector3[4]
            {
                direction, direction, direction, direction
            };

            return normals;
        }

        private Mesh GenerateBlockSide(Mesh mesh, BlockSide side)
        {
            switch (side)
            {
                case BlockSide.FRONT:
                {
                    mesh.vertices = new Vector3[]
                    {
                        _vertices[0], _vertices[3],_vertices[2],_vertices[1]
                    };

                    mesh.normals = GetNormals(Vector3.forward);
                    
                    break;
                }
                
                case BlockSide.BACK:
                {
                    mesh.vertices = new Vector3[]
                    {
                        _vertices[6], _vertices[7],_vertices[4],_vertices[5]
                    };
                    
                    mesh.normals = GetNormals(Vector3.back);
                    
                    break;
                }
                case BlockSide.LEFT:
                {
                    mesh.vertices = new Vector3[]
                    {
                        _vertices[3], _vertices[0],_vertices[4],_vertices[7]
                    };

                    mesh.normals = GetNormals(Vector3.left);

                    break;
                }
                case BlockSide.RIGHT:
                {
                    mesh.vertices = new Vector3[]
                    {
                        _vertices[2], _vertices[6],_vertices[5],_vertices[1]
                    };

                    mesh.normals = GetNormals(Vector3.right);

                    break;
                }
                
                case BlockSide.TOP:
                {
                    mesh.vertices = new Vector3[]
                    {
                        _vertices[7], _vertices[6],_vertices[2],_vertices[3]
                    };

                    mesh.normals = GetNormals(Vector3.up);
                    
                    break;
                }
                
                case BlockSide.BOTTOM:
                {
                    mesh.vertices = new Vector3[]
                    {
                        _vertices[5], _vertices[4],_vertices[0],_vertices[1]
                    };
                    
                    mesh.normals = GetNormals(Vector3.down);
                    
                    break;
                }
            }
            
            mesh.uv = new Vector2[]
            {
                _uv[3], _uv[2], _uv[0], _uv[1]
            };
            
            mesh.triangles = new int[]
            {
                3, 1, 0, 3, 2, 1
            };

            return mesh;
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

            gameObject.AddComponent<MeshRenderer>().material = _material;
        }
    }
}
