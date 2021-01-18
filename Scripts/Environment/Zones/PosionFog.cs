using Godot;
using System;

//Used to game the player but can be modified to kill anything in it
public class PosionFog : Area
{
    [Export]
    private float damageAmount = 1f;
    [Export]
    private DamageType damageType = DamageType.posion;
    private PlayerController player;
    public override void _Ready()
    {
        Connect("body_entered", this, nameof(PosionPlayer));
        Connect("body_exited", this, nameof(PlayerLeaves));
    }

    public override void _Process(float delta)
    {
        if (player != null)
        {
            player.TakeDamage(damageAmount * delta, damageType, null);
        }
    }

    public void PosionPlayer(Node body)
    {
        if (body.Name == "Player")
        {
            player = (PlayerController)body;
        }
    }

    public void PlayerLeaves(Node body)
    {
        if (body.Name == "Player")
        {
            player = null;
        }
    }
}
