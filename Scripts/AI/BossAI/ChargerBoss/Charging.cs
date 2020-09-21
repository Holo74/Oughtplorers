using Godot;
using Godot.Collections;

public class Charging : AILink
{
    private static readonly int[] numberSelection = { 7, 2, 3, 1, 0, 6, 4, 5 };
    private static Array nodes;
    private static int currentHoleIndex = 0;
    private int comingFromHole = -1;
    private Spatial target = null;
    [Export]
    private float speed = 5f;
    [Export]
    private float waitPeriod = 2f;
    private bool hit = false;

    public override void StartingLink()
    {
        hit = false;
        if (nodes == null)
            nodes = controller.GetTree().GetNodesInGroup("Targets");
        comingFromHole = numberSelection[currentHoleIndex];
        for (int i = 0; i < 8; i++)
        {
            Array a = new Array();
            a.Add(numberSelection[i]);
            ((Spatial)nodes[numberSelection[i]]).Connect("body_entered", this, nameof(EnterHole), a);
        }
        currentHoleIndex++;
        if (currentHoleIndex > 7)
            currentHoleIndex = 0;
        target = (Spatial)nodes[comingFromHole];
        controller.Transform = new Transform(target.Transform.basis, target.Transform.origin);
        waitPeriod = 2f;
        controller.health.canTakeDamage = false;
        controller.health.RegisterDamageSignal(TakeDamage);
    }

    private void RestartRunning()
    {
        waitPeriod = 2f;
        comingFromHole = numberSelection[currentHoleIndex];
        currentHoleIndex++;
        if (currentHoleIndex > 7)
            currentHoleIndex = 0;
        target = (Spatial)nodes[comingFromHole];
        controller.Transform = new Transform(target.Transform.basis, target.Transform.origin);
    }

    private void DisconnectAreas()
    {
        for (int i = 0; i < 8; i++)
        {
            if (((Spatial)nodes[numberSelection[i]]).IsConnected("body_entered", this, nameof(EnterHole)))
                ((Spatial)nodes[numberSelection[i]]).Disconnect("body_entered", this, nameof(EnterHole));
        }
    }

    public void EnterHole(Node body, int i)
    {
        if (body.GetParent().Name.Equals(controller.Name))
            if (i != comingFromHole)
                RestartRunning();
    }
    public override void LinkUpdate()
    {
        if (hit)
            return;
        if (waitPeriod < 0)
        {
            controller.GlobalTranslate(controller.Transform.basis.x * speed * controller.delta);
        }
        else
            waitPeriod -= controller.delta;
    }
    public override bool LinkEnd()
    {
        return hit;
    }
    public override void Transition()
    {
        controller.health.DeRegisterDamageSignal(TakeDamage);
        controller.UpdateLink((AILink)GetNextNode());
        DisconnectAreas();
        controller.health.canTakeDamage = true;
    }

    private void TakeDamage(float damage, float health, bool damaged)
    {
        hit = true;
    }
}
