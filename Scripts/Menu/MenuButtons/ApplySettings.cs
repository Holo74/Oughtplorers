using Godot;
using System;

public class ApplySettings : Button
{

    public override void _Ready()
    {
        Connect("pressed", this, nameof(Applied));
    }

    private void Applied()
    {
        SettingsOptions.ApplyNewSettings();
        SavingAndLoading.SavingOptionData("user://Option.save");
    }
}
