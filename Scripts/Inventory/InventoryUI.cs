using Godot;

/*
	InventoryUI is responsible for displaying the player's inventory.
	It listens for toggle events from InventoryGlobal and shows/hides the inventory UI.
	It generates item slots dynamically based on the specified number of columns and rows.
	
	I literally just copy-pasted this from a previous project, so it might need some adjustments.
*/

public partial class InventoryUI : Control
{
	[Export] public GridContainer Grid;
	[Export] public PackedScene ItemSlot;
	[Export] public float ItemSlotSize = 1f;

	public override void _Ready()
	{
		Visible = false;
		InventoryGlobal inventoryGlobal = GetNode<InventoryGlobal>("/root/InventoryGlobal");
		inventoryGlobal.ToggleInventory += _OnToggle;
	}

	private void _OnToggle()
	{
		Visible = !Visible;
		if (Visible)
		{
			GenerateItemSlots(9, 3);
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		else
		{
			foreach (var child in Grid.GetChildren())
			{
				child.QueueFree();
			}
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}

	private void GenerateItemSlots(int columns, int rows)
	{
		Grid.Columns = columns;
		for (int i = 0; i < columns * rows; i++)
		{
			Control instance = (Control)ItemSlot.Instantiate();
			instance.Set("SlotIdx", i);
			instance.Scale = new Vector2(ItemSlotSize, ItemSlotSize);
			Grid.AddChild(instance);
		}
	}
}
