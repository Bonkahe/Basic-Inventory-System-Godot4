extends Control
class_name InventorySlot

@export var IconSlot : TextureRect

var InventorySlotID : int = -1
var SlotFilled : bool = false

var SlotData : ItemData

func FillSlot(data : ItemData):
	SlotData = data
	SlotFilled = true
	IconSlot.texture = data.Icon
