using Godot;
public class MasterController : Spatial
{
    [Export]
    private Resource startLink;
    private AILink currentLink;
    [Export]
    private int kinBodyNum = -1, animChildNum = -1, healthChildNum = -1;
    public KinematicBody body;
    public AnimationTree animation;
    public HealthKinematic health;
    public float delta;
    [Signal]
    public delegate void StartUpCreature(MasterController controller);
    public override void _Ready()
    {
        if (kinBodyNum >= 0)
            body = GetChild<KinematicBody>(kinBodyNum);
        if (animChildNum >= 0)
        {
            animation = GetChild<AnimationTree>(animChildNum);
            animation.Active = true;
        }
        if (healthChildNum >= 0)
        {
            health = GetChild<HealthKinematic>(healthChildNum);
        }
        currentLink = (AILink)startLink;
        currentLink.AssignController(this);
        currentLink.StartingLink();
        EmitSignal(nameof(StartUpCreature), this);
    }

    public override void _Process(float delta)
    {
        this.delta = delta;
        if (currentLink == null)
            return;
        if (!currentLink.Started)
            currentLink.StartingLink();
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
