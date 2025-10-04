using Godot;
using System;

[Tool]
public partial class Chunk : StaticBody3D
{
	[Export]
	public CollisionShape3D CollisionShape { get; set; }

	[Export]
	public MeshInstance3D MeshInstance { get; set; }

	public static Vector3I dimensions = new Vector3I(16, 128, 16);

	private static readonly Vector3I[] _vertices = new Vector3I[]
		{
			// Cube Vertices
			new Vector3I(0,0,0),
			new Vector3I(1,0,0),
			new Vector3I(0,1,0),
			new Vector3I(1,1,0),
			new Vector3I(0,0,1),
			new Vector3I(1,0,1),
			new Vector3I(0,1,1),
			new Vector3I(1,1,1)
		};

	// Cube faces (using the cube vertices' idx)
	private static readonly int[] _top = new int[] { 2, 3, 7, 6 };
	private static readonly int[] _bottom = new int[] { 0, 4, 5, 1 };
	private static readonly int[] _left = new int[] { 6, 4, 0, 2 };
	private static readonly int[] _right = new int[] { 3, 1, 5, 7 };
	private static readonly int[] _back = new int[] { 7, 5, 4, 6 };
	private static readonly int[] _front = new int[] { 2, 0, 1, 3 };

	private SurfaceTool _surfaceTool = new();

	private Block[,,] _blocks = new Block[dimensions.X, dimensions.Y, dimensions.Z];

	int[,,] _lightMap = new int[dimensions.X, dimensions.Y, dimensions.Z];

	public Vector2I ChunkPosition { get; private set; }

	// Later i could replace this with a NoiseManager or TerrainManager or somethin like that to manage the chunks better adding more detail to them
	[Export]
	public FastNoiseLite Noise { get; set; }

	public void SetChunkPosition(Vector2I position)
	{
		ChunkManager.Instance.UpdateChunkPosition(this, position, ChunkPosition);
		ChunkPosition = position;
		CallDeferred(Node3D.MethodName.SetGlobalPosition, new Vector3(ChunkPosition.X * dimensions.X, 0, ChunkPosition.Y * dimensions.Z));

		GD.Print(position);

		Generate();
		Update();
	}

	public void Generate()
	{
		_blocks = TerrainManager.Instance.GetChunkData(ChunkPosition);
		_lightMap = GetLightmap();
	}

	public void Update()
	{
		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		for (int x = 0; x < dimensions.X; x++)
		{
			for (int y = 0; y < dimensions.Y; y++)
			{
				for (int z = 0; z < dimensions.Z; z++)
				{
					CreateBlockMesh(new Vector3I(x, y, z));
				}
			}
		}

		_surfaceTool.SetMaterial(BlockManager.Instance.ChunkMaterial);
		var mesh = _surfaceTool.Commit();

		MeshInstance.Mesh = mesh;
		CollisionShape.Shape = mesh.CreateTrimeshShape();
	}

	private void CreateBlockMesh(Vector3I blockPosition)
	{
		var block = _blocks[blockPosition.X, blockPosition.Y, blockPosition.Z];

		if (block == BlockManager.Instance.Air) return;

		if (CheckTransparent(blockPosition + Vector3I.Up))
		{
			CreateFaceMesh(_top, blockPosition, block.TextureTop ?? block.TextureSide);
		}

		if (CheckTransparent(blockPosition + Vector3I.Down))
		{
			CreateFaceMesh(_bottom, blockPosition, block.TextureBottom ?? block.TextureSide);
		}

		if (CheckTransparent(blockPosition + Vector3I.Left))
		{
			CreateFaceMesh(_left, blockPosition, block.TextureSide);
		}

		if (CheckTransparent(blockPosition + Vector3I.Right))
		{
			CreateFaceMesh(_right, blockPosition, block.TextureSide);
		}

		if (CheckTransparent(blockPosition + Vector3I.Forward))
		{
			CreateFaceMesh(_front, blockPosition, block.TextureSide);
		}

		if (CheckTransparent(blockPosition + Vector3I.Back))
		{
			CreateFaceMesh(_back, blockPosition, block.TextureSide);
		}
	}

	private void CreateFaceMesh(int[] face, Vector3I blockPosition, Texture2D texture)
	{
		var texturePosition = BlockManager.Instance.GetTextureAtlasPosition(texture);
		var textureAtlasSize = BlockManager.Instance.TextureAtlasSize;

		var uvOffset = texturePosition / textureAtlasSize;
		var uvWidth = 1f / textureAtlasSize.X;
		var uvHeight = 1f / textureAtlasSize.Y;

		var uvA = uvOffset + new Vector2(uvWidth, 0);
		var uvB = uvOffset + new Vector2(uvWidth, uvHeight);
		var uvC = uvOffset + new Vector2(0, uvHeight);
		var uvD = uvOffset + new Vector2(0, 0);

		var a = _vertices[face[0]] + blockPosition;
		var b = _vertices[face[1]] + blockPosition;
		var c = _vertices[face[2]] + blockPosition;
		var d = _vertices[face[3]] + blockPosition;

		var uvTriangle1 = new Vector2[] { uvA, uvB, uvC };
		var uvTriangle2 = new Vector2[] { uvA, uvC, uvD };

		var triangle1 = new Vector3[] { a, b, c };
		var triangle2 = new Vector3[] { a, c, d };

		var normal = ((Vector3)(c - a)).Cross(((Vector3)(b - a))).Normalized();
		var normals = new Vector3[] { normal, normal, normal };

		_surfaceTool.AddTriangleFan(triangle1, uvTriangle1, normals: normals);
		_surfaceTool.AddTriangleFan(triangle2, uvTriangle2, normals: normals);
	}

	private bool CheckTransparent(Vector3I blockPosition)
	{
		if (blockPosition.X < 0 || blockPosition.X >= dimensions.X) return true;
		if (blockPosition.Y < 0 || blockPosition.Y >= dimensions.Y) return true;
		if (blockPosition.Z < 0 || blockPosition.Z >= dimensions.Z) return true;

		return _blocks[blockPosition.X, blockPosition.Y, blockPosition.Z].Transparent;
	}

	public void SetBlock(Vector3I blockPosition, Block block)
	{
		try
		{
			_blocks[blockPosition.X, blockPosition.Y, blockPosition.Z] = block;
		}
		catch
		{
			GD.Print("Error: Cannot build over the maximum build limit");
		}

		Update();
	}

	public Block GetBlock(Vector3I blockPosition)
	{
		return _blocks[blockPosition.X, blockPosition.Y, blockPosition.Z];
	}
	private int[,,] GetLightmap()
	{
		int[,,] map = new int[dimensions.X, dimensions.Y, dimensions.Z];

		for (var x = 0; x < dimensions.X; x++)
		{
			for (var y = 0; y < dimensions.Y; y++)
			{
				for (var z = 0; z < dimensions.Z; z++)
				{
					map[x, y, z] = 15;
				}
			}
		}

		return map;
	}
}
