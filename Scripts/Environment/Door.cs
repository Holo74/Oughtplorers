using Godot;
using System;
using System.Collections.Generic;

//Currently not actually working because the door just kind of vanishes when you shoot it
public class Door : HealthStatic
{
    private string path;
    private float time;
    private AnimationTree tree;
    private bool loaded = false, registered = false;
    private Vector3 offset, rot;
    public override void _Ready()
    {
        DoorLoadingInfo info = GetParent<DoorLoadingInfo>();
        offset = info.offset;
        rot = info.rot;
        path = info.pathway;
        WorldManager.instance.RegisterSwitchingRoomEvent(RoomSwitched);
        registered = true;
        GetParent<Spatial>().Translate(Vector3.Up * 4);
        loaded = false;
        Init(-1);
        tree = GetChild(1).GetChild<AnimationTree>(0);
        tree.Active = true;
    }

    public override void _Process(float delta)
    {
        if (time > 0)
        {
            if (time < 2)
            {
                tree.Set("parameters/conditions/Hit", false);
            }
            time -= delta;
            if (time < 0)
            {
                tree.Set("parameters/conditions/Time", true);
            }
        }
    }

    public override bool TakeDamage(float damage, DamageType typing, Node source)
    {
        //Currently the I need to figure out how the world is going to be pieced together so the doors will just open in tiny scenes with the animator
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
                loaded = true;
                GetParent<Spatial>().Translate(Vector3.Down * 4);
            }
        }
        else
        {
            if (loaded)
            {
                GetParent<Spatial>().Translate(Vector3.Up * 4);
                loaded = false;
            }
        }
    }
}
