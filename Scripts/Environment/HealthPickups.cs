using Godot;
using System;

public class HealthPickups : Area
{
    [Export]
    private float healAmount = 10f;

    public override void _Ready()
    {
        GetChild<AnimationPlayer>(2).CurrentAnimation = "Floating";
    }

    public void PlayerEnter(Node body)
    {
        if (body is PlayerController player)
        {
            player.Heal(healAmount);
            QueueFree();
        }
    }
}
