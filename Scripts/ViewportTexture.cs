using Godot;
using System;

public class ViewportTexture : Control
{
    ShaderMaterial passing;

    public override void _Ready()
    {
        passing = Material as ShaderMaterial;
    }

    public override void _Process(float delta)
    {
        passing.SetShaderParam("ViewportTexture", GetViewport().GetTexture());
    }
}
