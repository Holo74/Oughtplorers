using Godot;
using System;

public class RegectSettings : SwapToMenu
{
    protected override bool OnSwap()
    {
        SettingsOptions.ResetNewSettings();
        return true;
    }
}
