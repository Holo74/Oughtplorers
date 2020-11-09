using Godot;
using System;

public class FalloutOfBounds : Node
{
    Spatial OOBSpoint;
    [Export]
    private float damageDone = 10f;
    public override void _Ready()
    {
        if (!GetTree().HasGroup("OutOfBoundsSpawn"))
        {
            GD.Print("Doesn't have OutOfBoundsSpawn group");
            GetTree().Quit();
        }
        OOBSpoint = (Spatial)GetTree().GetNodesInGroup("OutOfBoundsSpawn")[0];
    }
    public void PlayerFellIn(Node body)
    {
        if (body is PlayerController player)
        {
            player.playMovement.Stop();
            player.SetPosAndRot(OOBSpoint.GlobalTransform);
            if (player.GetHealth() > damageDone + 1)
            {
                player.TakeDamage(damageDone, DamageType.fall, null);
            }
        }
    }
}
