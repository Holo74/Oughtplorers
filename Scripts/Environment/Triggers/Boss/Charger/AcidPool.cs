using Godot;
using System;

public class AcidPool : HealthKinematic
{
    private bool activatedPool = false, finishedMoving = false;
    private Spatial acid;
    public override void _Ready()
    {
        acid = GetChild<Spatial>(1);
    }

    public override void _Process(float delta)
    {
        if (!finishedMoving && activatedPool)
        {
            acid.Translation = acid.Translation.MoveToward(Vector3.Zero, delta);
            if (acid.Translation.DistanceTo(Vector3.Zero) < .01f)
            {
                finishedMoving = true;
                acid.Translation = Vector3.Zero;
                GD.Print("Finished moving acid");
            }
        }
    }
    public override bool TakeDamage(float damage, DamageType typing, Node Source)
    {
        GD.Print("Took damage");
        if (!activatedPool)
        {
            activatedPool = true;
        }
        return true;
    }
}
