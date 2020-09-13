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
    private Path path;
    [Signal]
    private delegate void CutSceneStart();
    private bool start = false;

    public override void _Ready()
    {
        InGameMenu.Instance.Connect(nameof(InGameMenu.TransitionCamera), this, "StartCamera", null, 4u);
        path = GetChild<Path>(0);
        path.Curve = curve;
        pathF = path.GetChild<PathFollow>(0);
        camera = pathF.GetChild<Camera>(0);
    }

    public override void _Process(float delta)
    {
        if (start)
        {
            pathF.UnitOffset += delta * speed;
            if (pathF.UnitOffset >= 1)
            {
                start = false;
                InGameMenu.Instance.StartCameraTransition();
            }
        }
    }

    public void EndCutscene()
    {
        camera.Current = false;
        PlayerController.Instance.camera.Current = true;
        QueueFree();
    }

    public void StartCamera()
    {
        start = true;
        PlayerController.Instance.camera.Current = false;
        camera.Current = true;
        InGameMenu.Instance.Connect(nameof(InGameMenu.TransitionCamera), this, "EndCutscene", null, 4u);
    }

    public void PlayerEnter(Node body)
    {
        if (body is PlayerController controller)
        {
            controller.inputs.ToggleInputAccepting();
            InGameMenu.Instance.StartCameraTransition();
        }
    }
}
