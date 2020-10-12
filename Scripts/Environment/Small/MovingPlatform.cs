using Godot;
using System;

public class MovingPlatform : Spatial
{
    private Tween tween;
    private bool completedTransition = true;
    [Export]
    private Curve3D path;
    [Export]
    private float timeToComplete = 4f, delay = 1f;
    [Export]
    private Tween.TransitionType transition;
    [Export]
    private Tween.EaseType ease;
    private int pointOn = 0;
    public override void _Ready()
    {
        tween = GetChild<Tween>(0);
    }

    public override void _Process(float delta)
    {
        if (completedTransition)
        {
            completedTransition = false;
            tween.InterpolateProperty(this, "translation", Transform.origin, path.GetPointPosition(pointOn), timeToComplete, transition, ease, delay);
            tween.Start();
            pointOn++;
            if (pointOn >= path.GetPointCount())
                pointOn = 0;
        }
    }

    public void CompleteMove()
    {
        completedTransition = true;
    }
}
