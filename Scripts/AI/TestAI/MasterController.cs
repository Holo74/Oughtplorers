using Godot;
public class MasterController : Spatial
{
    private AILink currentLink;
    public override void _Ready()
    {
        currentLink = new TestAI.InitialStartup(this);
        currentLink.StartingLink();
    }

    public override void _Process(float delta)
    {
        if (currentLink == null)
            return;
        currentLink.LinkUpdate();
        if (currentLink.LinkEnd())
        {
            currentLink.Transition();
        }
    }

    public void UpdateLink(AILink link)
    {
        currentLink = link;
    }
}
