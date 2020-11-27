using Godot;
using System;

public class WeaponController : Camera
{
    private AnimationController anim
    {
        get { return controller.anim; }
    }

    [Signal]
    public delegate void WeaponEquiped(CurrentWeaponEquiped previous, CurrentWeaponEquiped tool);
    [Signal]
    public delegate void WeaponFired(CurrentWeaponEquiped tool);
    public WeaponBase[] tools = new WeaponBase[5];
    public WeaponBase currentTool;
    public CurrentWeaponEquiped currentToolEnum;
    private PlayerController controller;
    private Spatial nCurrentTool;
    public override void _Ready()
    {
        controller = GetParent().GetParent().GetParent<PlayerController>();
        CallDeferred(nameof(FinishingUp));
    }

    public void FinishingUp()
    {
        anim.Connect(nameof(AnimationController.GunAnimationFinished), this, nameof(WeaponAnimationFinished));
    }

    public void WeaponAnimationFinished(string name)
    {
        if (name.Equals("Holstering"))
        {
            nCurrentTool.Scale = Vector3.Zero;
            WeaponHolstered();
        }
    }

    public void EquipWeapon(CurrentWeaponEquiped request)
    {
        if (currentToolEnum == request)
            return;
        anim.PlayGunAnimation("Holstering");
        currentToolEnum = request;
    }

    public void SetCurrentWeapon(CurrentWeaponEquiped set)
    {
        currentToolEnum = set;
        WeaponHolstered();
    }

    public void WeaponHolstered()
    {
        anim.SwapGunAnimationPlayer(currentToolEnum);
        currentTool = tools[(int)currentToolEnum];
        nCurrentTool = GetChild<Spatial>((int)currentToolEnum);
        nCurrentTool.Scale = Vector3.One;
        anim.PlayGunAnimation("Idle");
    }

    public void UseCurrentWeapon(bool force)
    {
        anim.PlayGunAnimation("Writing");
    }
}

public enum CurrentWeaponEquiped
{
    first,
    second,
    third,
    fourth,
    none
}
