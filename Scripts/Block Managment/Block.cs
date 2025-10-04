using Godot;
using System;

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
