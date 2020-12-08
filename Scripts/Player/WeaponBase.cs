using Godot;
using System;

public abstract class WeaponBase
{
    public bool Equiped;
    protected float reloadTimer;
    protected bool canFire = true;

    public void UpdateGun(float delta)
    {
        if (Equiped)
        {
            HoldingGun(delta);
        }
        else
        {
            Holstered(delta);
        }
        if (reloadTimer < 0 && !canFire)
        {
            canFire = true;
        }
    }

    public abstract bool FireGun(Vector3 pos, Basis rot);
    public abstract void Secondary();

    public abstract void HoldingGun(float delta);

    public abstract void Holstered(float delta);

    public void ReadyGun()
    {
        canFire = true;
    }

    public bool WeaponReady()
    {
        return canFire;
    }
}
