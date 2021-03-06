using Godot;
using System;

public class Cutscene : Spatial
{
    [Signal]
    private delegate void CutSceneStart();
    [Signal]
    private delegate void CutSceneEnd();
    [Export]
    private string animationName = "";
    private bool start = false;
    private AnimationPlayer animation;
    [Export]
    private bool autoConnect = true;
    private string worldInfoName { get { return GetParent().Name + animationName; } }
    public override void _Ready()
    {
        if (WorldManager.instance.WorldInfoHas(worldInfoName))
        {
            if (WorldManager.instance.GetWorldInfoData<bool>(worldInfoName))
            {
                QueueFree();
                return;
            }
        }
        animation = GetChild<AnimationPlayer>(1);
        if (autoConnect)
            InGameMenu.Instance.Connect(nameof(InGameMenu.TransitionCamera), this, "StartCamera", null, 4u);
    }

    public void ConnectToCutscene()
    {
        InGameMenu.Instance.Connect(nameof(InGameMenu.TransitionCamera), this, "StartCamera", null, 4u);
    }

    public override void _Process(float delta)
    {
        if (start)
        {
            if (!animation.IsPlaying())
            {
                start = false;
                InGameMenu.Instance.StartCameraTransition();
            }
        }
    }

    public void EndCutscene()
    {
        EmitSignal("CutSceneEnd");
        InGameMenu.Instance.ReadyMenu();
        GameManager.ToggleCutscene();
        QueueFree();
    }

    public void StartCamera()
    {
        EmitSignal("CutSceneStart");
        GameManager.ToggleCutscene();
        animation.Play(animationName);
        start = true;
        InGameMenu.Instance.Connect(nameof(InGameMenu.TransitionCamera), this, "EndCutscene", null, 4u);
    }

    public void PlayerEnter(Node body)
    {
        if (body is PlayerController controller)
        {
            InGameMenu.Instance.StartCameraTransition();
            WorldManager.instance.AddToWorldInfo(worldInfoName, true);
            //GetChild(1).QueueFree();
        }
    }
}
