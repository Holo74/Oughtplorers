using Godot;

//Mostly a helper tool that is used to store packed scenes and what parent the objects should be with 
public class ObjectPool<T> where T : Spatial
{
    PackedScene creator;
    private Node parent;
    public ObjectPool(string name, Node parent = null)
    {
        creator = ResourceLoader.Load<PackedScene>("res://Resources/SpawnInstances/" + name);
        if (parent != null)
        {
            this.parent = parent;
        }
    }

    public ObjectPool(PackedScene packed, Node parent = null)
    {
        creator = packed;
        if (parent != null)
        {
            this.parent = parent;
        }
    }

    public T Pull()
    {
        if (parent == null)
        {
            parent = GameManager.Instance.Root;
        }
        T holder = (T)creator.Instance();
        GameManager.Instance.Root.AddChild(holder);
        holder.Owner = parent;
        return holder;
    }

    public T Pull(Vector3 pos, Vector3 rot)
    {
        if (parent == null)
        {
            parent = GameManager.Instance.Root;
        }
        T holder = (T)creator.Instance();
        holder.Translation = pos;
        holder.Rotation = rot;
        GameManager.Instance.Root.AddChild(holder);
        holder.Owner = parent;
        return holder;
    }
}
