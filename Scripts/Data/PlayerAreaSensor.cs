using Godot;
using System.Collections.Generic;

///<summary>Used to detect if anything exists in a larger area than what raycasts are capable of doing</summary>
public class PlayerAreaSensor : Node
{
    //Used to label the are that the sensors are detecting in.
    [Export]
    private AreaSensorDirection direction;
    //This is used to see if anything is still in the body's area
    private int currentBodyCount;
    //Used to get areas without directly referencing to it
    private static Dictionary<AreaSensorDirection, bool> area = new Dictionary<AreaSensorDirection, bool>();
    //Gets the objects by the name that they go by
    private static Dictionary<AreaSensorDirection, PlayerAreaSensor> areaSensors = new Dictionary<AreaSensorDirection, PlayerAreaSensor>();
    //If something appears in the space or if everything is out of the space
    [Signal]
    public delegate void ChangedState(bool state);

    public static bool GetArea(AreaSensorDirection direction)
    {
        if (area.ContainsKey(direction))
            return area[direction];
        return false;
    }

    public static PlayerAreaSensor GetPlayerSensor(AreaSensorDirection direction)
    {
        if (areaSensors.ContainsKey(direction))
            return areaSensors[direction];
        return null;
    }

    public override void _Ready()
    {
        if (!area.ContainsKey(direction))
        {
            area.Add(direction, false);
        }
        if (!areaSensors.ContainsKey(direction))
        {
            areaSensors.Add(direction, this);
        }
        Connect("body_entered", this, nameof(Entered));
        Connect("body_exited", this, nameof(Left));
    }

    public void Entered(Node body)
    {
        if (body.Name != "Player")
        {
            currentBodyCount++;
            if (!area[direction])
            {
                area[direction] = true;
                EmitSignal(nameof(ChangedState), true);
            }
        }
    }

    public void RegisterStateChange(Godot.Object target, string method)
    {
        Connect(nameof(ChangedState), target, method);
    }

    public void Left(Node body)
    {
        if (body.Name != "Player")
        {
            currentBodyCount--;
            if (currentBodyCount == 0)
            {
                area[direction] = false;
                EmitSignal(nameof(ChangedState), false);
            }
        }
    }

    public static void ResetSensors()
    {
        areaSensors = new Dictionary<AreaSensorDirection, PlayerAreaSensor>();
        area = new Dictionary<AreaSensorDirection, bool>();
    }
}

public enum AreaSensorDirection
{
    Left,
    Right,
    Bottom,
    Above
}
