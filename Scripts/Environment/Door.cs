using Godot;
using System;
using System.Collections.Generic;

//Currently not actually working because the door just kind of vanishes when you shoot it
public class Door : HealthStatic
{
    private string path;
    private float time;
    private AnimationTree tree;
    private bool loaded = false, registered = false, close = false;
    private Vector3 offset, rot;
    public override void _Ready()
    {
        DoorLoadingInfo info = GetParent<DoorLoadingInfo>();
        offset = info.offset;
        rot = info.rot;
        path = info.pathway;
        WorldManager.instance.RegisterSwitchingRoomEvent(RoomSwitched);
        registered = true;
        //GetParent<Spatial>().Translate(Vector3.Up * 4);
        tree = GetChild(1).GetChild<AnimationTree>(0);
        //Here because otherwise the doors in other rooms would show up when rooms load in
        ActivateDoor(info.loadDoor);
        Init(-1);
        tree.Active = true;
    }

    public override void _Process(float delta)
    {
        if (time > 0 && loaded)
        {
            if (time < 2)
            {
                tree.Set("parameters/conditions/Hit", false);
            }
            time -= delta;
            if (time < 0)
            {
                close = true;
            }
        }
        if (close)
        {
            if (PlayerController.Instance.GlobalTransform.origin.DistanceTo(GlobalTransform.origin) > 5)
            {
                tree.Set("parameters/conditions/Time", true);
            }
        }
    }

    private void ActivateDoor(bool t)
    {
        if (t)
        {
            Visible = true;
            loaded = true;
            CollisionLayer = 1;
            CollisionMask = 1;
            tree.Set("parameters/conditions/Time", true);
            tree.Set("parameters/conditions/Hit", false);
            close = true;
        }
        else
        {
            Visible = false;
            loaded = false;
            CollisionLayer = 0;
            CollisionMask = 0;
            tree.Set("parameters/conditions/Time", false);
            tree.Set("parameters/conditions/Hit", true);
            time = 3f;
        }
    }

    public override bool TakeDamage(float damage, DamageType typing, Node source)
    {
        //Currently the I need to figure out how the world is going to be pieced together so the doors will just open in tiny scenes with the animator
        if (!PlayerController.Instance.playerTrappedInRoom)
            WorldManager.instance.LoadArea(path, GetParent().GetParent<Spatial>().GlobalTransform.origin + offset, GetParent().GetChild<Spatial>(0).Rotation + rot, DoorOpening);
        return true;
    }

    public void DoorOpening(bool loaded)
    {
        tree.Set("parameters/conditions/Time", false);
        tree.Set("parameters/conditions/Hit", true);
        time = 3f;
    }

    public override void _ExitTree()
    {
        if (registered && WorldManager.instance != null)
            WorldManager.instance.DeregisterSwitchingRoomEvent(RoomSwitched);
    }

    private void RoomSwitched(Node room)
    {
        if (room.Name.Equals(GetParent().GetParent().Name))
        {
            if (!loaded)
            {
                //GetParent<Spatial>().Translate(Vector3.Down * 4);
                ActivateDoor(true);
            }
        }
        else
        {
            if (loaded)
            {
                //GetParent<Spatial>().Translate(Vector3.Up * 4);
                ActivateDoor(false);
            }
        }
    }
}
