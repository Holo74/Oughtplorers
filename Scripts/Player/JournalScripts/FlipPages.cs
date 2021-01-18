using Godot;
using System;

public class FlipPages : MeshInstance
{
    [Export]
    private Color highlightedColor, unHighlightedColor;
    private SpatialMaterial color;
    [Export]
    private bool reverse = false;
    public override void _Ready()
    {
        color = GetSurfaceMaterial(0) as SpatialMaterial;
        color.AlbedoColor = unHighlightedColor;
    }

    public void Hovering(bool hover)
    {
        if (hover)
        {
            color.AlbedoColor = highlightedColor;
        }
        else
        {
            color.AlbedoColor = unHighlightedColor;
        }
    }

    public void Clicked()
    {
        PlayersJournal.instance.AdvancePages(reverse);
    }
}
