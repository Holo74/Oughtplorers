using Godot;
using System;

public class FloorCollapse : Spatial
{
    public void DeleteFloor()
    {
        QueueFree();
    }
}
