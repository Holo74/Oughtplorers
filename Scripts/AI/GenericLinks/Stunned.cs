using Godot;
using System;

public class Stunned : AILink
{
    [Export]
    private Resource hitNode;
    [Export]
    private string hitNodePath = "";
    [Export]
    private float waitingTime = 5f;
    private float currentTime = 0;
    private bool hit = false;
    private void TookDamage(float damage, float health, bool d)
    {
        if (d)
            hit = true;
    }
    public override void StartingLink()
    {
        currentTime = waitingTime;
        hit = false;
        controller.health.RegisterDamageSignal(TookDamage);
    }
    public override void LinkUpdate()
    {
        if (currentTime >= 0)
        {
            GD.Print(currentTime);
            currentTime -= controller.delta;
        }
    }
    public override bool LinkEnd()
    {
        return currentTime < 0 || hit;
    }
    public override void Transition()
    {
        controller.health.DeRegisterDamageSignal(TookDamage);
        if (hit)
        {
            if (hitNode != null)
                controller.UpdateLink((AILink)hitNode);
            else
                controller.UpdateLink((AILink)ResourceLoader.Load(hitNodePath));
        }
        else
        {
            controller.UpdateLink((AILink)GetNextNode());
        }
    }
}
