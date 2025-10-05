using Godot;
using System;

/*
 * Represents a type of block in the game, including its textures and properties.
 * 
 * To notice:
 * In the future, I might want to add more properties to blocks, such as hardness, light emission, etc.
 * I also want to implement a way to handle different block states (e.g., water flowing, crops growing, etc.).
 */

[Tool]
[GlobalClass]
public partial class Block : Resource
{
	[Export] public Texture2D TextureTop {  get; set; }
	[Export] public Texture2D TextureSide { get; set; }
	[Export] public Texture2D TextureBottom { get; set; }
	[Export] public float BreakTime { get; set; }
	[Export] public bool Transparent { get; set; }

	//[Export] public bool Transparent { get; set; }

	public Texture2D[] Textures => new Texture2D[] { TextureTop, TextureSide, TextureBottom };

	public Block() // Constructor
	{

	}
}
