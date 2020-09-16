using Godot;
using System;

public class ChargerBossStartLink : AILink
{
    public override void StartingLink()
    {
        started = true;
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
