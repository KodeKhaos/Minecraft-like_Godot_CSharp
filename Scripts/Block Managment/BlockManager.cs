using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


/*
 * BlockManager is responsible for managing all block types and their textures.
 * 
 * It stores all the blocks in the game and creates a texture atlas.
 */

[Tool]
public partial class BlockManager : Node
{
	[Export]
	public Block Air { get; set; }

	[Export] 
	public Block Stone { get; set; }

	[Export]
	public Block Dirt { get; set; }

	[Export]
	public Block Grass { get; set; }

	[Export]
	public Block OakLog { get; set; }

	[Export]
	public Block OakPlank { get; set; }

	[Export]
	public Block OakLeaves { get; set; }

	[Export]
	public Block Bedrock { get; set; }

	[Export]
	public Block CoalOre { get; set; }

	[Export]
	public Block CopperOre { get; set; }

	[Export]
	public Block IronOre { get; set; }

	[Export]
	public Block GoldOre { get; set; }

	[Export]
	public Block DiamondOre { get; set; }

	[Export]
	public Block EmeraldOre { get; set; }
	[Export]
	public Block DeepslateCoalOre { get; set; }

	[Export]
	public Block DeepslateCopperOre { get; set; }

	[Export]
	public Block DeepslateIronOre { get; set; }

	[Export]
	public Block DeepslateGoldOre { get; set; }

	[Export]
	public Block DeepslateDiamondOre { get; set; }

	[Export]
	public Block DeepslateEmeraldOre { get; set; }

	[Export]
	public Block Deepslate { get; set; }

	[Export]
	public Block Granite { get; set; }

	[Export]
	public Block Diorite { get; set; }

	[Export]
	public Block Andesite { get; set; }

	[Export]
	public Block Tuff { get; set; }


	private readonly Dictionary<Texture2D, Vector2I> _atlasLookup = new();

	private int _gridWidth = 4;
	private int _gridHeight;

	public Vector2I BlockTextureSize { get; } = new(16, 16);

	public Vector2 TextureAtlasSize { get; private set; }

	public static BlockManager Instance { get; private set; }

	public StandardMaterial3D ChunkMaterial { get; private set; }

	public override void _Ready()
	{
		Instance = this;

		var blockTextures = new Block[] { Air, Stone, Dirt, Grass, OakLog, OakPlank, OakLeaves, Bedrock, CoalOre, CopperOre, IronOre, GoldOre, DiamondOre, EmeraldOre, DeepslateCoalOre, DeepslateCopperOre, DeepslateIronOre, DeepslateGoldOre, DeepslateDiamondOre, DeepslateEmeraldOre, Deepslate, Granite, Diorite, Andesite, Tuff }.SelectMany(block => block.Textures).Where(texture => texture != null).Distinct().ToArray();

		for (int i = 0; i < blockTextures.Length; i++) 
		{
			var texture = blockTextures[i];
			_atlasLookup.Add(texture, new Vector2I(i % _gridWidth, Mathf.FloorToInt(i / _gridWidth)));
		}

		_gridHeight = Mathf.CeilToInt(blockTextures.Length / (float)_gridWidth);
		var image = Image.Create(_gridWidth * BlockTextureSize.X, _gridHeight * BlockTextureSize.Y, false, Image.Format.Rgba8);

		for (var x = 0; x < _gridWidth; x++)
		{
			for (var y = 0; y < _gridHeight; y++)
			{
				var imgIndex = x + y * _gridWidth;

				if (imgIndex >= blockTextures.Length) continue;

				var currentImage = blockTextures[imgIndex].GetImage();
				currentImage.Convert(Image.Format.Rgba8);

				image.BlitRect(currentImage, new Rect2I(Vector2I.Zero, BlockTextureSize), new Vector2I(x, y) * BlockTextureSize);
			}
		}

		var textureAtlas = ImageTexture.CreateFromImage(image);

		ChunkMaterial = new()
		{
			AlbedoTexture = textureAtlas,
			TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest
		};

		ChunkMaterial.Transparency = BaseMaterial3D.TransparencyEnum.AlphaScissor;

		TextureAtlasSize = new Vector2(_gridWidth, _gridHeight);

		GD.Print($"Done loading {blockTextures.Length} images to make {_gridWidth} x {_gridHeight} atlas");
	}

	public Vector2I GetTextureAtlasPosition(Texture2D texture)
	{
		if (texture == null) return Vector2I.Zero;
		else { return _atlasLookup[texture]; }
	}
}
