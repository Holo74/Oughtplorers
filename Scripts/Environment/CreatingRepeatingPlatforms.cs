using Godot;
using System;

//I made this because I was lazy and didn't want to create lots of platforms in the debug area
public class CreatingRepeatingPlatforms : Spatial
{
    [Export]
    public string basis;
    [Export]
    public int stepX = 0, stepY = 0, stepZ = 0;
    [Export]
    public int xAmount = 4;

    public override void _Ready()
    {
        PackedScene holder = ResourceLoader.Load(basis) as PackedScene;
        for (int i = 0; i < xAmount; i++)
        {
            Spatial created = holder.Instance() as Spatial;
            AddChild(created);
            created.Translation = new Vector3((i * stepX), (i * stepY), (i * stepZ));
        }
    }
}
