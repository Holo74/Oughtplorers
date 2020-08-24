using Godot;
using System;

public class SwapFromSettings : SwapToMenu
{
    [Export]
    private NodePath confirmPath;
    private Control confirm;
    protected override bool OnSwap()
    {
        if (SettingsOptions.ChangesMade())
        {
            if (confirm == null)
            {
                confirm = GetNode<Control>(confirmPath);
            }
            GameManager.Instance.currentMenu.AddonToCurrent(confirm);
            return false;
        }
        return true;
    }
}
