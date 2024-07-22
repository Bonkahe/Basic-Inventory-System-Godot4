using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class InventoryHandlerMono : Node
{
    [Export] public int ItemSlotsCount { get; set; } = 20;

    [Export] public GridContainer InventoryGrid { get; set; }
    [Export] public PackedScene InventorySlotPrefab { get; set; }


    List<InventorySlotMono> InventorySlots = new List<InventorySlotMono>();

    public override void _Ready()
	{
        for (int i = 0; i < ItemSlotsCount; i++)
        {
            InventorySlotMono slot = InventorySlotPrefab.Instantiate() as InventorySlotMono;
            InventoryGrid.AddChild(slot);
            slot.InventorySlotID = i;
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
}
