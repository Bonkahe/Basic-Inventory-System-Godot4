using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class PlayerInteractionHandlerMono : Area3D
{
    [Signal] public delegate void OnItemPickedUpEventHandler(ItemDataMono item);
    [Export] public Array<ItemDataMono> ItemTypes { get; set; } = new Array<ItemDataMono>();

    private List<InteractableItemMono> NearbyBodies = new List<InteractableItemMono>();

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Interact"))
        {
            PickupNearestItem();
        }
    }

    private void PickupNearestItem()
    {
        InteractableItemMono nearestItem = NearbyBodies.OrderBy(x => x.GlobalPosition.DistanceTo(GlobalPosition)).FirstOrDefault();

        if (nearestItem != null)
        {
            nearestItem.QueueFree();
            NearbyBodies.Remove(nearestItem);

            ItemDataMono template = ItemTypes.FirstOrDefault(x => x.ItemModelPrefab.ResourcePath == nearestItem.SceneFilePath);
            if (template != null)
            {
                GD.Print("Item id:" + ItemTypes.IndexOf(template) + " Item Name:" + template.ItemName);
                EmitSignal(SignalName.OnItemPickedUp, template);
            }
            else
            {
                GD.PrintErr("Item not found");
            }
        }
    }


    public void OnObjectEnteredArea(Node3D body)
    {
        if (body is InteractableItemMono item)
        {
            item.GainFocus();

            NearbyBodies.Add(item);
        }
    }

    public void OnObjectExitedArea(Node3D body)
    {
        if (body is InteractableItemMono item && NearbyBodies.Contains(item))
        {
            item.LoseFocus();

            NearbyBodies.Remove(item);
        }
    }
}
