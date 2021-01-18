using Godot;
using System;

public class Scannables : Spatial
{
    private RayCast ray;
    private bool seen = false;
    private bool added = false;
    private SpatialMaterial scanMaterial;
    private float scanMaterialFade = 0f;
    [Export]
    private Color transparent, showing, needsScanning;
    [Export]
    private ScanInfoResource itemInfo;

    public string name { get { return AutoLoadScanInfo.AppendToFrontScan + itemInfo.GetScanName(); } }
    public override void _Ready()
    {
        ray = GetChild(0).GetChild<RayCast>(1);
        scanMaterial = GetChild<MeshInstance>(2).GetSurfaceMaterial(0) as SpatialMaterial;
        scanMaterial.AlbedoColor = transparent;
    }

    public void ChangeSeenStatus(bool status)
    {
        seen = status;
    }

    public bool WasScanned()
    {
        if (PlayersJournal.instance.scanNames.Contains(name))
        {
            return true;
        }
        return false;
    }

    public void Scanned()
    {
        if (scanMaterialFade > 0f)
        {
            scanMaterial.AlbedoColor = showing;
        }
        PlayersJournal.instance.AddPageToBook(itemInfo);
    }

    public void ShowScannable()
    {
        if (WasScanned())
        {
            scanMaterial.AlbedoColor = showing;
        }
        else
        {
            scanMaterial.AlbedoColor = needsScanning;
        }
        scanMaterialFade = 5f;
    }

    public ScanInfoResource GetScanInfo()
    {
        return itemInfo;
    }

    public override void _Process(float delta)
    {
        if (scanMaterialFade > 0f)
        {
            scanMaterialFade -= delta;
            if (scanMaterialFade <= 0f)
            {
                scanMaterial.AlbedoColor = transparent;
            }
        }
        // if (!seen)
        // {
        //     if (added)
        //     {
        //         ScanNode.Instance.ScannablesOutOfSight(this);
        //         added = false;
        //     }
        //     return;
        // }
        // ray.LookAt(PlayerController.Instance.GlobalTransform.origin, Vector3.Up);
        // if (ray.IsColliding())
        // {
        //     if (((Spatial)ray.GetCollider()) is PlayerController player)
        //     {
        //         if (!added)
        //         {
        //             ScanNode.Instance.ScannablesInSight(this);
        //             added = true;
        //         }
        //         return;
        //     }
        // }
        // if (added)
        // {
        //     ScanNode.Instance.ScannablesOutOfSight(this);
        //     added = false;
        // }
    }
}
