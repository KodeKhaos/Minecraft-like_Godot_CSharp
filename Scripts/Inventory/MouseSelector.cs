using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;

public partial class MouseSelector : Control
{

	[Export] public Label QuantityText;
	[Export] public Sprite2D Icon;

	public String item;
	public int amount;
	public bool has_item = false;
	public int item_slot = -1;
	public int hovered_slot = -1;
	public bool item_from_hotbar = false;
	public bool hovered_from_hotbar = false;
	public bool fully_dragged = false;
	public Godot.Collections.Dictionary<String, Variant> dragged_item;

	private PossibleItems possibleItems;

	public override void _Ready()
	{
		possibleItems = GetTree().Root.GetNode<PossibleItems>("Main/ItemManager");
	}
	public override void _PhysicsProcess(double delta)
	{
		if (!((Control)GetParent()).Visible)
		{
			ClearMouseSelector();
		}
		else
		{
			GD.Print("hovered slot: ", hovered_slot, " item_slot: ", item_slot);
		}
			Visible = has_item;
		var mouse_position = GetGlobalMousePosition();
		Position = mouse_position;

		if ((Input.IsActionJustReleased("click") || Input.IsActionJustReleased("right_click") || !((Control)GetParent()).Visible) && fully_dragged) 
		{
			dragged_item = new Godot.Collections.Dictionary<String, Variant>{ ["item"] = item, ["amount"] = amount };
			GD.Print("released click, dragged_item: ", dragged_item);
			HandleDrop();
			InventoryGlobal.Instance.EmitSignal(InventoryGlobal.SignalName.RefreshInventory);
		}
		else if (Input.IsActionJustPressed("click") || Input.IsActionJustPressed("right_click"))
		{
			fully_dragged = true;
			GD.Print("pressed click, fully_dragged: ", fully_dragged);
		}
	}

	private void ClearMouseSelector()
	{
		has_item = false;
		item_slot = -1;
		amount = 0;
		item = "";
	}

	public void SetItemData(int slot_idx, bool half_stack)
	{
		item_slot = slot_idx;
		if (hovered_from_hotbar)
		{
			item = ((Item)InventoryGlobal.Instance.HotbarInventory[slot_idx]["item"]).ItemName;
			if (half_stack)
			{
				amount = Mathf.CeilToInt((float)InventoryGlobal.Instance.HotbarInventory[slot_idx]["amount"] / 2.0);
			}
			else
			{
				amount = (int)InventoryGlobal.Instance.HotbarInventory[slot_idx]["amount"];
			}
		}
		else
		{
			item = ((Item)InventoryGlobal.Instance.Inventory[slot_idx]["item"]).ItemName;
			if (half_stack)
			{
				amount = Mathf.CeilToInt((float)InventoryGlobal.Instance.Inventory[slot_idx]["amount"] / 2.0);
			}
			else
			{
				amount = (int)InventoryGlobal.Instance.Inventory[slot_idx]["amount"];
			}
		}

		has_item = true;
		if (amount > 1)
		{
			QuantityText.Text = amount.ToString();
		}
		else
		{
			QuantityText.Text = "";
		}
		Icon.Texture = possibleItems.PossibleItemsDict[item].Texture;
		item_from_hotbar = hovered_from_hotbar;
	}

	private void DropItem()
	{
		InventoryGlobal.Instance.EmitSignal(InventoryGlobal.SignalName.RefreshInventory);
		ClearMouseSelector();
	}
	private void HandleDrop()
	{
		if (hovered_slot < 0)
		{
			DropItemFromInventory();
			DropItem();
			return;
		}

		if (item_from_hotbar)
		{
			HandleHotbarDrop();
		}
		else
		{
			HandleInventoryDrop();
		}

		ClearMouseSelector();
		DropItem();
	}
	private void DropItemFromInventory()
	{
		if (item_from_hotbar)
		{
			if (InventoryGlobal.Instance.HotbarInventory[item_slot] != null)
			{
				int amount = (int)InventoryGlobal.Instance.HotbarInventory[item_slot]["amount"];
				amount += (int)dragged_item["amount"];
				InventoryGlobal.Instance.HotbarInventory[item_slot]["amount"] = amount;
			}
			else
			{
				InventoryGlobal.Instance.HotbarInventory[item_slot] = dragged_item;
			}
			InventoryGlobal.Instance.DropItemFromHotbar(item_slot, (int)dragged_item["amount"]);
			InventoryGlobal.Instance.HotbarInventory[item_slot] = null;
		}
		else
		{
			if (InventoryGlobal.Instance.Inventory[item_slot] != null)
			{
				int amount = (int)InventoryGlobal.Instance.Inventory[item_slot]["amount"];
				amount += (int)dragged_item["amount"];
				InventoryGlobal.Instance.Inventory[item_slot]["amount"] = amount;
			}
			else
			{
				InventoryGlobal.Instance.Inventory[item_slot] = dragged_item;
			}
			InventoryGlobal.Instance.DropItemFromInventory(item_slot, (int)dragged_item["amount"]);
			InventoryGlobal.Instance.Inventory[item_slot] = null;
		}
	}

	private void HandleHotbarDrop()
	{
		if (hovered_from_hotbar)
		{
			if (item_slot == hovered_slot)
			{
				if (InventoryGlobal.Instance.HotbarInventory[item_slot] != null)
				{
					int amount = (int)InventoryGlobal.Instance.HotbarInventory[item_slot]["amount"];
					amount += (int)dragged_item["amount"];
					InventoryGlobal.Instance.HotbarInventory[item_slot]["amount"] = amount;
				}
				else
				{
					InventoryGlobal.Instance.HotbarInventory[item_slot] = dragged_item;
				}
			}
			else
			{
				MergeOrSwapItems(InventoryGlobal.Instance.HotbarInventory, hovered_slot);
			}
		}
		else
		{
			MergeOrSwapItems(InventoryGlobal.Instance.Inventory, hovered_slot);
		}
	}
	private void HandleInventoryDrop()
	{
		if (hovered_from_hotbar)
		{
			MergeOrSwapItems(InventoryGlobal.Instance.HotbarInventory, hovered_slot);
		}
		else
		{
			if (item_slot == hovered_slot)
			{
				if (InventoryGlobal.Instance.Inventory[item_slot] != null)
				{
					int amount = (int)InventoryGlobal.Instance.Inventory[item_slot]["amount"];
					amount += (int)dragged_item["amount"];
					InventoryGlobal.Instance.Inventory[item_slot]["amount"] = amount;
				}
				else
				{
					InventoryGlobal.Instance.Inventory[item_slot] = dragged_item;
				}
			}
			else
			{
				MergeOrSwapItems(InventoryGlobal.Instance.Inventory, hovered_slot);
			}
		}
	}
	private void MergeOrSwapItems(List<Godot.Collections.Dictionary<string, Variant>> target_inventory, int slot)
	{
		if (target_inventory[slot] != null)
		{
			if ((Item)dragged_item["item"] == (Item)target_inventory[slot]["item"])
			{
				MergeStacks(target_inventory, slot);
				return;
			}
		}

		// Swap items if merging isn't possible
		var temp = target_inventory[slot];
		target_inventory[slot] = dragged_item;
		var temp2 = dragged_item;
		dragged_item = temp;
		if (item_from_hotbar)
		{
			if (InventoryGlobal.Instance.HotbarInventory[item_slot] != null)
			{
				target_inventory[slot] = temp;
				if (hovered_from_hotbar)
				{
					if (InventoryGlobal.Instance.HotbarInventory[hovered_slot] != null)
					{
						InventoryGlobal.Instance.DropItemFromHotbar(hovered_slot, (int)InventoryGlobal.Instance.HotbarInventory[hovered_slot]["amount"]);
					}
					InventoryGlobal.Instance.HotbarInventory[hovered_slot] = temp2;
				}
				else
				{
					if (InventoryGlobal.Instance.Inventory[hovered_slot] != null)
					{
						InventoryGlobal.Instance.DropItemFromInventory(hovered_slot, (int)InventoryGlobal.Instance.Inventory[hovered_slot]["amount"]);
					}
					InventoryGlobal.Instance.Inventory[hovered_slot] = temp2;
				}
				return;
			}
			InventoryGlobal.Instance.HotbarInventory[item_slot] = dragged_item;
		}
		else
		{
			if (InventoryGlobal.Instance.Inventory[item_slot] != null)
			{
				target_inventory[slot] = temp;
				if (hovered_from_hotbar)
				{
					if (InventoryGlobal.Instance.HotbarInventory[hovered_slot] != null)
					{
						InventoryGlobal.Instance.DropItemFromHotbar(hovered_slot, (int)InventoryGlobal.Instance.HotbarInventory[hovered_slot]["amount"]);
					}
					InventoryGlobal.Instance.HotbarInventory[hovered_slot] = temp2;
				}
				else
				{
					if (InventoryGlobal.Instance.Inventory[hovered_slot] != null)
					{
						InventoryGlobal.Instance.DropItemFromInventory(hovered_slot, (int)InventoryGlobal.Instance.Inventory[hovered_slot]["amount"]);
					}
					InventoryGlobal.Instance.Inventory[hovered_slot] = temp2;
				}
				return;
			}
			InventoryGlobal.Instance.Inventory[item_slot] = dragged_item;
		}
	}

	private void MergeStacks(List<Godot.Collections.Dictionary<string, Variant>> target_inventory, int slot)
	{
		var max_stack = possibleItems.PossibleItemsDict[((Item)dragged_item["item"]).ItemName].MaxStackSize;
		var total = (int)dragged_item["amount"] + (int)target_inventory[slot]["amount"];

		if (total <= max_stack)
		{
			target_inventory[slot]["amount"] = total;
		}
		else
		{
			target_inventory[slot]["amount"] = max_stack;
			dragged_item["amount"] = total - max_stack; // Keep the leftover in hand
			if (item_from_hotbar)
			{
				if (InventoryGlobal.Instance.HotbarInventory[item_slot] != null)
				{
					int amount = (int)InventoryGlobal.Instance.HotbarInventory[item_slot]["amount"];
					amount += (int)dragged_item["amount"];
					InventoryGlobal.Instance.HotbarInventory[item_slot]["amount"] = amount;
				}
				else
				{
					InventoryGlobal.Instance.HotbarInventory[item_slot] = dragged_item;
				}
			}
			else
			{
				if (InventoryGlobal.Instance.Inventory[item_slot] != null)
				{
					int amount = (int)InventoryGlobal.Instance.Inventory[item_slot]["amount"];
					amount += (int)dragged_item["amount"];
					InventoryGlobal.Instance.Inventory[item_slot]["amount"] = amount;
				}
				else
				{
					InventoryGlobal.Instance.HotbarInventory[item_slot] = dragged_item;
				}
			}
		}

		// Update the original slot to show the removed half
		if (hovered_from_hotbar)
		{
			InventoryGlobal.Instance.HotbarInventory = target_inventory;
		}
		else
		{
			InventoryGlobal.Instance.Inventory = target_inventory;
		}
	}
}
