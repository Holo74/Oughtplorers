public abstract class AILink
{
    public AILink(MasterController controller)
    {
        this.controller = controller;
    }
    protected bool started = false;
    public bool Started { get { return started; } }
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