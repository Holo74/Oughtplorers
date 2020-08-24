using Godot;
using System;

public class ConfirmButton : SwapToMenu
{
    protected override bool OnSwap()
    {
        SettingsOptions.ApplyNewSettings();
        return true;
    }
}
