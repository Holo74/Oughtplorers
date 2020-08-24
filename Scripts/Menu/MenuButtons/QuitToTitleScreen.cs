using Godot;
using System;

public class QuitToTitleScreen : Button
{
    public override void _Ready()
    {
        Connect("pressed", GameManager.Instance, nameof(GameManager.QuitToMenu));
    }
}
