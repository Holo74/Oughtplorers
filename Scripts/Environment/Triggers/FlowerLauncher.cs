using Godot;
using System;

[Tool]
public class FlowerLauncher : Spatial
{
    [Export]
    private float strength = 10f;
    [Export]
    private bool updateLine = true;
    private ImmediateGeometry tragectory;

    public override void _Ready()
    {
        if (Engine.EditorHint)
        {
            tragectory = (ImmediateGeometry)GetTree().GetNodesInGroup("Lines")[0];
        }
    }

    public void HitPlayer(Node body)
    {
        if (body is PlayerController player)
        {
            player.playMovement.SetKnockback(GlobalTransform.basis.y * strength);
        }
    }

    public override void _Process(float delta)
    {
        if (Engine.EditorHint)
        {
            if (updateLine)
            {
                updateLine = false;
                tragectory.Clear();
                tragectory.Begin(Mesh.PrimitiveType.LineStrip);
                float time = 0;
                Vector3 vect = (GlobalTransform.basis.y * strength);
                float step = .01f;
                while (time < 2)
                {
                    tragectory.AddVertex(GlobalTransform.origin +
                    new Vector3(
                            vect.x * time,
                            vect.y * time - (PlayerOptions.gravity / 2) * time * time,
                            vect.z * time
                    ));
                    time += step;
                }
                tragectory.End();
            }
        }
    }
}
