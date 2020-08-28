using Godot;
using System;

public class FadeToDeath : AILink
{
    public FadeToDeath(MasterController controller) : base(controller) { }
    public override void StartingLink()
    {
        started = true;
        controller.body.CollisionLayer = 0;
    }
    public override void LinkUpdate()
    {
        controller.Scale = controller.Scale.LinearInterpolate(Vector3.Zero, 2 * controller.delta);
        if (controller.Scale.x < .01f)
        {
            controller.QueueFree();
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
