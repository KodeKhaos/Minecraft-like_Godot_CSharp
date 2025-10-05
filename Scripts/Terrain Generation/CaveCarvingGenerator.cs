using Godot;
using static TerrainManager;

/* 
    Cave Carving Generation Step
    This step carves out caves in the terrain using 3D noise functions.
    It uses a combination of cave shape noise, cave worm noise, and a cave mask to determine where to carve out caves.
    The cave mask ensures that caves are more likely to appear at lower altitudes.
    Cave Shape Noise: Determines the general shape and distribution of caves.
    Cave Worm Noise: Adds additional detail and variation to the caves.
    Cave Mask: Controls the vertical distribution of caves, making them more likely to appear at lower altitudes.
    The parameters for the noise functions can be adjusted to create different styles of caves.
    But please, do not touch the parameters here, unless you know what you are doing. It took ages to find a good balance.
*/

public class CaveCarvingGenerator : ITerrainGeneratorStep
{
    private FastNoiseLite _caveShapeNoise;
    private FastNoiseLite _caveWormNoise;
    private FastNoiseLite _caveMaskNoise;
    private int _width, _height, _depth;

    public CaveCarvingGenerator(FastNoiseLite caveShapeNoise, FastNoiseLite caveWormNoise, FastNoiseLite caveMaskNoise,
                               int width, int height, int depth)
    {
        _caveShapeNoise = caveShapeNoise;
        _caveWormNoise = caveWormNoise;
        _caveMaskNoise = caveMaskNoise;
        _width = width;
        _height = height;
        _depth = depth;
    }

    public void Generate(ref TerrainManager.TerrainType[,,] map, Vector2I chunkCoord)
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                for (int z = 0; z < _depth; z++)
                {
                    int worldX = chunkCoord.X * _width + x;
                    int worldY = y;
                    int worldZ = chunkCoord.Y * _depth + z;

                    float scale = 1.0f;

                    // Block must be valid (Stone or Deepslate)
                    if (map[x, y, z] == TerrainType.Bedrock || map[x, y, z] == TerrainType.Air)
                        continue;

                    float mask = _caveMaskNoise.GetNoise3D(worldX, worldY, worldZ);
                    if (mask < 0.0f + (worldY / _height))
                        continue;

                    float caveValue = _caveShapeNoise.GetNoise3D(worldX, worldY, worldZ);
                    if (caveValue > 0.7f) // lower = more caves
                    {
                        map[x, y, z] = TerrainType.Air;
                    }

                    float density = _caveWormNoise.GetNoise3D(((float)worldX + Mathf.Sin((float)worldZ * 0.01f) * 10f) / scale, worldY / scale, worldZ / scale);
                    if (density > 0.55f && density < 0.8f)
                    {
                        map[x, y, z] = TerrainType.Air;
                    }
                }
            }
        }
    }
}