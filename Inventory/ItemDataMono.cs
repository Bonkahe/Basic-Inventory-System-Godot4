using Godot;
using System;

[GlobalClass]
public partial class ItemDataMono : Resource
{
    [Export] public string ItemName { get; set; }
    [Export] public PackedScene ItemModelPrefab { get; set; }
}
