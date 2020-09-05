using Godot;
using System;

//An in game menu that controls the settings and the regular hud
public class InGameMenu : MenuBase
{
    [Export]
    private NodePath pathToQuit, pathToHealthConatiner;
    private Control hud, menu, healthContainer, savingRequest, savingCompleting, upgradeMenu;
    private ProgressBar healthBar;
    private RichTextLabel displayText, upgradeName, upgradeDescription;
    private static InGameMenu instance;
    public static InGameMenu Instance { get { return instance; } }
    private float timer = 4f, savingBuffer = 1f;
    private bool saving = false, finishAnim = false;
    public string inputMapAction = "", finishToAnim = "";
    private AnimationPlayer animations;
    public override void _Ready()
    {
        GameManager.Instance.currentMenu = this;
        foreach (Node c in GetChildren())
        {
            if (c is Control a)
                a.Visible = false;
        }
        GameManager.Instance.Connect(nameof(GameManager.ToggleGame), this, nameof(ToggleMenu));
        PlayerController.Instance.Connect(nameof(PlayerController.TakingDamage), this, nameof(UpdateHealth));
        PlayerController.Instance.AttachToDeath(Dead);
        hud = GetChild<Control>(0);
        menu = GetChild<Control>(1);
        savingRequest = GetChild<Control>(3);
        savingCompleting = GetChild<Control>(4);
        mainNode = menu;
        healthContainer = GetNode<Control>(pathToHealthConatiner);
        healthBar = healthContainer.GetChild<ProgressBar>(0);
        displayText = GetChild(0).GetChild<RichTextLabel>(5);
        instance = this;
        animations = GetChild<AnimationPlayer>(6);
        upgradeMenu = GetChild<Control>(8);
        upgradeName = upgradeMenu.GetChild(0).GetChild<RichTextLabel>(0);
        upgradeDescription = upgradeMenu.GetChild(0).GetChild<RichTextLabel>(1);
        upgradeMenu.GetChild(0).GetChild(2).Connect("pressed", GameManager.Instance, nameof(GameManager.ToggleGamePause));
        PlayerController.Instance.ability.AddToStateChange(PlayerStateAnimations);
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("Quit") && PlayerController.CharacterPlaying() && GameManager.Instance.allowInputs)
        {
            GameManager.Instance.ToggleGamePause();
        }
        if (saving)
        {
            savingBuffer -= delta;
            if (savingBuffer < 0)
            {
                saving = false;
                GameManager.Instance.SaveGame(WorldManager.instance.GetCurrentRoomFile());
            }
        }
        if (finishAnim)
        {
            if (animations.CurrentAnimationLength == animations.CurrentAnimationPosition)
            {
                finishAnim = false;
                animations.CurrentAnimation = finishToAnim;
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (timer < 3f)
        {
            timer += delta;
            if (timer > 3f)
            {
                displayText.BbcodeText = "";
            }
        }
    }

    public void ToggleMenu(bool state)
    {
        if (state)
        {
            SwapToHud();
            SettingsOptions.ResetNewSettings();
            mainNode = menu;
        }
        else
        {
            hud.Visible = false;
            mainNode.Visible = true;
        }
    }

    public static void DisplayText(string text)
    {
        instance.displayText.BbcodeText = "[center]" + text + "[/center]";
        instance.timer = 0f;
    }

    public void UpdateHealth(float newValue)
    {
        healthBar.Value = (newValue - 1) % 100;
    }

    private void Dead()
    {
        healthBar.Value = 0;
    }

    public void ReadyMenu()
    {
        SwapToHud();
    }

    public void SaveGameConfirmed(bool saved)
    {
        if (saved)
        {
            mainNode.Visible = false;
            mainNode = savingCompleting;
            mainNode.Visible = true;
            saving = true;
            savingBuffer = 1f;
        }
        else
        {
            GameManager.Instance.ToggleGamePause();
        }
    }

    public void SaveRequest()
    {
        mainNode = savingRequest;
        GameManager.Instance.ToggleGamePause();
    }

    private void SwapToHud()
    {
        mainNode.Visible = false;
        hud.Visible = true;
        hud.RectPosition = Vector2.Zero;
    }

    public Control Hud()
    {
        return hud;
    }

    private void PlayerStateAnimations(PlayerState state)
    {
        string animationName = "";
        bool f = false;
        switch (state)
        {
            case PlayerState.standing:
                animationName = "Idle";
                break;
            case PlayerState.walking:
                animationName = "Walking";
                break;
            case PlayerState.crouch:
                break;
            case PlayerState.fallingDown:
                break;
            case PlayerState.fallingUp:
                animationName = "Jumping";
                f = true;
                break;
        }
        if (finishAnim)
            finishToAnim = animationName;
        else
            animations.CurrentAnimation = animationName;
        finishAnim = f;
    }

    public void GetUpgrade(string name, string description)
    {
        mainNode = upgradeMenu;
        upgradeName.BbcodeText = "[center]" + name + "[/center]";
        upgradeDescription.Text = description;
        GameManager.Instance.ToggleGamePause();
    }
}
