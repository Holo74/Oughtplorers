using Godot;
using System;

public class SlowFall : Spatial
{
    private bool playerInZone = false;
    private PlayerController player;
    private float waitTime = 0f;
    [Export]
    private float duration = .2f;

    public override void _Process(float delta)
    {
        if (playerInZone)
        {
            if (waitTime > 0f)
            {
                waitTime -= delta;
                return;
            }
            if (player.playMovement.GetVerticalMove().y < -1)
            {
                waitTime = duration;
                player.playMovement.AddVer(player.playMovement.GetVerticalMove() * -.1f);
            }
        }
    }



    public void PlayerEntered(Node player)
    {
        waitTime = 0f;
        if (player is PlayerController controller)
        {
            playerInZone = true;
            this.player = controller;
        }
    }

    public void PlayerExit(Node player)
    {
        if (player is PlayerController controller)
        {
            playerInZone = false;
        }
    }
}
