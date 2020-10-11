using Godot;
using System;

public class BossDied : AILink
{
    private Cutscene node;
    public override void StartingLink()
    {
        node = (Cutscene)controller.GetTree().GetNodesInGroup("movableCutscene")[0];
        node.ConnectToCutscene();
        node.GlobalTransform = new Transform(controller.GetParent<Spatial>().GlobalTransform.basis, controller.GetParent<Spatial>().GlobalTransform.origin);
        PlayerController.Instance.playerTrappedInRoom = false;
        controller.QueueFree();
    }
    public override void LinkUpdate()
    {

    }
    public override bool LinkEnd()
    {
        return false;
    }
    public override void Transition()
    {

    }
}
