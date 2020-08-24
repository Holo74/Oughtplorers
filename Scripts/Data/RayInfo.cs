using Godot;

///<summary>Stores info for the raycast that is gotten with another script</summary>
public class RayInfo
{
    public RayInfo(RayCastData caster)
    {
        this.caster = caster;
    }
    public Vector3 normal = Vector3.Zero;
    public bool colliding = false;
    public Node hit = null;
    public RayCastData caster;
}
