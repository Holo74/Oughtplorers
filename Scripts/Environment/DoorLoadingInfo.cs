using Godot;
using System;

public class DoorLoadingInfo : Spatial
{
    [Export]
    public string pathway = "";
    [Export]
    public Vector3 offset, rot;
}
