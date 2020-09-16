using Godot;
using System;

public class TinyCreatureStart : AILink
{
    public TinyCreatureStart() { }

    public override void StartingLink()
    {
        controller.health.Init(1);
        started = true;
    }
    public override void LinkUpdate()
    {

    }
    public override bool LinkEnd()
    {
        return controller.health.IsDead();
    }
    public override void Transition()
    {
        controller.animation.Set("parameters/conditions/Dead", true);
        controller.UpdateLink(new FadeToDeath(controller));
    }
}
