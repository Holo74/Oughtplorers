using Godot;
using System;

public class SettingFloatValues : Node
{
    [Export]
    private string name = "";
    [Export]
    private SettingsNames optionName;
    private SpinBox number;
    private Slider slider;
    [Export]
    private float multiplier = 1, min = 0, max = 100;
    public override void _Ready()
    {
        number = GetChild<SpinBox>(0);
        slider = GetChild<Slider>(1);
        number.Connect("value_changed", this, nameof(ValuesChanged));
        slider.Connect("value_changed", this, nameof(ValuesChanged));
        number.MaxValue = max;
        slider.MaxValue = max;
        slider.MinValue = min;
        number.MinValue = min;
        GetChild<RichTextLabel>(2).BbcodeText = "[center]" + name + "[/center]";
        number.Value = SettingsOptions.GetSetting<float>(optionName) * multiplier;
        slider.Value = number.Value;
        SettingsOptions.RegisterUpdatedEvent(UpdateValue);
    }

    private void ValuesChanged(float value)
    {
        slider.Value = value;
        number.Value = value;
        SettingsOptions.UpdateData(optionName, value / multiplier);
    }

    private void UpdateValue()
    {
        slider.Value = SettingsOptions.GetSetting<float>(optionName) * multiplier;
    }

    public override void _ExitTree()
    {
        SettingsOptions.DeRegisterEvent(UpdateValue);
    }
}
