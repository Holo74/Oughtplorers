using Godot;
using System;

//A target that is used to test projectiles in the debug area
public class TestHit : HealthStatic
{
    public override void _Ready()
    {
        Init(-1);
    }

    public override bool TakeDamage(float damage, DamageType typing, Node source)
    {
        InGameMenu.DisplayText("The red cube took " + damage + " damage of type " + typing);
        return true;
    }
}
