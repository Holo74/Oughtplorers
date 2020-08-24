using Godot;
using System;

//Used to display the speed of the player but shouldn't be in the final game
public class SpeedDisplay : Label
{
    public override void _Process(float delta)
    {
        Text = "Speed: " + (PlayerController.Instance.playMovement.GetCurrentSpeed() * PlayerController.Instance.playMovement.GetAccelerate());
    }
}
