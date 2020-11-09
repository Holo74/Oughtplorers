using Godot;
using System;

public class SetOOBS : Node
{
    Spatial OOBSpoint, newPoint;
    public override void _Ready()
    {
        OOBSpoint = (Spatial)GetTree().GetNodesInGroup("OutOfBoundsSpawn")[0];
        newPoint = GetChild<Spatial>(1);
    }

    public void PlayerEntered(Node body)
    {
        if (body is PlayerController player)
        {
            OOBSpoint.GlobalTransform = newPoint.GlobalTransform;
        }
    }
}
