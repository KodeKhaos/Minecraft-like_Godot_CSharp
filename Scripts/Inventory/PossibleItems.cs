using Godot;
using Godot.Collections;

[Tool]
public partial class PossibleItems : Node
{
	[Export] public Item[] PossibleItemsList;
	[Export] public Dictionary<string, Item> PossibleItemsDict;
	public static PossibleItems Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;

		PossibleItemsDict.Clear();

		foreach (var item in PossibleItemsList)
		{
			PossibleItemsDict.Add(item.ItemName, item);
		}
	}
	//{
	/*["Pink Fruit"] = new Godot.Collections.Dictionary
	{
		["texture"] = GD.Load<Texture2D>("res://assets/Icons/icon31.png"),
		["type"] = "consumable",
		["max_stack_size"] = 64,
		["attributes"] = new Godot.Collections.Dictionary
		{
			["effect"] = "restore hunger",
			["amount"] = 10,
			["duration"] = 0
		}
	},
	["Health Potion I"] = new Godot.Collections.Dictionary
	{
		["texture"] = GD.Load<Texture2D>("res://assets/Icons/icon01.png"),
		["type"] = "consumable",
		["max_stack_size"] = 16,
		["attributes"] = new Godot.Collections.Dictionary
		{
			["effect"] = "heal",
			["amount"] = 25,
			["duration"] = 0
		}
	},
	["Iron Pickaxe"] = new Godot.Collections.Dictionary
	{
		["texture"] = GD.Load<Texture2D>("res://assets/Icons/icon34.png"),
		["type"] = "tool",
		["max_stack_size"] = 1,
		["attributes"] = new Godot.Collections.Dictionary
		{
			["tool_type"] = "pickaxe",
			["durability"] = 50,
			["damage"] = 5,
			["harvest_power"] = 2
		}
	},
	*/
	// ... Add other items here following the same pattern
	// For brevity, you should fill in the rest following this style
	//};
}
