using System;
using System.Collections.Generic;
using System.Linq;
using ChunkManagement;
using JetBrains.Annotations;
using UnityEngine;
using VoxelManagement;

// namespace Block
// {
//     public class Voxel
//     {
//         private BlockType _type;
//         private bool _isTransparent;
//         private Vector3 _position;
//         private Chunk _chunkParent;
//         private Dictionary<BlockType, Rect> _uvCoordinates;
//         
//         #region MESH_PARAMETERS
//         
//
//         #endregion
//
//         public Voxel(BlockType type, Chunk chunkParent , Vector3 position, Dictionary<BlockType, Rect> uvCoords)
//         {
//             _type = type;
//             _position = position;
//             _chunkParent = chunkParent;
//             _uvCoordinates = uvCoords;
//             _isTransparent = type.Equals(BlockType.AIR);
//         }
//
//         private Vector3 GetSideDirection(BlockSide side)
//         {
//             switch (side)
//             {
//                 case BlockSide.TOP:
//                     return new Vector3(0, 1, 0);
//                 
//                 case BlockSide.BOTTOM:
//                     return new Vector3(0, -1, 0);
//
//                 case BlockSide.FRONT:
//                     return new Vector3(0, 0, 1);
//
//                 case BlockSide.BACK:
//                     return new Vector3(0, 0, -1);
//                 
//                 case BlockSide.RIGHT:
//                     return new Vector3(1, 0, 0);
//                 
//                 case BlockSide.LEFT:
//                     return new Vector3(-1, 0, 0);
//             }
//             return Vector3.zero;
//         }
//
//         private bool HasTransparentNeighbour(BlockSide blockSide)
//         {
//             var chunkVoxels = _chunkParent.ChunkBlocks;
//
//             var neighbourPos = _position + GetSideDirection(blockSide);
//
//             if (!(neighbourPos.x >= 0 && neighbourPos.y >= 0 && neighbourPos.z >= 0)) 
//                 return true;
//             
//             if (neighbourPos.x < chunkVoxels.GetLength(0))
//             {
//                 if (neighbourPos.y < chunkVoxels.GetLength(1))
//                 {
//                     if (neighbourPos.z < chunkVoxels.GetLength(2))
//                     {
//                         return chunkVoxels[(int) neighbourPos.x, (int) neighbourPos.y, (int) neighbourPos.z]._isTransparent;
//                     }
//                 }
//             }
//             
//             return true;
//         }
//         
//         
//         public void CreateBlock()
//         {
//             foreach (var side in VoxelData.VoxelSides)
//             {
//                 if(HasTransparentNeighbour(side))
//                     CreateBlockSide(side);
//             }
//         }
//         
//
//         private void CreateBlockSide(BlockSide side)
//         {
//             var uvs = GetBlockSideUVs();
//             
//             var mesh = new Mesh();
//             mesh = GenerateBlockSide(mesh, side, uvs);
//
//             var blockSide = new GameObject()
//             {
//                 name = side.ToString(),
//
//                 transform =
//                 {
//                     parent = _chunkParent.transform,
//                     position = _position
//                 }
//             };
//                 
//             var meshFilter = blockSide.AddComponent<MeshFilter>();
//             meshFilter.mesh = mesh;
//         }
//         
//         private Vector2[] GetBlockSideUVs()
//         {
//             Vector2[] uvs;
//
//             var uvCoord = _uvCoordinates[_type];
//
//             uvs = new Vector2[4]
//             {
//                 new Vector2(uvCoord.x, uvCoord.y),
//                 new Vector2(uvCoord.x + uvCoord.width, uvCoord.y),
//                 new Vector2(uvCoord.x, uvCoord.y + uvCoord.height),
//                 new Vector2(uvCoord.x + uvCoord.width, uvCoord.y + uvCoord.height),
//             };
//             
//             return uvs;
//         }
//         
//         private Vector3[] GetNormals(Vector3 direction)
//         {
//             Vector3[] normals = new Vector3[4]
//             {
//                 direction, direction, direction, direction
//             };
//
//             return normals;
//         }
//
//         private Mesh GenerateBlockSide(Mesh mesh, BlockSide side, Vector2[] uv)
//         {
//             switch (side)
//             {
//                 case BlockSide.FRONT:
//                 {
//                     mesh.vertices = new Vector3[]
//                     {
//                         VoxelData.Vertices[0], VoxelData.Vertices[3],VoxelData.Vertices[2],VoxelData.Vertices[1]
//                     };
//
//                     mesh.normals = GetNormals(Vector3.forward);
//                     
//                     break;
//                 }
//                 
//                 case BlockSide.BACK:
//                 {
//                     mesh.vertices = new Vector3[]
//                     {
//                         VoxelData.Vertices[6], VoxelData.Vertices[7], VoxelData.Vertices[4], VoxelData.Vertices[5]
//                     };
//                     
//                     mesh.normals = GetNormals(Vector3.back);
//                     
//                     break;
//                 }
//                 case BlockSide.LEFT:
//                 {
//                     mesh.vertices = new Vector3[]
//                     {
//                         VoxelData.Vertices[3], VoxelData.Vertices[0], VoxelData.Vertices[4], VoxelData.Vertices[7]
//                     };
//
//                     mesh.normals = GetNormals(Vector3.left);
//
//                     break;
//                 }
//                 case BlockSide.RIGHT:
//                 {
//                     mesh.vertices = new Vector3[]
//                     {
//                         VoxelData.Vertices[2], VoxelData.Vertices[6], VoxelData.Vertices[5], VoxelData.Vertices[1]
//                     };
//
//                     mesh.normals = GetNormals(Vector3.right);
//
//                     break;
//                 }
//                 
//                 case BlockSide.TOP:
//                 {
//                     mesh.vertices = new Vector3[]
//                     {
//                         VoxelData.Vertices[7], VoxelData.Vertices[6], VoxelData.Vertices[2], VoxelData.Vertices[3]
//                     };
//
//                     mesh.normals = GetNormals(Vector3.up);
//                     
//                     break;
//                 }
//                 
//                 case BlockSide.BOTTOM:
//                 {
//                     mesh.vertices = new Vector3[]
//                     {
//                         VoxelData.Vertices[5], VoxelData.Vertices[4], VoxelData.Vertices[0], VoxelData.Vertices[1]
//                     };
//                     
//                     mesh.normals = GetNormals(Vector3.down);
//                     
//                     break;
//                 }
//             }
//             
//             mesh.uv = new Vector2[]
//             {
//                 _uv[3], _uv[2], _uv[0], _uv[1]
//             };
//             
//             mesh.triangles = new int[]
//             {
//                 3, 1, 0, 3, 2, 1
//             };
//
//             return mesh;
//         }
//
//
//     }
// }
