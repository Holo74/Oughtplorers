using Godot;
using System;

//Shows the state that the player is currently in but shouldn't be in the final build
public class StateShower : Label
{
    public override void _Process(float delta)
    {
        if (PlayerController.Instance != null)
            Text = PlayerController.Instance.ability.GetCurrentState().ToString();
    }
}
