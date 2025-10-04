using Godot;
using System.Linq;
using static TerrainManager;

public class UndergroundDecorGenerator : ITerrainGeneratorStep
{
	private int _deepslateLayer, _width, _height, _depth;
	private FastNoiseLite _graniteNoise, _dioriteNoise, _andesiteNoise, _tuffNoise;

	public UndergroundDecorGenerator(int deepslateLayer, FastNoiseLite graniteNoise, FastNoiseLite dioriteNoise, FastNoiseLite andesiteNoise, FastNoiseLite tuffNoise, int width, int height, int depth)
	{
		_width = width;
		_height = height;
		_depth = depth;
		_deepslateLayer = deepslateLayer;
		_graniteNoise = graniteNoise;
		_dioriteNoise = dioriteNoise;
		_andesiteNoise = andesiteNoise;
		_tuffNoise = tuffNoise;
	}

	public void Generate(ref TerrainManager.TerrainType[,,] map, Vector2I chunkCoord)
	{
		for (int x = 0; x < _width; x++)
		{
			for (int z = 0; z < _depth; z++)
			{
				int worldX = chunkCoord.X * _width + x;
				int worldZ = chunkCoord.Y * _depth + z;


				for (int y = 0; y < _height; y++)
				{
					var random = new RandomNumberGenerator();
					if (y == random.RandiRange(1, 2) || y == 0)
					{
						map[x, y, z] = TerrainType.Bedrock;
						continue;
					}
					random = new RandomNumberGenerator();
					if (y <= _deepslateLayer + 4 && map[x, y, z] == TerrainType.Stone)
						if (y < _deepslateLayer)
							map[x, y, z] = TerrainType.Deepslate;
						else if (y <= _deepslateLayer + 2)
							if (random.RandiRange(0, 1) == 0)
								map[x, y, z] = TerrainType.Deepslate;
							else if (random.RandiRange(0, 3) == 0)
								map[x, y, z] = TerrainType.Deepslate;
				}
			}
		}
		for (int x = 0; x < _width; x++)
		{
			for (int z = 0; z < _depth; z++)
			{
				int worldX = chunkCoord.X * _width + x;
				int worldZ = chunkCoord.Y * _depth + z;

				for (int y = 0; y < _height; y++)
				{
					int worldY = y;

					if (map[x, y, z] == TerrainType.Stone || map[x, y, z] == TerrainType.Deepslate)
					{
						var graniteDensity = _graniteNoise.GetNoise3D(worldX, worldY, worldZ);
						var dioriteDensity = _dioriteNoise.GetNoise3D(worldX, worldY, worldZ);
						var andesiteDensity = _andesiteNoise.GetNoise3D(worldX, worldY, worldZ);
						var tuffDensity = _tuffNoise.GetNoise3D(worldX, worldY, worldZ);

						// We do this to fix overlaps in common spots in the world making the most dense material win.
						float[] densityList = { graniteDensity, dioriteDensity, andesiteDensity, tuffDensity };
						switch (densityList.ToList().IndexOf(densityList.Max()))
						{
							case 0:
								// Granite
								if (graniteDensity > 0.9f)
									map[x, y, z] = TerrainType.Granite;
								break;
							case 1:
								// Diorite
								if (dioriteDensity > 0.93f)
									map[x, y, z] = TerrainType.Diorite;
								break;
							case 2:
								// Andesite
								if (andesiteDensity > 0.85f)
									map[x, y, z] = TerrainType.Andesite;
								break;
							case 3:
								// Tuff
								if (y < _deepslateLayer)
									if (tuffDensity > 0.7f)
										map[x, y, z] = TerrainType.Tuff;
								else if (y < _deepslateLayer * 1.34)
									if (tuffDensity > 0.8f)
										map[x, y, z] = TerrainType.Tuff;
								break;
						}
					}
				}
			}
		}
	}
}

