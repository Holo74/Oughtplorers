using Godot;
using System;

[Tool]
public class FlowerLauncher : HealthKinematic
{
    [Export]
    private float strength = 10f;
    [Export]
    private bool updateLine = true;
    private ImmediateGeometry tragectory;
    [Export]
    private bool canShoot = true;
    private bool tookDamage = false;
    [Export]
    private float timerResetTime = 3f;
    private float timer = 1f;
    private Particles particles;

    public override bool TakeDamage(float damage, DamageType typing, Node Source)
    {
        if (!tookDamage)
        {
            tookDamage = true;
            canShoot = !canShoot;
            UpdateColor();
        }
        timer = timerResetTime;
        return true;
    }

    public override void _Ready()
    {
        //Need to make a shader specifically for this so we don't need to duplicate the materials each time as it causes compiling lag when playing
        particles = GetChild<Particles>(2);
        UpdateColor();
        if (Engine.EditorHint)
        {
            tragectory = (ImmediateGeometry)GetTree().GetNodesInGroup("Lines")[0];
        }
    }

    public void HitPlayer(Node body)
    {
        if (canShoot)
        {
            if (body is PlayerController player)
            {
                player.playMovement.SetKnockback(GlobalTransform.basis.y * strength);
            }
        }
    }

    private void UpdateColor()
    {
        particles.Emitting = canShoot;
    }

    public override void _Process(float delta)
    {
        if (tookDamage)
        {
            GD.Print(timer);
            timer -= delta;
            if (timer < 0)
            {
                tookDamage = false;
                canShoot = !canShoot;
                UpdateColor();
            }
        }
        if (Engine.EditorHint)
        {
            if (updateLine)
            {
                updateLine = false;
                tragectory.Clear();
                tragectory.Begin(Mesh.PrimitiveType.LineStrip);
                float time = 0;
                Vector3 vect = (GlobalTransform.basis.y * strength);
                float step = .01f;
                while (time < 2)
                {
                    tragectory.AddVertex(GlobalTransform.origin +
                    new Vector3(
                            vect.x * time,
                            vect.y * time - (PlayerOptions.gravity / 2) * time * time,
                            vect.z * time
                    ));
                    time += step;
                }
                tragectory.End();
            }
        }
    }
}
