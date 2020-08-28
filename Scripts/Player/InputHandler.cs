using Godot;
using System;

//Is the singolton that gets all the inputs for the player to use
public class InputHandler : Node
{
    [Signal]
    public delegate void MouseMoved(Vector2 inputting);
    public delegate void inputting();
    private inputting MF, MB, ML, MR, J, C, S, T, H, G, St, Q, W1, W2, W3, W4, Ho, CWU, CWD;
    public void RegisterToInputEvent(Keys key, inputting function)
    {
        switch (key)
        {
            case Keys.moveForward:
                MF += function;
                break;
            case Keys.moveBack:
                MB += function;
                break;
            case Keys.moveLeft:
                ML += function;
                break;
            case Keys.moveRight:
                MR += function;
                break;
            case Keys.jump:
                J += function;
                break;
            case Keys.crouch:
                C += function;
                break;
            case Keys.sprint:
                S += function;
                break;
            case Keys.throwing:
                T += function;
                break;
            case Keys.hitting:
                H += function;
                break;
            case Keys.gliding:
                G += function;
                break;
            case Keys.strafe:
                St += function;
                break;
            case Keys.escapeButton:
                Q += function;
                break;
            case Keys.weapon1:
                W1 += function;
                break;
            case Keys.weapon2:
                W2 += function;
                break;
            case Keys.weapon3:
                W3 += function;
                break;
            case Keys.weapon4:
                W4 += function;
                break;
            case Keys.hostler:
                Ho += function;
                break;
            case Keys.cycleUp:
                CWU += function;
                break;
            case Keys.cycleDown:
                CWD += function;
                break;
            default:
                break;
        }
    }
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
        "CycleWeaponDown"
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
    cycleDown
}