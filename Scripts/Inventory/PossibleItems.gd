extends Node

const possible_items = {
	"Pink Fruit": {
		"texture": preload("res://assets/Icons/icon31.png"),
		"type": "consumable",
		"max_stack_size": 64,
		"attributes": {
			"effect": "restore hunger",
			"amount": 10,
			"duration": 0  # Instant effect, can be >0 for gradual recovery
		}
	},
	"Health Potion I": {
		"texture": preload("res://assets/Icons/icon01.png"),
		"type": "consumable",
		"max_stack_size": 16,
		"attributes": {
			"effect": "heal",
			"amount": 25,
			"duration": 0
		}
	},
	"Health Potion II": {
		"texture": preload("res://assets/Icons/icon02.png"),
		"type": "consumable",
		"max_stack_size": 16,
		"attributes": {
			"effect": "heal",
			"amount": 75,
			"duration": 0
		}
	},
	"Stamina Potion I": {
		"texture": preload("res://assets/Icons/icon03.png"),
		"type": "consumable",
		"max_stack_size": 16,
		"attributes": {
			"effect": "stamina",
			"amount": 75,
			"duration": 0
		}
	},
	"Potion of Experience": {
	"texture": preload("res://assets/Icons/icon05.png"),
	"type": "consumable",
	"max_stack_size": 32,
	"attributes": {
		"effect": "gives xp boost",
		"amount": 100,
		"duration": 0  # If it was an overtime boost, this could be > 0
		}
	},
	"Potion of Poison": {
	"texture": preload("res://assets/Icons/icon10.png"),
	"type": "consumable",
	"max_stack_size": 8,
	"attributes": {
		"effect": "damage",
		"amount": 9,  # Per each second, for 10 seconds in total makes -90 hp
		"duration": 10  # If it was an overtime boost, this could be > 0
		}
	},
	"Coin": {
	"texture": preload("res://assets/Icons/icon20.png"),
	"type": "economy",
	"max_stack_size": 128,
	"attributes": {
		"value": 1,
		}
	},
	"Gem": {
	"texture": preload("res://assets/Icons/icon21.png"),
	"type": "economy",
	"max_stack_size": 32,
	"attributes": {
		"value": 50,
		}
	},
	"Mushroom": {
		"texture": preload("res://assets/Icons/icon32.png"),
		"type": "consumable",
		"max_stack_size": 64,
		"attributes": {
			"effect": "hunger_restore",
			"amount": 20,
			"duration": 0  # Instant effect, can be >0 for gradual recovery
		}
	},
	# Pickaxes
	"Wooden Pickaxe": {
		"texture": preload("res://assets/Icons/icon33.png"),
		"type": "tool",
		"max_stack_size": 1,  # Tools usually don't stack
		"attributes": {
			"tool_type" : "pickaxe",
			"durability": 25,
			"damage": 2,
			"harvest_power": 1  # Determines what materials it can break
		}
	},
	"Iron Pickaxe": {
		"texture": preload("res://assets/Icons/icon34.png"),
		"type": "tool",
		"max_stack_size": 1,  # Tools usually don't stack
		"attributes": {
			"tool_type" : "pickaxe",
			"durability": 50,
			"damage": 5,
			"harvest_power": 2  # Determines what materials it can break
		}
	},
	"Golden Pickaxe": {
		"texture": preload("res://assets/Icons/icon35.png"),
		"type": "tool",
		"max_stack_size": 1,  # Tools usually don't stack
		"attributes": {
			"tool_type" : "pickaxe",
			"durability": 100,
			"damage": 8,
			"harvest_power": 3  # Determines what materials it can break
		}
	},
	"Diamond Pickaxe": {
		"texture": preload("res://assets/Icons/icon36.png"),
		"type": "tool",
		"max_stack_size": 1,  # Tools usually don't stack
		"attributes": {
			"tool_type" : "pickaxe",
			"durability": 200,
			"damage": 15,
			"harvest_power": 4  # Determines what materials it can break
		}
	},
	# Axes
	"Wooden Axe": {
		"texture": preload("res://assets/Icons/icon37.png"),
		"type": "tool",
		"max_stack_size": 1,
		"attributes": {
			"tool_type": "axe",
			"durability": 25,
			"damage": 6,
			"harvest_power": 1
		}
	},
	"Iron Axe": {
		"texture": preload("res://assets/Icons/icon38.png"),
		"type": "tool",
		"max_stack_size": 1,
		"attributes": {
			"tool_type": "axe",
			"durability": 50,
			"damage": 10,
			"harvest_power": 2
		}
	},
	"Golden Axe": {
		"texture": preload("res://assets/Icons/icon39.png"),
		"type": "tool",
		"max_stack_size": 1,
		"attributes": {
			"tool_type": "axe",
			"durability": 100,
			"damage": 9,
			"harvest_power": 3
		}
	},
	"Diamond Axe": {
		"texture": preload("res://assets/Icons/icon40.png"),
		"type": "tool",
		"max_stack_size": 1,
		"attributes": {
			"tool_type": "axe",
			"durability": 200,
			"damage": 18,
			"harvest_power": 4
		}
	},
	# Shovels
	"Wooden Shovel": {
		"texture": preload("res://assets/Icons/icon41.png"),
		"type": "tool",
		"max_stack_size": 1,
		"attributes": {
			"tool_type": "shovel",
			"durability": 20,
			"damage": 1,
			"harvest_power": 1
		}
	},
	"Iron Shovel": {
		"texture": preload("res://assets/Icons/icon42.png"),
		"type": "tool",
		"max_stack_size": 1,
		"attributes": {
			"tool_type": "shovel",
			"durability": 40,
			"damage": 2,
			"harvest_power": 2
		}
	},
	"Golden Shovel": {
		"texture": preload("res://assets/Icons/icon43.png"),
		"type": "tool",
		"max_stack_size": 1,
		"attributes": {
			"tool_type": "shovel",
			"durability": 80,
			"damage": 3,
			"harvest_power": 3
		}
	},
	"Diamond Shovel": {
		"texture": preload("res://assets/Icons/icon44.png"),
		"type": "tool",
		"max_stack_size": 1,
		"attributes": {
			"tool_type": "shovel",
			"durability": 150,
			"damage": 5,
			"harvest_power": 4
		}
	},

	# Swords
	"Wooden Sword": {
		"texture": preload("res://assets/Icons/icon45.png"),
		"type": "weapon",
		"max_stack_size": 1,
		"attributes": {
			"weapon_type": "sword",
			"durability": 25,
			"damage": 14
		}
	},
	"Iron Sword": {
		"texture": preload("res://assets/Icons/icon46.png"),
		"type": "weapon",
		"max_stack_size": 1,
		"attributes": {
			"weapon_type": "sword",
			"durability": 50,
			"damage": 20
		}
	},
	"Golden Sword": {
		"texture": preload("res://assets/Icons/icon47.png"),
		"type": "weapon",
		"max_stack_size": 1,
		"attributes": {
			"weapon_type": "sword",
			"durability": 120,
			"damage": 40
		}
	},
	"Diamond Sword": {
		"texture": preload("res://assets/Icons/icon48.png"),
		"type": "weapon",
		"max_stack_size": 1,
		"attributes": {
			"tool_type": "sword",
			"durability": 200,
			"damage": 95
		}
	},
		# Helmets
	"Iron Helmet": {
		"texture": preload("res://assets/Icons/icon49.png"),
		"type": "armor",
		"max_stack_size": 1,
		"attributes": {
			"armor_type": "helmet",
			"durability": 100,
			"defense": 2
		}
	},
	"Golden Helmet": {
		"texture": preload("res://assets/Icons/icon50.png"),
		"type": "armor",
		"max_stack_size": 1,
		"attributes": {
			"armor_type": "helmet",
			"durability": 150,
			"defense": 3
		}
	},
	"Diamond Helmet": {
		"texture": preload("res://assets/Icons/icon51.png"),
		"type": "armor",
		"max_stack_size": 1,
		"attributes": {
			"armor_type": "helmet",
			"durability": 250,
			"defense": 5
		}
	},

	# Chestplates
	"Iron Chestplate": {
		"texture": preload("res://assets/Icons/icon52.png"),
		"type": "armor",
		"max_stack_size": 1,
		"attributes": {
			"armor_type": "chestplate",
			"durability": 200,
			"defense": 5
		}
	},
	"Golden Chestplate": {
		"texture": preload("res://assets/Icons/icon53.png"),
		"type": "armor",
		"max_stack_size": 1,
		"attributes": {
			"armor_type": "chestplate",
			"durability": 250,
			"defense": 7
		}
	},
	"Diamond Chestplate": {
		"texture": preload("res://assets/Icons/icon54.png"),
		"type": "armor",
		"max_stack_size": 1,
		"attributes": {
			"armor_type": "chestplate",
			"durability": 400,
			"defense": 10
		}
	},

	# Shields
	"Shield": {
		"texture": preload("res://assets/Icons/icon55.png"),
		"type": "armor",
		"max_stack_size": 1,
		"attributes": {
			"armor_type": "shield",
			"durability": 250,
			"defense": 8
		}
	},
}
