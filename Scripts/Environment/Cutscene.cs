using Godot;
using System;

public class Cutscene : Node
{
    [Export]
    private Curve3D curve;
    [Export]
    private float speed = 1f;
    private PathFollow pathF;
    private Camera camera;
    private Spatial lookingAt;
    private Path path;
    [Signal]
    private delegate void CutSceneStart();
    [Signal]
    private delegate void CutSceneEnd();
    private bool start = false;

    public override void _Ready()
    {
        if (WorldManager.instance.WorldInfoHas(GetParent().Name))
        {
            if (WorldManager.instance.GetWorldInfoData<bool>(GetParent().Name))
            {
                QueueFree();
                return;
            }
        }
        InGameMenu.Instance.Connect(nameof(InGameMenu.TransitionCamera), this, "StartCamera", null, 4u);
        path = GetChild<Path>(0);
        path.Curve = curve;
        pathF = path.GetChild<PathFollow>(0);
        camera = pathF.GetChild<Camera>(0);
        lookingAt = ((Spatial)GetTree().GetNodesInGroup("CutscenePoint")[0]);
    }

    public override void _Process(float delta)
    {
        if (start)
        {
            pathF.Offset += delta * speed;
            camera.LookAt(lookingAt.GlobalTransform.origin, Vector3.Up);
            if (pathF.UnitOffset >= 1)
            {
                start = false;
                InGameMenu.Instance.StartCameraTransition();
            }
        }
    }

    public void EndCutscene()
    {
        EmitSignal("CutSceneEnd");
        camera.Current = false;
        InGameMenu.Instance.ReadyMenu();
        GameManager.ToggleCutscene();
        QueueFree();
    }

    public void StartCamera()
    {
        EmitSignal("CutSceneStart");
        GameManager.ToggleCutscene();
        start = true;
        camera.Current = true;
        InGameMenu.Instance.Connect(nameof(InGameMenu.TransitionCamera), this, "EndCutscene", null, 4u);
    }

    public void PlayerEnter(Node body)
    {
        if (body is PlayerController controller)
        {
            InGameMenu.Instance.StartCameraTransition();
            WorldManager.instance.AddToWorldInfo(GetParent().Name, true);
            GetChild(1).QueueFree();
        }
    }
}
