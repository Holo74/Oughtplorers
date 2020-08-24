using Godot;
using System;

public class QuitButton : Button
{
    public override void _Ready()
    {
        Connect("pressed", GameManager.Instance, nameof(GameManager.QuitGame));
    }
}
