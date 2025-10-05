using Godot;
using System;

/*
	Interface for generation steps

    Nothing to look here... Just a resource.
    You can create more steps by creating a new script that inherits from this one and implements the ITerrainGeneratorStep interface.
	Then, you can add it to the TerrainManager's generationSteps array in the editor.
*/

public partial class ITerrainGenerationStep : Node
{
	
}

// Interface for generation steps
public interface ITerrainGeneratorStep
{
	void Generate(ref TerrainManager.TerrainType[,,] map, Vector2I chunkCoord);
}