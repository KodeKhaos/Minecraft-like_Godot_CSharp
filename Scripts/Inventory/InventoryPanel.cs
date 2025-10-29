using Godot;
using System;

public partial class InventoryPanel : Panel
{
	public override void _Ready()
	{
		this.MouseEntered += InputEventMouseEnter;
		this.MouseExited += InputEventMouseExit;
		this.GuiInput += InputEventMouseClick;
	}

	public void InputEventMouseEnter()
	{
		var mouse_selector = GetParent().GetNode<MouseSelector>("Mouse Selector");
		mouse_selector.hovered_slot = -1;
	}
	public void InputEventMouseExit()
	{
		
	}

	public void InputEventMouseClick(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			var mouse_selector = GetParent().GetNode<MouseSelector>("Mouse Selector");
		}
	}
}
