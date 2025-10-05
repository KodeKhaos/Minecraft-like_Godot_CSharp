using Godot;
using Godot.Collections;
using System;

/* 
 * Item represents a type of item that can be stored in the player's inventory.
 * Each item has a name, type, maximum stack size, texture, and a dictionary of attributes.
 * Inside Attributes, you can store any additional data relevant to the item, such as durability, enchantments, etc.
 */

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
