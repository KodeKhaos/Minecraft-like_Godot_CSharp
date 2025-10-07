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

	public void DropItemFromHotbar(int hotbar_slot, int amount)
	{
		AddItemFromHotbarToWorld(hotbar_slot, amount);
		RemoveItemFromHotbar(hotbar_slot, amount);
		EmitSignal(SignalName.RefreshInventory);
	}

	public void AddItemFromHotbarToWorld(int hotbar_slot, int amount)
	{

	}

	public void RemoveItemFromHotbar(int hotbar_slot, int amount)
	{
		if ((int)HotbarInventory[hotbar_slot]["amount"] > amount)
		{
			int hotbarAmount = (int)HotbarInventory[hotbar_slot]["amount"];
			hotbarAmount -= amount;
			HotbarInventory[hotbar_slot]["amount"] = hotbarAmount;
		}
		else
		{
			HotbarInventory[hotbar_slot] = null;
		}
	}

	public void DropItemFromInventory(int inventory_slot, int amount)
	{
		AddItemFromInventoryToWorld(inventory_slot, amount);
		RemoveItemFromInventory(inventory_slot, amount);
		EmitSignal(SignalName.RefreshInventory);
	}

	public void AddItemFromInventoryToWorld(int inventory_slot, int amount)
	{

	}

	public void RemoveItemFromInventory(int inventory_slot, int amount)
	{
		if ((int)Inventory[inventory_slot]["amount"] > amount)
		{
			int inventoryAmount = (int)Inventory[inventory_slot]["amount"];
			inventoryAmount -= amount;
			Inventory[inventory_slot]["amount"] = inventoryAmount;
		}
		else
		{
			Inventory[inventory_slot] = null;
		}
	}
}
