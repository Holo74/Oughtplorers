using Godot;
using System;

public class Knockback : Spatial
{
    [Export]
    private float damage = 10f, strength = 10f;
    [Export]
    private DamageType type = DamageType.debug;
    public void HitPlayer(Node body)
    {
        if (body is PlayerController player)
        {
            player.playMovement.SetKnockback((player.GlobalTransform.origin - GlobalTransform.origin).Normalized() * strength);
            player.TakeDamage(damage, type, this);
        }
    }
}
