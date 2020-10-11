using Godot;
using System;

public class ChargerBossHealth : HealthKinematic
{
    public override bool TakeDamage(float damage, DamageType typing, Node Source)
    {
        Damaged(damage);
        Source.QueueFree();
        return canTakeDamage;
    }
}
