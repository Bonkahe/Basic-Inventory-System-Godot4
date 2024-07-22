using Godot;
using System;

[GlobalClass]
public partial class InventorySlotMono : Control
{
    [Export] public TextureRect IconSlot { get; set; }

    public int InventorySlotID { get; set; } = -1;
    public bool SlotFilled { get; set; } = false;

    public ItemDataMono SlotData { get; set; }

    public void FillSlot(ItemDataMono data)
    {
        SlotData = data;
        SlotFilled = true;
        IconSlot.Texture = data.Icon;
    }
}
