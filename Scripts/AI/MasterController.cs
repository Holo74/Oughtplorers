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
    [Export]
    private float healthStart = 0;
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
            health.Init(healthStart);
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
        if (!currentLink.LinkEstablished())
            currentLink.AssignController(this);
        if (!currentLink.Started)
        {
            currentLink.StartingLink();
            currentLink.ConfirmStart();
        }
        currentLink.LinkUpdate();
        if (currentLink.LinkEnd())
        {
            currentLink.Transition();
        }
    }

    public void UpdateLink(AILink link)
    {
        currentLink = link;
        currentLink.RestartLink();
    }
}
