using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class TerrainManager : Node
{
	public static TerrainManager Instance { get; private set; }

	public enum TerrainType
	{
		Air,
		Grass,
		Dirt,
		Stone,
		CoalOre,
		CopperOre,
		IronOre,
		GoldOre,
		DiamondOre,
		EmeraldOre,
		DeepslateCoalOre,
		DeepslateCopperOre,
		DeepslateIronOre,
		DeepslateGoldOre,
		DeepslateDiamondOre,
		DeepslateEmeraldOre,
		Deepslate,
		Granite,
		Diorite,
		Andesite,
		Tuff,
		Bedrock
	}

	// Landscape Noises and Other Data
	[Export] public FastNoiseLite LandscapeNoise;

	// Underground Decoration Noises and Other Data
	[Export] public int DeepslateLayer;
	[Export] public FastNoiseLite GraniteNoise;
	[Export] public FastNoiseLite DioriteNoise;
	[Export] public FastNoiseLite AndesiteNoise;
	[Export] public FastNoiseLite TuffNoise;

	// Cave Generation Noises
	[Export] public FastNoiseLite CaveShapeNoise;
	[Export] public FastNoiseLite CaveWormNoise;
	[Export] public FastNoiseLite CaveMaskNoise;

	// Ore Generation Noises
	[Export] public FastNoiseLite CoalOreNoise;
	[Export] public FastNoiseLite IronOreNoise;
	[Export] public FastNoiseLite CopperOreNoise;
	[Export] public FastNoiseLite GoldOreNoise;
	[Export] public FastNoiseLite DiamondOreNoise;
	[Export] public FastNoiseLite EmeraldOreNoise;

	private List<ITerrainGeneratorStep> _generationSteps;

	public override void _Ready()
	{
		Instance = this;

		var oreNoises = new FastNoiseLite[6];
		oreNoises[0] = CoalOreNoise;
		oreNoises[1] = CopperOreNoise;
		oreNoises[2] = IronOreNoise;
		oreNoises[3] = GoldOreNoise;
		oreNoises[4] = DiamondOreNoise;
		oreNoises[5] = EmeraldOreNoise;
		
		// Initialize the list of generation steps in the order they should run
		_generationSteps = new List<ITerrainGeneratorStep>
		{
			new HeightmapGenerator(LandscapeNoise, Chunk.dimensions.X, Chunk.dimensions.Y, Chunk.dimensions.Z),
			new UndergroundDecorGenerator(DeepslateLayer, GraniteNoise, DioriteNoise, AndesiteNoise, TuffNoise, Chunk.dimensions.X, Chunk.dimensions.Y, Chunk.dimensions.Z),
			new OrePlacementGenerator(oreNoises, Chunk.dimensions.X, Chunk.dimensions.Y, Chunk.dimensions.Z),
			new SurfaceBaseGenerator(LandscapeNoise, Chunk.dimensions.X, Chunk.dimensions.Y, Chunk.dimensions.Z),
			new CaveCarvingGenerator(CaveShapeNoise, CaveWormNoise, CaveMaskNoise, Chunk.dimensions.X, Chunk.dimensions.Y, Chunk.dimensions.Z),
			new SurfaceFixerGenerator(Chunk.dimensions.X, Chunk.dimensions.Y, Chunk.dimensions.Z)
			// Add more as you want (like trees, grass, etc)
		};
	}

	public TerrainType[,,] GenerateTerrainMap(Vector2I chunkCoord)
	{
		int width = Chunk.dimensions.X;
		int height = Chunk.dimensions.Y;
		int depth = Chunk.dimensions.Z;

		// Initialize full map to Air
		TerrainType[,,] map = new TerrainType[width, height, depth];
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				for (int z = 0; z < depth; z++)
					map[x, y, z] = TerrainType.Air;

		// Run all generation steps
		foreach (var step in _generationSteps)
			step.Generate(ref map, chunkCoord);

		return map;
	}

	public Block[,,] GetChunkData(Vector2I chunkCoord)
	{
		var terrainMap = GenerateTerrainMap(chunkCoord);
		return ConvertToBlocks(terrainMap);
	}

	private Block[,,] ConvertToBlocks(TerrainType[,,] map)
	{
		int width = Chunk.dimensions.X;
		int height = Chunk.dimensions.Y;
		int depth = Chunk.dimensions.Z;

		Block[,,] blocks = new Block[width, height, depth];

		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				for (int z = 0; z < depth; z++)
					blocks[x, y, z] = TerrainTypeToBlock(map[x, y, z]);

		return blocks;
	}

	private Block TerrainTypeToBlock(TerrainType type)
	{
		switch (type)
		{
			case TerrainType.Grass: return BlockManager.Instance.Grass;
			case TerrainType.Dirt: return BlockManager.Instance.Dirt;
			case TerrainType.Stone: return BlockManager.Instance.Stone;
			case TerrainType.Air: return BlockManager.Instance.Air;
			case TerrainType.Bedrock: return BlockManager.Instance.Bedrock;
			case TerrainType.CoalOre: return BlockManager.Instance.CoalOre;
			case TerrainType.CopperOre: return BlockManager.Instance.CopperOre;
			case TerrainType.IronOre: return BlockManager.Instance.IronOre;
			case TerrainType.GoldOre: return BlockManager.Instance.GoldOre;
			case TerrainType.DiamondOre: return BlockManager.Instance.DiamondOre;
			case TerrainType.EmeraldOre: return BlockManager.Instance.EmeraldOre;
			case TerrainType.DeepslateCoalOre: return BlockManager.Instance.DeepslateCoalOre;
			case TerrainType.DeepslateCopperOre: return BlockManager.Instance.DeepslateCopperOre;
			case TerrainType.DeepslateIronOre: return BlockManager.Instance.DeepslateIronOre;
			case TerrainType.DeepslateGoldOre: return BlockManager.Instance.DeepslateGoldOre;
			case TerrainType.DeepslateDiamondOre: return BlockManager.Instance.DeepslateDiamondOre;
			case TerrainType.DeepslateEmeraldOre: return BlockManager.Instance.DeepslateEmeraldOre;
			case TerrainType.Deepslate: return BlockManager.Instance.Deepslate;
			case TerrainType.Granite: return BlockManager.Instance.Granite;
			case TerrainType.Diorite: return BlockManager.Instance.Diorite;
			case TerrainType.Andesite: return BlockManager.Instance.Andesite;
			case TerrainType.Tuff: return BlockManager.Instance.Tuff;
			default: return BlockManager.Instance.Air;
		}
	}
}