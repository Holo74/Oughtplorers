using Godot;
public abstract class AILink : Resource
{
    [Export]
    private Resource nextNode = null;
    [Export]
    protected string resourcePath = "";
    protected Resource GetNextNode()
    {
        if (nextNode != null)
        {
            return nextNode;
        }
        return ResourceLoader.Load(resourcePath);
    }

    public void ConfirmStart()
    {
        started = true;
    }

    public virtual void RestartLink()
    {
        started = false;
    }

    public AILink(MasterController controller)
    {
        this.controller = controller;
    }
    public AILink() { }
    public void AssignController(MasterController controller)
    {
        this.controller = controller;
    }
    protected bool started = false;
    public bool Started { get { return started; } }
    public bool LinkEstablished()
    {
        return controller != null;
    }
    protected MasterController controller;
    public abstract void StartingLink();
    public abstract void LinkUpdate();
    public abstract bool LinkEnd();
    public abstract void Transition();
}

// public override void StartingLink()
// {

// }
// public override void LinkUpdate()
// {

// }
// public override bool LinkEnd()
// {

// }
// public override void Transition()
// {

// }