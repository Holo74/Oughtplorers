using Godot.Collections;
using System.Collections;
using Godot;

public class SettingsOptions
{
    private static float mouseXSensitivity = -.2f;
    private static float mouseYSensitivity = -.2f;
    private static bool toggleSprint = false;
    private static bool invertX = false, invertY = false;
    private static float cameraFOV = 70f;
    public static bool leanWhileRunning = true;
    private static Dictionary SettingsData, NewSettings;
    public delegate void UpdateSettings();
    private static event UpdateSettings updated;

    public static void RegisterUpdatedEvent(UpdateSettings function)
    {
        updated += function;
    }

    public static void DeRegisterEvent(UpdateSettings function)
    {
        updated -= function;
    }

    public static void SetData(Dictionary data)
    {
        SettingsData = data.Duplicate();
        NewSettings = data.Duplicate();
        for (int i = 0; i < 17; i++)
        {
            if (i > 6 && i < 10)
                continue;
            InputHandler.SetActionFromString(InputHandler.GetNameFromInt(i), (string)SettingsData[InputHandler.GetNameFromInt(i)]);
            string name = InputHandler.GetNameFromInt(i);
            SettingsData[name] = InputHandler.GetScanCode(name);
        }
    }
    private static void UpdateDataFromSource()
    {
        if (SettingsData != null)
        {
            SettingsData[SettingsNames.mouseXSensitivity.ToString()] = mouseXSensitivity;
            SettingsData[SettingsNames.mouseYSensitivity.ToString()] = mouseYSensitivity;
            SettingsData[SettingsNames.toggleSprint.ToString()] = toggleSprint;
            SettingsData[SettingsNames.cameraFOV.ToString()] = cameraFOV;
            SettingsData[SettingsNames.invertX.ToString()] = invertX;
            SettingsData[SettingsNames.invertY.ToString()] = invertY;
            for (int i = 0; i < 17; i++)
            {
                if (i > 6 && i < 10)
                    continue;
                string name = InputHandler.GetNameFromInt(i);
                SettingsData[name] = InputHandler.GetScanCode(name);
            }
        }
        else
        {
            SettingsData = new Dictionary();
            SettingsData.Add(SettingsNames.mouseXSensitivity.ToString(), mouseXSensitivity);
            SettingsData.Add(SettingsNames.mouseYSensitivity.ToString(), mouseYSensitivity);
            SettingsData.Add(SettingsNames.toggleSprint.ToString(), toggleSprint);
            SettingsData.Add(SettingsNames.cameraFOV.ToString(), cameraFOV);
            SettingsData.Add(SettingsNames.invertX.ToString(), invertX);
            SettingsData.Add(SettingsNames.invertY.ToString(), invertY);
            for (int i = 0; i < 17; i++)
            {
                if (i > 6 && i < 10)
                    continue;
                string name = InputHandler.GetNameFromInt(i);
                SettingsData.Add(name, InputHandler.GetScanCode(name));
            }
        }
        NewSettings = SettingsData.Duplicate();
    }
    public static Dictionary GetData()
    {
        if (SettingsData == null)
        {
            UpdateDataFromSource();
        }
        return SettingsData;
    }
    public static void UpdateData(SettingsNames name, object data)
    {
        if (SettingsData == null)
            UpdateDataFromSource();
        NewSettings[name.ToString()] = data;
    }
    public static void UpdateKeyData(Keys name, InputEventKey key)
    {
        if (SettingsData == null)
            UpdateDataFromSource();
        NewSettings[InputHandler.GetNameFromKey(name)] = OS.GetScancodeString(key.Scancode);
    }
    public static T GetSetting<T>(SettingsNames name)
    {
        return (T)(GetData()[name.ToString()]);
    }
    public static InputEventKey GetInputFromKey(Keys key)
    {
        InputEventKey inputEvent = new InputEventKey();
        inputEvent.Scancode = (uint)OS.FindScancodeFromString((string)SettingsData[InputHandler.GetNameFromKey(key)]);
        return inputEvent;
    }

    public static void ApplyNewSettings()
    {
        SettingsData = NewSettings.Duplicate();
        updated?.Invoke();
    }

    public static void ResetNewSettings()
    {
        NewSettings = SettingsData.Duplicate();
        updated?.Invoke();
    }

    public static bool ChangesMade()
    {
        ICollection newS = NewSettings.Values;
        ICollection oldS = SettingsData.Values;
        return !newS.ToString().Equals(oldS.ToString());
    }
}

public enum SettingsNames
{
    mouseXSensitivity,
    mouseYSensitivity,
    toggleSprint,
    cameraFOV,
    invertX,
    invertY
}
