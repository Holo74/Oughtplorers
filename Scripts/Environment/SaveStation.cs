using Godot;
using System;

public class SaveStation : Spatial
{
    private int waitFrames = 0;
    public override void _Ready()
    {
        GetChild(0).Connect("body_entered", this, nameof(SaveGame));
        if (!GameManager.Instance.playing)
            waitFrames = 100;
    }

    public override void _Process(float delta)
    {
        if (waitFrames > 0)
            waitFrames -= 1;
    }

    public void SaveGame(Node body)
    {
        if (waitFrames > 0)
            return;
        if (body is PlayerController player)
        {
            if (!GameManager.Instance.playing)
                return;
            player.SetPosAndRot(GetChild<Spatial>(3).GlobalTransform);
            player.playMovement.Stop();
            InGameMenu.Instance.SaveRequest();
        }

    }
}
