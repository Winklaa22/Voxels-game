using UnityEngine;

namespace Blocks.Type
{
    [CreateAssetMenu(fileName = "newMeshData", menuName = "Mesh data")]
    public class MeshData : ScriptableObject
    {
        [SerializeField] private FaceData[] _faces;

        public FaceData[] Faces
        {
            get
            {
                return _faces;
            }
        }

        [SerializeField] private Vector3[] _vertices = new Vector3[8]
        {
            new Vector3(-.5f, -.5f, .5f), 
            new Vector3(.5f, -.5f, .5f),
            new Vector3(.5f, .5f, .5f),
            new Vector3(-.5f, .5f, .5f),
            new Vector3(-.5f, -.5f, -.5f),
            new Vector3(.5f, -.5f, -.5f),
            new Vector3(.5f, .5f, -.5f),
            new Vector3(-.5f, .5f, -.5f),
        };

        public Vector3[] Vertices
        {
            get
            {
                return _vertices;
            }
        }
    }
}