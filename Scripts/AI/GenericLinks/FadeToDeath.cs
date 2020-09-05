using Godot;
using System;

public class FadeToDeath : AILink
{
    public FadeToDeath(MasterController controller) : base(controller) { }
    public override void StartingLink()
    {
        started = true;
        controller.health.CollisionLayer = 0;
        controller.health.CollisionMask = 0;
    }
    public override void LinkUpdate()
    {

        if (controller.Scale.x < .01f)
        {
            controller.QueueFree();
        }
        else
        {
            controller.Scale = controller.Scale.LinearInterpolate(Vector3.Zero, 2 * controller.delta);
        }
    }
    public override bool LinkEnd()
    {
        return false;
    }
    public override void Transition()
    {

    }
}
