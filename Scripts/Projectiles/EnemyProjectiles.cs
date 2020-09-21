using Godot;
using System;

//Enemy projectiles that are designed to damage the player and everything else in their layer
public class EnemyProjectiles : Area
{
    private float time = 0;
    [Export]
    private float speed = 1.2f;
    [Export]
    private float damage = 1, maxTime = 5;
    [Export]
    private DamageType damageType;
    private Vector3 direction;
    public override void _EnterTree()
    {
        time = maxTime;
        direction -= Transform.basis.z;
    }

    public override void _Ready()
    {
        Connect("body_entered", this, nameof(HitTarget));
    }

    public void HitTarget(Node body)
    {
        if (body is Health health)
        {
            health.TakeDamage(1f, damageType, this);
        }
        else
        {
            Remove();
        }
    }

    public void Remove()
    {
        QueueFree();
    }

    public override void _Process(float delta)
    {
        time -= delta;
        Translation += direction * delta * speed;
        if (time < 0)
        {
            Remove();
        }
    }
}
