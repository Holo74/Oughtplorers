using Godot;
using System;

public class MakeCurrentRoom : Area
{
    public override void _Ready()
    {
        Connect("body_entered", this, nameof(PlayerEntered));

    }

    public void PlayerEntered(Node body)
    {
        if (body.Name.Equals("Player"))
        {
            WorldManager.instance.SetNewCurrentRoom(GetParent());
        }
    }
}
