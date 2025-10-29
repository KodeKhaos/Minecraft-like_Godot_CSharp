using Godot;
using System.Collections.Generic;

/*
	Singleton that manages the player's inventory and hotbar.
*/

public partial class InventoryGlobal : Node
{
	[Signal] public delegate void ToggleInventoryEventHandler();
	[Signal] public delegate void RefreshInventoryEventHandler();

	bool inventoryHidden = true;

	public static InventoryGlobal Instance;
	

	// Inventory item is a dictionary with keys: "name" (string) and "amount" (int), or null for empty slots
	public List<Godot.Collections.Dictionary<string, Variant>> Inventory = new()
	{
		null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null,
	};

	public List<Godot.Collections.Dictionary<string, Variant>> HotbarInventory = new()
	{
		null, null, null, null, null, null, null, null, null,
	};
	public override void _Ready()
	{
		Instance = this;

		var possibleItems = GetTree().Root.GetNode<PossibleItems>("Main/ItemManager");
		var fruits = new Godot.Collections.Dictionary<string, Variant>();
		possibleItems.PossibleItemsDict.TryGetValue("Pink Fruit", out var item);
		fruits.Add("item", item);
		fruits.Add("amount", 5);
		Inventory[0] = fruits;
		var pickaxe = new Godot.Collections.Dictionary<string, Variant>();
		possibleItems.PossibleItemsDict.TryGetValue("Iron Pickaxe", out var _item);
		pickaxe.Add("item", _item);
		pickaxe.Add("amount", 1);
		Inventory[1] = pickaxe;
		GD.Print("Initialized inventory with items: ", Inventory);
	}
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("toggle_inventory"))
		{
			EmitSignal(SignalName.ToggleInventory);
			GD.Print("toggled");
			inventoryHidden = !inventoryHidden;
		}

		if (@event.IsActionPressed("exit") && inventoryHidden)
		{
			GD.Print("exited");
			GetTree().Quit();
		}
		else if (@event.IsActionPressed("exit") && !inventoryHidden)
		{
			EmitSignal(SignalName.ToggleInventory);
			GD.Print("toggled");
			inventoryHidden = !inventoryHidden;
		}
	}

	public void AddItem(Godot.Collections.Dictionary<string, Variant> item)
	{
		// TODO: Implement my add item logic here
	}
	public void DropItemFromInventory(Godot.Collections.Dictionary<string, Variant> dragged_item)
	{
		EmitSignal(SignalName.RefreshInventory);
	}
}
