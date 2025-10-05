using Godot;
using static TerrainManager;

/*
	Handles the generation of the heightmap using noise.
    
    Its the most basic step, and should be called first.
*/
public class HeightmapGenerator : ITerrainGeneratorStep
{
	private FastNoiseLite _landscapeNoise;
	private int _width, _height, _depth;

	public HeightmapGenerator(FastNoiseLite landscapeNoise, int width, int height, int depth)
	{
		_landscapeNoise = landscapeNoise;
		_width = width;
		_height = height;
		_depth = depth;
	}

	public void Generate(ref TerrainManager.TerrainType[,,] map, Vector2I chunkCoord)
	{
		for (int x = 0; x < _width; x++)
		{
			for (int z = 0; z < _depth; z++)
			{
				int worldX = chunkCoord.X * _width + x;
				int worldZ = chunkCoord.Y * _depth + z;

				// Use noise as-is; all parameters set in inspector
				float noiseValue = _landscapeNoise.GetNoise2D(worldX, worldZ);
				int terrainHeight = Mathf.Clamp((int)((noiseValue + 1f) * 40f) + 45, 0, _height - 1);

				for (int y = 0; y < _height; y++)
				{
					if (y <= terrainHeight)
						map[x, y, z] = TerrainType.Stone;
					else
						map[x, y, z] = TerrainType.Air;
				}
			}
		}
	}
}
