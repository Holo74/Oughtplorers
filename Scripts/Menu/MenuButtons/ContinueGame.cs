using Godot;
using System;

public class ContinueGame : Button
{
    public override void _Ready()
    {
        Connect("pressed", GameManager.Instance, nameof(GameManager.ToggleGamePause));
        GameManager.Instance.Connect(nameof(GameManager.ToggleGame), this, nameof(ToggleOffWithDeath));
    }

    public void ToggleOffWithDeath(bool playing)
    {
        if (PlayerController.Instance.IsDead())
            Disabled = true;
        else
            Disabled = false;
    }
}
