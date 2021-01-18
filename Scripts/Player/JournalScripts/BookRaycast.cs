using Godot;
using System;

public class BookRaycast : Spatial
{
    private Camera camera
    {
        get { return PlayerController.Instance.gunCamera; }
    }
    private const float rayLength = 2;
    private bool do_raycast = false;
    private Vector3 ray_origin = Vector3.Zero;
    private Vector3 ray_end = Vector3.Zero;
    private Tabs currentHighlighted;
    private FlipPages flipPages;
    public override void _Input(InputEvent @event)
    {
        if (!PlayersJournal.instance.ActiveJournal())
            return;
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            ray_origin = camera.ProjectRayOrigin(eventMouseMotion.Position);
            ray_end = ray_origin + camera.ProjectRayNormal(eventMouseMotion.Position) * rayLength;
            do_raycast = true;
        }
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == 1)
        {
            if (currentHighlighted != null)
            {
                if (currentHighlighted.HasInfo())
                    PlayersJournal.instance.SetCurrentTab(currentHighlighted);
            }
            if (flipPages != null)
            {
                flipPages.Clicked();
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (do_raycast == true)
        {
            do_raycast = false;
            PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
            Godot.Collections.Dictionary results = spaceState.IntersectRay(ray_origin, ray_end, null, 512u);
            if (results.Count > 0)
            {
                if (results["collider"] is Spatial spatial)
                {
                    if (spatial.GetParent() is Tabs tabs)
                    {
                        currentHighlighted = tabs;
                        tabs.MouseEnter(true);
                        ResetPageFlip();
                        return;
                    }
                    if (spatial.GetParent() is FlipPages flip)
                    {
                        flipPages = flip;
                        flipPages.Hovering(true);
                        return;
                    }
                }
            }
            if (currentHighlighted != null)
            {
                currentHighlighted.MouseEnter(false);
                currentHighlighted = null;
            }
            ResetPageFlip();
        }
    }

    private void ResetPageFlip()
    {
        if (flipPages != null)
        {
            flipPages.Hovering(false);
            flipPages = null;
        }
    }
}