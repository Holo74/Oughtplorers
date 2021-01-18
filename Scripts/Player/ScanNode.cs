using Godot;
using System.Collections.Generic;

public class ScanNode : Spatial
{
    public static ScanNode Instance { get { return instance; } }
    private Camera camera { get { return PlayerController.Instance.camera; } }
    private const float rayLength = 10;
    private int scanPulseCharge = 0;
    private bool do_raycast = false;
    private Vector3 ray_origin = Vector3.Zero;
    private Vector3 ray_end = Vector3.Zero;
    private static ScanNode instance;
    [Signal]
    public delegate void FoundScannable(Scannables scan);
    [Signal]
    public delegate void LostScannables();
    private Scannables current;

    public override void _Ready()
    {
        instance = this;
    }

    // public void ScannablesInSight(Scannables scan)
    // {
    //     if (scan.WasScanned())
    //         return;
    //     scannables.Add(scan);
    //     EmitSignal(nameof(FoundScannable), scan);
    // }

    // public void ScannablesOutOfSight(Scannables scan)
    // {
    //     scannables.Remove(scan);
    //     if (scannables.Count == 0)
    //     {
    //         EmitSignal("LostScannables");
    //     }
    // }

    public bool Scan()
    {
        if (current != null)
        {

            current.Scanned();
            current = null;
            EmitSignal("LostScannables");
            return true;
        }
        return false;
    }

    //This part will ray cast to make sure that the objects are seen by the camera
    public override void _Input(InputEvent @event)
    {
        // if (GameManager.Instance.allowInputs)
        // {
        //     if (@event is InputEventMouseMotion eventMouseMotion)
        //     {
        // ray_origin = camera.ProjectRayOrigin(eventMouseMotion.Position);
        // ray_end = ray_origin + camera.ProjectRayNormal(eventMouseMotion.Position) * rayLength;
        // do_raycast = true;
        //     }
        // }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!GameManager.Instance.playing)
            return;
        if (scanPulseCharge < 10)
        {
            scanPulseCharge++;
        }
        else
        {
            scanPulseCharge = 0;
            ray_origin = camera.ProjectRayOrigin(GetViewport().Size / 2);
            ray_end = ray_origin + camera.ProjectRayNormal(GetViewport().Size / 2) * rayLength;
            do_raycast = true;
        }
        if (do_raycast)
        {
            do_raycast = false;
            PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
            Godot.Collections.Dictionary results = spaceState.IntersectRay(ray_origin, ray_end, null, 256u);
            if (results.Count > 0)
            {
                if (results["collider"] is Spatial spatial)
                {
                    if (spatial.GetParent() is Scannables scan)
                    {
                        if (current != scan)
                        {
                            if (!scan.WasScanned())
                            {
                                current = scan;
                                EmitSignal("FoundScannable", current);
                                return;
                            }
                        }
                        return;
                    }
                }
            }
            if (current != null)
            {
                current = null;
                EmitSignal("LostScannables");
            }

        }
    }

}
