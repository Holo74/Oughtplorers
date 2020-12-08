using Godot;
using System;

public class FirstWeapon : WeaponBase
{
    public override bool FireGun(Vector3 pos, Basis rot)
    {
        if (canFire)
        {
            WorldManager.instance.shots.Pull(pos, rot);
            canFire = false;
            reloadTimer = 2f;
            return true;
        }
        return false;
    }

    public override void Secondary()
    {

    }

    public override void HoldingGun(float delta)
    {
    }

    public override void Holstered(float delta)
    {
        if (!canFire)
        {
            reloadTimer -= delta * 2f;
        }
    }
}
