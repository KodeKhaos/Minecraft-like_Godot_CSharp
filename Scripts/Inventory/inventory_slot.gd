extends Control

@export var slot_idx : int
@export var itemIcon : Sprite2D
@export var itemQuantity : Label
@export var detailsPanel : ColorRect
@export var usagePanel : ColorRect

var show_hide : bool = true

func _ready():
	InventoryGlobal.toggle_inventory.connect(_on_toggle)
	InventoryGlobal.hide_usage_panels.connect(_on_hide_usage_panels)
	set_slot_data_from_inventory()

func _on_toggle():
	if !get_parent().get_parent().visible:
		queue_free()

func _on_hide_usage_panels():
	if show_hide:
		usagePanel.visible = false
	show_hide = true

func set_slot_data_from_inventory():
	var inventory_item
	if InventoryGlobal.inventory[slot_idx]:
		inventory_item = InventoryGlobal.inventory[slot_idx]
	else:
		clear_slot()
		return
	itemIcon.texture = PossibleItems.possible_items[inventory_item["name"]]["texture"]
	itemQuantity.text = str(inventory_item["amount"])
	detailsPanel.get_child(0).text = inventory_item["name"]
	detailsPanel.get_child(1).text = PossibleItems.possible_items[inventory_item["name"]]["type"]
	
	var item_data = PossibleItems.possible_items.get(inventory_item["name"], null)
	
	if "attributes" in item_data and item_data["attributes"]:
		var attr_text = ""
		for key in item_data["attributes"]:
			# Insert each attribute directly with a newline
			attr_text = attr_text + key.capitalize() + ": " + str(item_data["attributes"][key]) + "\n"
		# Remove the last newline character to prevent an extra blank line
		detailsPanel.get_child(2).text = attr_text.strip_edges()
	else:
		detailsPanel.get_child(2).text = ""  # Empty if no attributes

func clear_slot():
	itemIcon.texture = null
	itemQuantity.text = ""

func item_button_mouse_entered():
	if detailsPanel.get_child(0).text != "" and usagePanel.visible == false:
		detailsPanel.visible = true

func item_button_mouse_exited():
	detailsPanel.visible = false

func _on_item_button_pressed():
	detailsPanel.visible = !detailsPanel.visible
	usagePanel.visible = !usagePanel.visible
	show_hide = false
	InventoryGlobal.hide_usage_panels.emit()
	if detailsPanel.get_child(0).text == "":
		detailsPanel.visible = false
		usagePanel.visible = false

func _on_use_button_pressed():
	print("used")
	usagePanel.visible = true

func _on_drop_button_pressed():
	print("pressed")
	usagePanel.visible = true
