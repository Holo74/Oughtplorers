using Godot;
using System;

public class RetrieveAnimationPlayer : Node
{
    [Export]
    private NodePath path;
    private AnimationPlayer animation;

    public override void _Ready()
    {
        animation = GetNode<AnimationPlayer>(path);
    }

    public AnimationPlayer GetAnimationPlayer()
    {
        return animation;
    }
}
