using Godot;
using System;

public class Scanner : WeaponBase
{
    public override bool FireGun(Vector3 pos, Basis rot)
    {
        if (ScanNode.Instance.Scan())
        {
            PlayerController.Instance.anim.PlayGunAnimation("Writing");
            canFire = false;
            return true;
        }
        return false;
    }
    public override void Secondary()
    {
        foreach (Node scans in GameManager.Instance.GetTree().GetNodesInGroup("Scannables"))
        {
            ((Scannables)scans).ShowScannable();
        }
    }

    public override void HoldingGun(float delta)
    {

    }

    public override void Holstered(float delta)
    {

    }
}
