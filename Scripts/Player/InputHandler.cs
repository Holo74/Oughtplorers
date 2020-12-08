using Godot;
using System;

//Is the singolton that gets all the inputs for the player to use
public class InputHandler : Node
{
    [Signal]
    public delegate void MouseMoved(Vector2 inputting);
    private bool[] inputs = new bool[19];
    public static InputHandler Instance { get; private set; }
    private static string[] ActionNames =
    {
        "MoveForward", "MoveBack", "MoveLeft",
        "MoveRight", "Jump", "Crouch",
        "Sprint", "Throwing", "Hit",
        "Jump", "Strafe", "Quit",
        "Weapon1", "Weapon2", "Weapon3",
        "Weapon4", "Holster", "CycleWeaponUp",
        "CycleWeaponDown", "Zoom"
    };

    public static string GetNameFromKey(Keys key)
    {
        return ActionNames[(int)key];
    }
    public static string GetNameFromInt(int key)
    {
        return ActionNames[key];
    }

    public override void _Ready()
    {
        Instance = this;
    }

    public static string GetScanCode(string key)
    {
        string holder = "";
        InputEventKey eventKey = (InputEventKey)InputMap.GetActionList(key)[0];
        holder = OS.GetScancodeString(eventKey.Scancode);
        return holder;
    }

    public static void SetActionFromString(string key, string code)
    {
        InputEventKey inputEvent = new InputEventKey();
        inputEvent.Scancode = (uint)OS.FindScancodeFromString(code);
        SetActionFromInput(key, inputEvent);
    }

    public static void SetActionFromInput(string key, InputEventKey code)
    {
        InputMap.EraseAction(key);
        InputMap.AddAction(key);
        InputMap.ActionAddEvent(key, code);
    }

    public override void _Input(InputEvent @event)
    {
        if (!GameManager.Instance.allowInputs)
            return;
        if (@event is InputEventMouseMotion motion)
        {
            EmitSignal(nameof(MouseMoved), motion.Relative);
        }
    }

    public bool GetInput(Keys key)
    {
        return inputs[(int)key];
    }

    public void ConnectToMouseMovement(Godot.Object target, string name)
    {
        Connect(nameof(InputHandler.MouseMoved), target, name);
    }
}

public enum Keys
{
    moveForward,
    moveBack,
    moveLeft,
    moveRight,
    jump,
    crouch,
    sprint,
    throwing,
    hitting,
    gliding,
    strafe,
    escapeButton,
    weapon1,
    weapon2,
    weapon3,
    weapon4,
    hostler,
    cycleUp,
    cycleDown,
    zoom
}