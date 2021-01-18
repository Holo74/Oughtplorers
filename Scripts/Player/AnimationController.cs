using Godot;
using System;

public class AnimationController : Node
{
    private AnimationPlayer gunAnimations;
    private AnimationPlayer deathAnimation;
    private Tween cameraZoomTween;
    private PlayerController controller;
    private RetrieveAnimationPlayer[] retrieveGunAnimation = new RetrieveAnimationPlayer[5];
    [Signal]
    public delegate void GunAnimationFinished(string name);
    public override void _Ready()
    {
        cameraZoomTween = GetChild<Tween>(0);
        deathAnimation = GetChild<AnimationPlayer>(2);
        controller = GetParent<PlayerController>();
        CallDeferred(nameof(FillAnimPlayers));
    }

    private void FillAnimPlayers()
    {
        for (int i = 0; i < 5; i++)
        {
            retrieveGunAnimation[i] = WeaponController.instance.GetChild(i).GetChild<RetrieveAnimationPlayer>(0);
            retrieveGunAnimation[i].GetAnimationPlayer().Connect("animation_finished", this, nameof(FinishGunAnimation));
        }
    }

    public void FinishGunAnimation(string name)
    {
        gunAnimations.CurrentAnimation = "";
        EmitSignal(nameof(GunAnimationFinished), name);
    }

    public void PlayerDied()
    {
        deathAnimation.Play("DeathAnimation");
    }

    public void PlayGunAnimation(string name)
    {
        if (gunAnimations.CurrentAnimation != name)
            gunAnimations.Play(name);
    }

    public void SwapGunAnimationPlayer(CurrentWeaponEquiped toAnim)
    {
        gunAnimations = retrieveGunAnimation[(int)toAnim].GetAnimationPlayer();
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
