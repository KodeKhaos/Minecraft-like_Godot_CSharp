using Godot;
using static TerrainManager;

/*
	Handles the very top layer of terrain generation, converting the top layer of stone to grass and dirt.
*/
public class SurfaceBaseGenerator : ITerrainGeneratorStep
{
	private int _width, _height, _depth;
	private FastNoiseLite _noise;

	public SurfaceBaseGenerator(FastNoiseLite landscapeNoise, int width, int height, int depth)
	{
		_width = width;
		_height = height;
		_depth = depth;
		_noise = landscapeNoise;
	}

	public void Generate(ref TerrainManager.TerrainType[,,] map, Vector2I chunkCoord)
	{
		// Convert top faces of solids to grass/dirt
		for (int x = 0; x < _width; x++)
		{
			for (int z = 0; z < _depth; z++)
			{
				bool foundTop = false;
				bool foundDoubleTop = false;


				int worldX = chunkCoord.X * _width + x;
				int worldZ = chunkCoord.Y * _depth + z;

				for (int y = _height - 1; y >= 0; y--)
				{
					if (map[x, y, z] != TerrainType.Air && map[x, y, z] != TerrainType.Bedrock)
					{
						if (y > ((_noise.GetNoise2D(worldX, worldZ) + 1) * 40f) + 41f && !foundDoubleTop)
						{
							map[x, y, z] = TerrainType.Dirt;
						}
						if (!foundTop)
						{
							map[x, y, z] = TerrainType.Grass;
							foundTop = true;
						}
					}
					else if (foundTop && map[x, y, z] == TerrainType.Air)
					{
						foundDoubleTop = true;
					}
				}
			}
		}
	}

	private bool CheckIfStonelike(TerrainType block)
	{
		if (block == TerrainType.Stone || block == TerrainType.Diorite || block == TerrainType.Granite || block == TerrainType.Andesite || block == TerrainType.Deepslate)
		{
			return false;
		}
		return true;
	}
}

