using Godot;
using System;

//Ignores everything but the player and things not in the layer
public class PlayerProjectiles : Area
{
    [Export]
    private float MaxTime = 2;
    private float time = 1;
    [Export]
    private float speed = 1.2f;
    [Export]
    private float damage = 1;
    private Vector3 direction;
    public override void _EnterTree()
    {
        time = MaxTime;
        direction -= Transform.basis.z;
    }

    public override void _Ready()
    {
        Connect("body_entered", this, nameof(HitTarget));
    }

    public void HitTarget(Node body)
    {
        if (body.Name == "Player")
            return;
        if (body is Health health)
        {
            health.TakeDamage(1f, DamageType.basic, this);
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
        Translation += direction.Normalized() * delta * speed;
        if (time < 0)
        {
            Remove();
        }
    }
}
