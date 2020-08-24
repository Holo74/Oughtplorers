using Godot;
using System;

///<summary>An interface that is used to make objects take damage from projectiles</summary>
public interface Health
{
    bool TakeDamage(float amount, DamageType type, Node source);
}
