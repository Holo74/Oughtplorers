using Godot;
using System;

//Should be an option to toggle in the menu to display fps
public class FPSCounter : Label
{
    public override void _Process(float delta)
    {
        Text = "FPS: " + Performance.GetMonitor(Performance.Monitor.TimeFps).ToString();
    }
}
