using Godot;
using Godot.Collections;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* 
 * InventorySlot represents a single slot in the player's inventory.
 */

public partial class InventorySlot : Control
{
	[Export] public int SlotIdx;
	[Export] public Sprite2D ItemIcon;
	[Export] public Label ItemQuantity;
	[Export] public BaseButton ItemButton;
	[Export] public ColorRect DetailsPanel;

	private bool showHide = true;
	InventoryGlobal inventoryGlobal = null;
	MouseSelector mouseSelector = null;
	public override void _Ready()
	{
		mouseSelector = GetTree().Root.GetNode<MouseSelector>("Main/Player/Player UI/Inventory UI/Mouse Selector");
		var possibleItems = GetTree().Root.GetNode<PossibleItems>("Main/ItemManager");
		inventoryGlobal = InventoryGlobal.Instance;
		inventoryGlobal.RefreshInventory += RefreshInventory;
		SetSlotDataFromInventory();
	}

	public void RefreshInventory()
	{
		SetSlotDataFromInventory();
	}
	public void SetSlotDataFromInventory()
	{
		if (InventoryGlobal.Instance.Inventory.Count() < SlotIdx) {
			GD.PrintErr("Slot index: ", SlotIdx, " is out of range (inventory): ", InventoryGlobal.Instance.Inventory.Count());
			return;
		}
		
		if (SlotIdx >= 0 && SlotIdx < inventoryGlobal.Inventory.Count && inventoryGlobal.Inventory[SlotIdx] != null)
		{
			if ((Item)inventoryGlobal.Inventory[SlotIdx]["item"] != null);
			else
			{
				ClearSlot();
				return;
			}
			var inventoryItem = inventoryGlobal.Inventory[SlotIdx];
			inventoryItem.TryGetValue("item", out var tempItem);
			Item item = (Item)tempItem;
			var itemName = "";
			try 
			{
				itemName = item.ItemName;
			}
			catch (Exception e)
			{
				GD.PrintErr("Error getting item name for inventory item at slot ", SlotIdx, ": ", e.Message, " item: ", item, ", inventory item: ", inventoryItem);
				ClearSlot();
				return;
			}


			GD.Print("Inventory Item: ", inventoryItem, " item: ", item, " item name: ", itemName);

			ItemIcon.Texture = (Texture2D)PossibleItems.Instance.PossibleItemsDict[itemName].Texture;
			ItemQuantity.Text = inventoryItem["amount"].ToString();

			if (DetailsPanel.GetChild(0) is Label nameLabel)
				nameLabel.Text = itemName;

			if (DetailsPanel.GetChild(1) is Label typeLabel)
				typeLabel.Text = (String)PossibleItems.Instance.PossibleItemsDict[itemName].ItemType;

			if (DetailsPanel.GetChild(2) is Label attrLabel)
			{
				var itemData = PossibleItems.Instance.PossibleItemsDict[itemName];

				if (itemData.Attributes != null)
				{
					Dictionary attributesVariant = (Dictionary)itemData.Attributes;
					var attributes = attributesVariant;

					if (attributes != null && attributes.Count > 0)
					{
						StringBuilder attrText = new();

						foreach (var key in attributes.Keys)
						{
							attrText.Append($"{key.ToString().Capitalize()}: {attributes[key]}\n");
						}

						attrLabel.Text = attrText.ToString().Trim();
					}
					else
					{
						attrLabel.Text = "";
					}
				}
				else
				{
					attrLabel.Text = "";
				}
			}
		}
		else
		{
			ClearSlot();
		}
	}

	private void ClearSlot()
	{
		ItemIcon.Texture = null;
		ItemQuantity.Text = "";
		foreach (Label child in DetailsPanel.GetChildren())
		{
			child.Text = "";
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (ItemButton.IsHovered())
		{
			mouseSelector.hovered_from_hotbar = false;
			mouseSelector.hovered_slot = SlotIdx;
		}
	}
	public void ItemButtonMouseEntered()
	{
		if (DetailsPanel.GetChild(0) is Label nameLabel && nameLabel.Text != "")
		{
			DetailsPanel.Visible = true;
		}
		DetailsPanel.Visible = true;
	}

	public async Task ItemButtonMouseExited()
	{
		DetailsPanel.Visible = false;
		await MouseExit();
	}

	public async Task MouseExit()
	{
		await ToSignal(GetTree(), "PhysicsFrame");
		mouseSelector.hovered_slot = -1;
	}

	public void ItemButtonPressed(InputEvent @event)
	{
		DetailsPanel.Visible = false;

		if (inventoryGlobal.Inventory[SlotIdx] != null)
		{
			if (@event is InputEventMouseButton mouseEvent && mouseEvent.IsPressed())
			{
				if (!mouseSelector.has_item)
				{
					mouseSelector.fully_dragged = false;
					// Check which button
					switch (mouseEvent.ButtonIndex)
					{
						case MouseButton.Left:
							GD.Print("Left click");
							mouseSelector.SetItemData(SlotIdx, false);
							inventoryGlobal.Inventory[SlotIdx] = null;
							ClearSlot();
							
							RefreshInventory();
							break;
						case MouseButton.Right:
							GD.Print("Right click");
							mouseSelector.SetItemData(SlotIdx, true);
							if ((int)inventoryGlobal.Inventory[SlotIdx]["amount"] > 1)
							{
								inventoryGlobal.Inventory[SlotIdx]["amount"] = Mathf.FloorToInt((float)inventoryGlobal.Inventory[SlotIdx]["amount"] / 2.0);
							}
							else
							{
								inventoryGlobal.Inventory[SlotIdx] = null;
								ClearSlot();
							}
							RefreshInventory();
							break;
					}
				}
			}
		}
	}
}
