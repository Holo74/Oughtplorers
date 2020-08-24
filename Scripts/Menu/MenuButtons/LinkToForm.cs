using Godot;
using System;

public class LinkToForm : Node
{
    public override void _Ready()
    {
        Connect("pressed", this, nameof(OpenForm));
    }
    private void OpenForm()
    {
        OS.ShellOpen("https://forms.gle/vtRYUNtiyY48Jy588");
    }
}
