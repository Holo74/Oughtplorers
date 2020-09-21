using Godot;
using System;

public class LookAtObject : Spatial
{
    [Export]
    private NodePath path;
    private Spatial lookingAtThis;
    public override void _Ready()
    {
        lookingAtThis = GetNode<Spatial>(path);
    }

    public override void _Process(float delta)
    {
        LookAt(lookingAtThis.GlobalTransform.origin, Vector3.Up);
    }
}
