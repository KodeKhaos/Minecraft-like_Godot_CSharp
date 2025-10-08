using Godot;
using System;

public partial class InventoryPanel : Panel
{
	public void InputEventMouseEnter()
	{
		var mouse_selector = GetParent().GetNode<MouseSelector>("MouseSelector");
		mouse_selector.hovered_slot = -1;
	}
	public void InputEventMouseExit()
	{
		var mouse_selector = GetParent().GetNode<MouseSelector>("MouseSelector");
		mouse_selector.hovered_slot = -2;
	}
}
