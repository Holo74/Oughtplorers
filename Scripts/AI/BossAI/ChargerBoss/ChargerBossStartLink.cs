using Godot;
using System;

public class ChargerBossStartLink : AILink
{
    public override void StartingLink()
    {
        PlayerController.Instance.playerTrappedInRoom = true;
        started = true;
    }

    public override void LinkUpdate()
    {

    }
    public override bool LinkEnd()
    {
        return true;
    }
    public override void Transition()
    {
        controller.UpdateLink((AILink)GetNextNode());
    }
}
