using System.Collections.Generic;
using Blocks.Textures;
using Types.Particles;
using UnityEngine;

namespace Blocks.Type
{
    [CreateAssetMenu(menuName = "Voxel type", fileName = "newVoxelType")]
    public class Block : ScriptableObject
    {
        [SerializeField] private MaterialType _type;
        [SerializeField] private bool _isTransparent;
        [SerializeField] private TextureType[] _textures = new TextureType[6];
        [SerializeField] private MeshData _meshData;
        [SerializeField] private Texture2D _blockProfile;
        [SerializeField] private ParticlesName _destroyParticles;

        public ParticlesName DestroyParticles
        {
            get
            {
                return _destroyParticles;
            }
        }

        public Texture2D BlockProfile
        {
            get
            {
                return _blockProfile;
            }
        }

        public MaterialType Type
        {
            get
            {
                return _type;
            }
        }

        public MeshData MeshData
        {
            get
            {
                return _meshData;
            }
        }

        public bool IsSolid
        {
            get
            {
                return _type != MaterialType.AIR;
            }
        }

        public bool IsTransparent
        {
            get
            {
                return _isTransparent;
            }
        }

        public Mesh GetVoxelMesh()
        {
            var vertexIndex = 0;
            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            for (int i = 0; i < MeshData.Faces.Length; i++)
            {
                for (int j = 0; j < MeshData.Faces.Length; j++)
                {
                    var trisIndex = MeshData.Faces[i].Triangles[j];
                    vertices.Add(MeshData.Vertices[trisIndex]);
                    triangles.Add(vertexIndex);
                    vertexIndex++;
                }
            }

            var mesh = new Mesh()
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray()
                
            };
            mesh.name = _type.ToString();
            mesh.RecalculateNormals();



            return mesh;
        }


        public int GetTextureIDFromSide(BlockSide side)
        {
            for (int i = 0; i < _textures.Length; i++)
            {
                if (_textures[i].BlockSide.Equals(side))
                    return _textures[i].ID;
            }
            
            return 0;
        }
    }
}
