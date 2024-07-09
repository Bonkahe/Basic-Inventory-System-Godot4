using Godot;
using System;

public partial class InteractableItemMono : Node3D
{
    [Export] public MeshInstance3D ItemHighlightMesh { get; set; }


    public void GainFocus()
    {
        ItemHighlightMesh.Visible = true;
    }

    public void LoseFocus()
    {
        ItemHighlightMesh.Visible = false;
    }
}
