using Godot;
using System;

public class ToggleOptions : Control
{
    [Export]
    private string text = "";
    [Export]
    private SettingsNames optionName;
    private Button button;
    public override void _Ready()
    {
        Button holder = GetChild<Button>(0);
        holder.Pressed = SettingsOptions.GetSetting<bool>(optionName);
        holder.Connect("toggled", this, nameof(ChangeSetting));
        button = holder;
        SettingsOptions.RegisterUpdatedEvent(UpdateValue);
        button.Text = text;
    }

    public void ChangeSetting(bool state)
    {
        SettingsOptions.UpdateData(optionName, state);
    }

    public void UpdateValue()
    {
        button.Pressed = SettingsOptions.GetSetting<bool>(optionName);
    }

    public override void _ExitTree()
    {
        SettingsOptions.DeRegisterEvent(UpdateValue);
    }
}
