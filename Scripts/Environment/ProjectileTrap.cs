using Godot;
using System;

//A very basic trap that is used to test the damage the player but can be modified to server further use
public class ProjectileTrap : StaticBody
{
    private float timer = 0f;
    public override void _Process(float delta)
    {
        timer += delta;
        if (timer > 1)
        {
            timer = 0;
            WorldManager.instance.enemyProjectilePool.Pull(GlobalTransform.origin, GlobalTransform.basis.GetEuler());
        }
    }
}
