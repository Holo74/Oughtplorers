using Godot;
using System;

public class Scanner : WeaponBase
{
    public override bool FireGun(Vector3 pos, Basis rot)
    {
        return true;
    }

    public override void HoldingGun(float delta)
    {

    }

    public override void Holstered(float delta)
    {

    }
}
