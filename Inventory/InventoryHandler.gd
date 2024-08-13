extends Node
class_name InventoryHandler

@export var PlayerBody : CharacterBody3D
@export_flags_3d_physics var CollisionMask : int
 
@export var ItemSlotsCount : int = 20

@export var InventoryGrid : GridContainer
@export var InventorySlotPrefab : PackedScene = preload("res://Inventory/InventorySlot.tscn")

var InventorySlots : Array[InventorySlot] = []

var EquippedSlot : int = -1

func _ready():
	for i in ItemSlotsCount:
		var slot = InventorySlotPrefab.instantiate() as InventorySlot
		InventoryGrid.add_child(slot)
		slot.InventorySlotID = i
		slot.OnItemDropped.connect(ItemDroppedOnSlot.bind())
		slot.OnItemEquiped.connect(ItemEquipped.bind())
		InventorySlots.append(slot)

func PickupItem(item : ItemData):
	var foundSlot : bool = false
	for slot in InventorySlots:
		if (!slot.SlotFilled):
			slot.FillSlot(item, false)
			foundSlot = true
			break
	
	if (!foundSlot):
		var newItem = item.ItemModelPrefab.instantiate() as Node3D
		
		PlayerBody.get_parent().add_child(newItem)
		newItem.global_position = PlayerBody.global_position + PlayerBody.global_transform.basis.x * 2.0

func ItemEquipped(slotID : int):
	if (EquippedSlot != -1):
		InventorySlots[EquippedSlot].FillSlot(InventorySlots[EquippedSlot].SlotData, false)
	
	if (slotID != EquippedSlot && InventorySlots[slotID].SlotData != null):
		InventorySlots[slotID].FillSlot(InventorySlots[slotID].SlotData, true)
		EquippedSlot = slotID
	else:
		EquippedSlot = -1

func ItemDroppedOnSlot(fromSlotID : int, toSlotID : int):
	if EquippedSlot != -1:
		if EquippedSlot == fromSlotID:
			EquippedSlot = toSlotID
		elif EquippedSlot == toSlotID:
			EquippedSlot = fromSlotID
	
	var toSlotItem = InventorySlots[toSlotID].SlotData
	var fromSlotItem = InventorySlots[fromSlotID].SlotData
	
	InventorySlots[toSlotID].FillSlot(fromSlotItem, EquippedSlot == toSlotID)
	InventorySlots[fromSlotID].FillSlot(toSlotItem, EquippedSlot == fromSlotID)

func _can_drop_data(at_position: Vector2, data: Variant) -> bool:
	return typeof(data) == TYPE_DICTIONARY and data["Type"] == "Item"

func _drop_data(at_position: Vector2, data: Variant) -> void:
	if (EquippedSlot == data["ID"]):
		EquippedSlot = -1
	
	var newItem = InventorySlots[data["ID"]].SlotData.ItemModelPrefab.instantiate() as Node3D
	InventorySlots[data["ID"]].FillSlot(null, false)
	
	PlayerBody.get_parent().add_child(newItem)
	newItem.global_position = GetWorldMousePosition()


func GetWorldMousePosition() -> Vector3:
	var mousePos = get_viewport().get_mouse_position()
	var cam = get_viewport().get_camera_3d()
	var ray_start = cam.project_ray_origin(mousePos)
	var ray_end = ray_start + cam.project_ray_normal(mousePos) * cam.global_position.distance_to(PlayerBody.global_position) * 2.0
	var world3d : World3D = PlayerBody.get_world_3d()
	var space_state = world3d.direct_space_state
	
	var query = PhysicsRayQueryParameters3D.create(ray_start, ray_end, CollisionMask)
	
	var results = space_state.intersect_ray(query)
	if (results):
		return results["position"] as Vector3 + Vector3(0.0, 0.5, 0.0)
	else:
		return ray_start.lerp(ray_end, 0.5) + Vector3(0.0, 0.5, 0.0)
	
