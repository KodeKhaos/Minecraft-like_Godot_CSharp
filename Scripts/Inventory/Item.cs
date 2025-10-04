using Godot;
using Godot.Collections;
using System;

[Tool]
[GlobalClass]
public partial class Item : Resource
{
	[Export] public String ItemName { get; set; }
	[Export] public String ItemType { get; set; }
	[Export] public int MaxStackSize { get; set; }
	[Export] public Texture2D Texture { get; set; }
	[Export] public Dictionary<String, Variant> Attributes { get; set; }

	public Item() // Constructor
	{
		
	}
}
