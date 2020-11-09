using Godot;
using System;

public class AnimationController : Node
{
    private AnimationTree gunAnimations;
    private Tween cameraZoomTween;
    private PlayerController controller;
    public override void _Ready()
    {
        cameraZoomTween = GetChild<Tween>(0);
        gunAnimations = GetChild<AnimationTree>(1);
        controller = GetParent<PlayerController>();
    }

    public void ZoomCamera(float newValue, float oldValue)
    {
        cameraZoomTween.InterpolateProperty(controller.camera, "fov", oldValue, newValue, .4f,
                    Tween.TransitionType.Cubic, Tween.EaseType.Out);
        cameraZoomTween.InterpolateProperty(controller.gunCamera, "fov", oldValue, newValue, .4f,
        Tween.TransitionType.Cubic, Tween.EaseType.Out);
        cameraZoomTween.Start();
    }
}
