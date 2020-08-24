using Godot;
using System;

public class SaveStation : Spatial
{
    public override void _Ready()
    {
        GetChild(0).Connect("body_entered", this, nameof(SaveGame));
    }

    public void SaveGame(Node body)
    {
        if (body is PlayerController player)
        {
            if (!GameManager.Instance.playing)
                return;
            player.Translation = GlobalTransform.origin;
            player.Rotation = GlobalTransform.basis.GetEuler();
            player.headRotation.LookForward(player.Rotation);
            player.playMovement.Stop();
            InGameMenu.Instance.SaveRequest();
        }

    }
}
