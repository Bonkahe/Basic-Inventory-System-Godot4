using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class InventoryHandlerMono : Control
{
	[Export] public int ItemSlotsCount { get; set; } = 20;

	[Export] public CharacterBody3D PlayerBody { get; set; }
	[Export (PropertyHint.Layers3DPhysics)] public uint CollisionMask { get; set; }
    [Export] public GridContainer InventoryGrid { get; set; }
	[Export] public PackedScene  InventorySlotPrefab { get; set; }

	List<InventorySlotMono> InventorySlots = new List<InventorySlotMono>();

    public override void _Ready()
	{
		for (int i = 0; i < ItemSlotsCount; i++)
		{
            InventorySlotMono slot = InventorySlotPrefab.Instantiate() as InventorySlotMono;
			InventoryGrid.AddChild(slot);
			slot.InventorySlotID = i;
            slot.OnItemDropped += ItemDroppedOnSlot;
			InventorySlots.Add(slot);
        }
	}

    public void PickupItem(ItemDataMono item)
	{
		foreach (InventorySlotMono slot in InventorySlots)
		{
			if (!slot.SlotFilled)
			{
				slot.FillSlot(item);
				break;
            }
		}
	}

	public void ItemDroppedOnSlot(int fromSlotID, int toSlotID)
	{
		var toSlotItem = InventorySlots[toSlotID].SlotData;
		var fromSlotItem = InventorySlots[fromSlotID].SlotData;

		InventorySlots[toSlotID].FillSlot(fromSlotItem);
		InventorySlots[fromSlotID].FillSlot(toSlotItem);
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        return data.VariantType == Variant.Type.Dictionary && (string)data.AsGodotDictionary()["Type"] == "Item";
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
		int id = (int)data.AsGodotDictionary()["ID"];
        var newItem = InventorySlots[id].SlotData.ItemModelPrefab.Instantiate() as Node3D;

		InventorySlots[id].FillSlot(null);
		PlayerBody.GetParent().AddChild(newItem);
		newItem.GlobalPosition = GetWorldMousePosition();
    }

	public Vector3 GetWorldMousePosition()
	{
		Vector2 mousePos = GetViewport().GetMousePosition();
		Camera3D cam = GetViewport().GetCamera3D();
        Vector3 ray_start = cam.ProjectRayOrigin(mousePos);
		Vector3 ray_end = ray_start + cam.ProjectRayNormal(mousePos) * cam.GlobalPosition.DistanceTo(PlayerBody.GlobalPosition) * 2.0f;


		World3D world3d = PlayerBody.GetWorld3D();
		PhysicsDirectSpaceState3D space_state = world3d.DirectSpaceState;

		var query = PhysicsRayQueryParameters3D.Create(ray_start, ray_end, CollisionMask);

		var results = space_state.IntersectRay(query);
		if (results.Count > 0) {
			return (Vector3)results["position"] + new Vector3(0.0f, 0.5f, 0.0f);
		}
		else {
			return ray_start.Lerp(ray_end, 0.5f) + new Vector3(0.0f, 0.5f, 0.0f);
		}
    }
}
