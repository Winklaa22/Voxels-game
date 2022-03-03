using Management.VoxelManagement;
using Management.WorldManagement;
using UnityEngine;

public static class PerlinNoise
{
    public static float GetNoiseMap(Vector3 pos, float scale, float offset)
    {
        var noise = Mathf.PerlinNoise(((pos.x + .1f) / WorldManager.Instance.ChunkWidth * scale + offset), (pos.y + .1f) / WorldManager.Instance.ChunkWidth * scale + offset);
        return noise;
    }
}