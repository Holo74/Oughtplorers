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
    private WeaponBase[] tools = new WeaponBase[5];
    private WeaponBase currentTool;
    private CurrentWeaponEquiped currentToolEnum;
    private PlayerController controller;
    private Spatial nCurrentTool;
    public override void _Ready()
    {
        controller = GetParent().GetParent().GetParent<PlayerController>();
        tools[0] = new FirstWeapon();
        tools[4] = new Scanner();
        CallDeferred(nameof(FinishingUp));
    }

    public void FinishingUp()
    {
        anim.Connect(nameof(AnimationController.GunAnimationFinished), this, nameof(WeaponAnimationFinished));
    }

    public WeaponBase CurrentWeapon()
    {
        return currentTool;
    }

    public void WeaponAnimationFinished(string name)
    {
        switch (name)
        {
            case "Holstering":
                nCurrentTool.Scale = Vector3.Zero;
                WeaponHolstered();
                break;
            case "Writing":
            case "Equiping":
                anim.PlayGunAnimation("Idle");
                break;
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
        anim.PlayGunAnimation("Equiping");
    }

    public void UseCurrentWeapon(bool force = false)
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
