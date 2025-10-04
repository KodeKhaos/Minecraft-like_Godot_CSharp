using Godot;
using static TerrainManager;

public class SurfaceFixerGenerator : ITerrainGeneratorStep
{
	private int _width, _height, _depth;

	public SurfaceFixerGenerator(int width, int height, int depth)
	{
		_width = width;
		_height = height;
		_depth = depth;
	}

	public void Generate(ref TerrainManager.TerrainType[,,] map, Vector2I chunkCoord)
	{
		// Convert top faces of solids to grass/dirt
		for (int x = 0; x < _width; x++)
		{
			for (int z = 0; z < _depth; z++)
			{
				bool foundGrass = false;
				int dirtLayers = 0;

				for (int y = _height - 1; y >= 0; y--)
				{
					if (map[x, y, z] != TerrainType.Air && map[x, y, z] != TerrainType.Bedrock)
					{
						if (map[x,y,z] == TerrainType.Grass)
						{
							foundGrass = true;
						}
						else if (map[x,y,z] == TerrainType.Dirt && foundGrass == false)
						{
							map[x, y, z] = TerrainType.Stone;
							dirtLayers++;
						}
					}
					else if (foundGrass && map[x, y, z] == TerrainType.Air)
					{
						if (dirtLayers == 0)
						{
							map[x, y + 1, z] = TerrainType.Air;
							break;
						}
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

