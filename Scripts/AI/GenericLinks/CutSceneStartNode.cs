using Godot;
using System;

public class CutSceneStartNode : AILink
{
    [Export]
    private Resource nextNode;
    private bool cutSceneStart = false, endFlop = false;
    public void CutSceneSwapped(bool b)
    {
        if (b)
        {
            cutSceneStart = true;
        }
        else
        {
            if (cutSceneStart)
                endFlop = true;
        }
    }
    public override void StartingLink()
    {
        GameManager.Instance.Connect(nameof(GameManager.CutSceneSwitch), this, nameof(CutSceneSwapped));
        started = true;
    }
    public override void LinkUpdate()
    {

    }
    public override bool LinkEnd()
    {
        return endFlop;
    }
    public override void Transition()
    {
        controller.UpdateLink((AILink)nextNode);
    }
}
