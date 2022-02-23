using Management.VoxelManagement;
using UnityEngine;

public static class PerlinNoise
{
    public static float GetNoiseMap(Vector3 pos, float scale, float offset)
    {
        var noise = Mathf.PerlinNoise(((pos.x + .1f) / VoxelData.ChunkSize[0] * scale + offset), (pos.y + .1f) / VoxelData.ChunkSize[0] * scale + offset);
        return noise;
    }
}