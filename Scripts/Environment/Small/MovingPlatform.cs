using Godot;
using System;

public class MovingPlatform : Spatial
{
    private Tween tween;
    private bool completedTransition = true;
    [Export]
    private Vector3[] path;
    [Export]
    private float timeToComplete = 4f, delay = 1f;
    [Export]
    private Tween.TransitionType transition;
    [Export]
    private Tween.EaseType ease;
    private int pointOn = 0;
    private Vector3 originalPos;
    public override void _Ready()
    {
        tween = GetChild<Tween>(0);
        originalPos = GlobalTransform.origin;
    }

    public override void _Process(float delta)
    {
        if (completedTransition)
        {
            completedTransition = false;
            float distance = Mathf.Clamp((Transform.origin - (path[pointOn] + originalPos)).Length(), .1f, 100000000);
            tween.InterpolateProperty(this, "translation", Transform.origin, path[pointOn] + originalPos, timeToComplete * distance, transition, ease, delay);
            tween.Start();
            pointOn++;
            if (pointOn >= path.Length)
                pointOn = 0;
        }
    }

    public void CompleteMove()
    {
        completedTransition = true;
    }
}
