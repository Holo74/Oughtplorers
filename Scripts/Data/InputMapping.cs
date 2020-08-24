using Godot;

///<summary>This is used to label buttons with what button is used for a certain action. it should be replaced with something that can change input as well</summary>
public class InputMapping : Node
{
    [Export]
    private string text = "";
    [Export]
    private Keys input = Keys.crouch;
    private bool readingInput = false;
    private Button button;
    private float timer = 0;
    public override void _Ready()
    {
        button = GetChild<Button>(0);
        button.Text = ((InputEvent)InputMap.GetActionList(InputHandler.GetNameFromKey(input))[0]).AsText();
        GetChild<RichTextLabel>(1).Text = text;
        button.Connect("pressed", this, nameof(ButtonPressed));
        SettingsOptions.RegisterUpdatedEvent(UpdatingKeyConfig);
    }

    public override void _Process(float delta)
    {
        if (timer < 0)
        {
            readingInput = false;
            UpdatingKeyConfig();
        }
        else
        {
            timer -= delta;
        }
    }

    private void ButtonPressed()
    {
        readingInput = true;
        button.Text = "...";
        timer = 4;
    }

    public override void _Input(InputEvent @event)
    {
        if (readingInput)
        {
            if (@event is InputEventKey key)
            {
                readingInput = false;
                string newName = OS.GetScancodeString(key.Scancode);
                button.Text = newName;
                SettingsOptions.UpdateKeyData(input, key);
            }
        }
    }

    public void UpdatingKeyConfig()
    {
        InputEventKey updatedKey = SettingsOptions.GetInputFromKey(input);
        InputHandler.SetActionFromInput(InputHandler.GetNameFromKey(input), updatedKey);
        button.Text = OS.GetScancodeString(updatedKey.Scancode);
    }

    public override void _ExitTree()
    {
        SettingsOptions.DeRegisterEvent(UpdatingKeyConfig);
    }
}
