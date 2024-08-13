using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class InventorySlotMono : Control
{

    [Signal] public delegate void OnItemEquipedEventHandler(int SlotID);
    [Signal] public delegate void OnItemDroppedEventHandler(int fromSlotID, int toSlotID);

    [Export] public Panel EquippedHighlight { get; set; }
    [Export] public TextureRect IconSlot { get; set; }

    public int InventorySlotID {  get; set; } = -1;
	public bool SlotFilled { get; set; } = false;

	public ItemDataMono SlotData { get; set; }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.DoubleClick)
            {
                EmitSignal(SignalName.OnItemEquiped, InventorySlotID);
            }
        }
    }

    public void FillSlot(ItemDataMono data, bool equipped)
    {
        SlotData = data;
        EquippedHighlight.Visible = equipped;
        if (SlotData != null)
        {
            SlotFilled = true;
            IconSlot.Texture = data.Icon;
        }
        else
        {
            SlotFilled = false;
            IconSlot.Texture = null;
        }
    }

    public override Variant _GetDragData(Vector2 atPosition)
    {
        if (SlotFilled)
        {
            TextureRect preview = new TextureRect();
            preview.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            preview.Size = IconSlot.Size;
            preview.PivotOffset = IconSlot.Size / 2.0f;
            preview.Rotation = 2.0f;
            preview.Texture = IconSlot.Texture;
            SetDragPreview(preview);
            return new Dictionary { { "Type", "Item" }, { "ID", InventorySlotID} };
        }
        else
        {
            return false;
        }
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        return data.VariantType == Variant.Type.Dictionary && (string)data.AsGodotDictionary()["Type"] == "Item";
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        EmitSignal(SignalName.OnItemDropped, (int)data.AsGodotDictionary()["ID"], InventorySlotID);
    }
}
