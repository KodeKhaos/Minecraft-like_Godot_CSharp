using Godot;
using System;

public partial class ITerrainGenerationStep : Node
{
	
}

// Interface for generation steps
public interface ITerrainGeneratorStep
{
	void Generate(ref TerrainManager.TerrainType[,,] map, Vector2I chunkCoord);
}