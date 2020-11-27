using Godot;
using System;

public class PlayerAnimationController : BaseAttatch
{
    public PlayerAnimationController(PlayerController controller) : base(controller, false)
    {
        controller.AttachToDeath(PlayDeathAnimation);
        //controller.animationNode.Active = true;
        controller.ability.AddToStateChange(PlayerStateAnimations);
        //controller.ability.AddToWeaponChange(WeaponSwapped);
        // controller.ability.AddToFiredWeapon(WeaponShoot);
    }

    private void PlayDeathAnimation()
    {
        //controller.animationNode.Set("parameters/conditions/Died", true);
    }

    private void PlayerStateAnimations(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.walking:
                break;
            case PlayerState.standing:
                break;
            case PlayerState.wallRunning:
                break;
            case PlayerState.slide:
                break;
            case PlayerState.glide:
                break;
            default:
                break;
        }
    }

    private void WeaponSwapped(CurrentWeaponEquiped newWeapon)
    {
        switch (newWeapon)
        {
            case CurrentWeaponEquiped.first:
                break;
            case CurrentWeaponEquiped.second:
                break;
            case CurrentWeaponEquiped.third:
                break;
            case CurrentWeaponEquiped.fourth:
                break;
        }
    }

    private void WeaponShoot(CurrentWeaponEquiped weapon)
    {
        switch (weapon)
        {
            case CurrentWeaponEquiped.first:
                //controller.animationNode.Set("parameters/conditions/ShootPistol", true);
                break;
            case CurrentWeaponEquiped.second:
                break;
            case CurrentWeaponEquiped.third:
                break;
            case CurrentWeaponEquiped.fourth:
                break;
        }
    }

    public void SetAnimationToFalse(string animation)
    {
        //controller.animationNode.Set("parameters/conditions/" + animation, false);
    }
}
