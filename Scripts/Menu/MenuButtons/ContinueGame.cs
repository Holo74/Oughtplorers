using Godot;
using System;

public class ContinueGame : Button
{
    public override void _Ready()
    {
        Connect("pressed", GameManager.Instance, nameof(GameManager.ToggleGamePause));
    }
}
