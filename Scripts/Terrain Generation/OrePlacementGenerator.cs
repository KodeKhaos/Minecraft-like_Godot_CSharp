using Godot;
using System;
using System.Text.RegularExpressions;
using static TerrainManager;

/* 
	OrePlacementGenerator places ores in the terrain based on noise values and depth.
	It uses different noise functions for each type of ore to determine their placement.
    
    Do not modify the parameters of the noise functions here, they are perfectly balanced for ore distribution. Unless you know what you are doing.
*/

public class OrePlacementGenerator : ITerrainGeneratorStep
{
	private int _width, _height, _depth;
	public FastNoiseLite _coalOreNoise;
	public FastNoiseLite _ironOreNoise;
	public FastNoiseLite _copperOreNoise;
	public FastNoiseLite _goldOreNoise;
	public FastNoiseLite _diamondOreNoise;
	public FastNoiseLite _emeraldOreNoise;
	public OrePlacementGenerator(FastNoiseLite[] OreNoise, int width, int height, int depth)
	{ 
		_width = width;
		_height = height;
		_depth = depth;

		_coalOreNoise = OreNoise[0];
		_copperOreNoise = OreNoise[1];
		_ironOreNoise = OreNoise[2];
		_goldOreNoise = OreNoise[3];
		_diamondOreNoise = OreNoise[4];
		_emeraldOreNoise = OreNoise[5];
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
					int worldY = y;

					if (map[x, y, z] != TerrainType.Stone && map[x, y, z] != TerrainType.Deepslate)
						continue;

					// Coal
					var _oreDensity = _coalOreNoise.GetNoise3D(worldX, worldY, worldZ);
					if (_oreDensity > 0.76f)
					{
						var random = new RandomNumberGenerator();
						if (worldY > 50 + random.RandiRange(-5, 5))
							if (map[x, y, z] == TerrainType.Stone)
								map[x, y, z] = TerrainType.CoalOre;
							else if (map[x, y, z] == TerrainType.Deepslate)
                                map[x, y, z] = TerrainType.DeepslateCoalOre;
                    }

					// Iron
					_oreDensity = _ironOreNoise.GetNoise3D(worldX, worldY, worldZ);
					if (_oreDensity > 0.86f)
					{
						var random = new RandomNumberGenerator();
						if (worldY > 23 + random.RandiRange(-5, 5))
                            if (map[x, y, z] == TerrainType.Stone)
                                map[x, y, z] = TerrainType.IronOre;
                            else if (map[x, y, z] == TerrainType.Deepslate)
                                map[x, y, z] = TerrainType.DeepslateIronOre;
                    }

					// Copper
					_oreDensity = _copperOreNoise.GetNoise3D(worldX, worldY, worldZ);
					if (_oreDensity > 0.72f)
					{
						var random = new RandomNumberGenerator();
						if (worldY > 40 + random.RandiRange(-5, 5))
                            if (map[x, y, z] == TerrainType.Stone)
                                map[x, y, z] = TerrainType.CopperOre;
                            else if (map[x, y, z] == TerrainType.Deepslate)
                                map[x, y, z] = TerrainType.DeepslateCopperOre;
                    }

                    // Gold
                    _oreDensity = _goldOreNoise.GetNoise3D(worldX, worldY, worldZ);
                    if (_oreDensity > 0.9f)
                    {
                        var random = new RandomNumberGenerator();
                        if (worldY < 55 + random.RandiRange(-5, 5))
                            if (map[x, y, z] == TerrainType.Stone)
                                map[x, y, z] = TerrainType.GoldOre;
                            else if (map[x, y, z] == TerrainType.Deepslate)
                                map[x, y, z] = TerrainType.DeepslateGoldOre;
                    }

                    // Diamond
                    _oreDensity = _diamondOreNoise.GetNoise3D(worldX, worldY, worldZ);
                    if (_oreDensity > 0.922f)
                    {
                        var random = new RandomNumberGenerator();
                        if (worldY < 40 + random.RandiRange(-10, 20))
                            if (map[x, y, z] == TerrainType.Stone)
                                map[x, y, z] = TerrainType.DiamondOre;
                            else if (map[x, y, z] == TerrainType.Deepslate)
                                map[x, y, z] = TerrainType.DeepslateDiamondOre;
                    }
                }
			}
		}
	}
}
