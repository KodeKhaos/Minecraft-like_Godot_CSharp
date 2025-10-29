using Godot;
using System;

public partial class InventoryBackground : ColorRect
{
	void OnMouseEntered()
	{
		var mouse_selector = GetParent().GetNode<MouseSelector>("Mouse Selector");
		mouse_selector.hovered_slot = -2;
	}

	void GUIInput(InputEvent @event)
	{
		var mouse_selector = GetParent().GetNode<MouseSelector>("Mouse Selector");
		mouse_selector.hovered_slot = -2;
	}
}
