using Godot;
using System.Collections.Generic;

//Used to detect if something is in a very specific location  Used to get normals mostly
public class RayCastData : RayCast
{
    public static Dictionary<RayDirections, RayInfo> SurroundingCasts;
    [Export]
    private RayDirections current;
    [Signal]
    public delegate void ChangeState(bool state);
    public override void _Ready()
    {
        if (SurroundingCasts == null)
        {
            SurroundingCasts = new Dictionary<RayDirections, RayInfo>();
        }
        SurroundingCasts.Add(current, new RayInfo(this));
        Enabled = true;
    }

    public void RegisterStateChange(Godot.Object target, string method)
    {
        Connect(nameof(ChangeState), target, method);
    }

    public override void _PhysicsProcess(float delta)
    {
        bool currentState = IsColliding();
        if (!SurroundingCasts.ContainsKey(current))
            return;
        if (currentState != SurroundingCasts[current].colliding)
            EmitSignal(nameof(ChangeState), currentState);
        SurroundingCasts[current].colliding = currentState;
        if (currentState)
        {
            SurroundingCasts[current].normal = GetCollisionNormal();
            SurroundingCasts[current].hit = (Node)(GetCollider());
        }
    }

    public static void ClearDic()
    {
        SurroundingCasts = new Dictionary<RayDirections, RayInfo>();
    }
}

public enum RayDirections
{
    Bottom,
    Face,
    Top,
    HeadSpace,
    Left,
    Right
}