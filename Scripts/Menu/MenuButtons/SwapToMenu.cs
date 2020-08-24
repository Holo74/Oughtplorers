using Godot;
using System;

public class SwapToMenu : Control
{
    [Export]
    private NodePath path;
    protected Control node;
    public override void _Ready()
    {
        node = GetNode<Control>(path);
        Connect("pressed", this, nameof(PushedButton));
    }

    private void PushedButton()
    {
        if (OnSwap())
        {
            GameManager.Instance.currentMenu.MakeMainMenu(node);
        }
    }

    protected virtual bool OnSwap()
    {
        return true;
    }
}
