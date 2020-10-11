using Godot;
using System;

public class CutSceneStartNode : AILink
{
    private bool cutSceneStart = false, endFlop = false, toggleAnimation = false;
    [Export]
    private bool playAnimaion = false;
    [Export]
    private string animationName = "", nodeEndName = "";
    public void CutSceneSwapped(bool b)
    {
        if (b)
        {
            cutSceneStart = true;
            if (playAnimaion)
            {
                controller.animation.Set("parameters/conditions/" + animationName, true);
                toggleAnimation = true;
            }
        }
        else
        {
            if (cutSceneStart)
            {
                endFlop = true;
                controller.GlobalTransform = ((Spatial)controller.GetTree().GetNodesInGroup(nodeEndName)[0]).GlobalTransform;
            }

        }
    }
    public override void StartingLink()
    {
        GameManager.Instance.Connect(nameof(GameManager.CutSceneSwitch), this, nameof(CutSceneSwapped));
        started = true;
    }
    public override void LinkUpdate()
    {
        if (toggleAnimation)
        {
            toggleAnimation = false;
            controller.animation.Set("parameters/conditions/" + animationName, false);
        }
    }
    public override bool LinkEnd()
    {
        return endFlop;
    }
    public override void Transition()
    {
        controller.UpdateLink((AILink)GetNextNode());
        GameManager.Instance.Disconnect(nameof(GameManager.CutSceneSwitch), this, nameof(CutSceneSwapped));
    }
}
