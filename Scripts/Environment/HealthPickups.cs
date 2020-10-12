using Godot;
using System;

public class HealthPickups : Area
{
    [Export]
    private float healAmount = 10f;

    public void PlayerEnter(Node body)
    {
        if (body is PlayerController player)
        {
            player.Heal(healAmount);
            QueueFree();
        }
    }
}
