using Godot;
using System;

public class BossAcidSpew : AILink
{
    [Export]
    private int amountToSpew = 2;
    [Export]
    private float timeBetween = 1;
    private int spewed = 0;
    private float timeLeft = 1;
    private Godot.Collections.Array targetNodes;
    private static Spatial shootFrom;
    public override void StartingLink()
    {
        if (shootFrom == null)
            shootFrom = (Spatial)controller.GetTree().GetNodesInGroup("CeilingHole")[0];
        controller.GlobalTranslate(Vector3.Up * 20);
        spewed = 0;
        timeLeft = timeBetween;
        targetNodes = controller.GetTree().GetNodesInGroup("AcidPools");
    }

    public override void LinkUpdate()
    {
        if (timeLeft > 0)
        {
            timeLeft -= controller.delta;
            if (timeLeft < 0)
            {
                if (amountToSpew <= spewed)
                    return;
                timeLeft = timeBetween;
                Spatial target = (Spatial)targetNodes[spewed];
                shootFrom.LookAt(target.GlobalTransform.origin, Vector3.Up);
                WorldManager.instance.enemyProjectilePool.Pull(shootFrom.GlobalTransform.origin, shootFrom.GlobalTransform.basis);
                target.RemoveFromGroup("AcidPools");
                spewed++;
            }
        }
    }
    public override bool LinkEnd()
    {
        return amountToSpew == spewed && timeLeft < 0;
    }
    public override void Transition()
    {
        controller.UpdateLink((AILink)GetNextNode());
    }
}
