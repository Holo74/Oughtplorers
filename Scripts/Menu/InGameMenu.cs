using Godot;
using System;

//An in game menu that controls the settings and the regular hud
public class InGameMenu : MenuBase
{
    [Export]
    private NodePath pathToQuit, pathToHealthConatiner;
    private Control hud, menu, healthContainer, savingRequest, savingCompleting, upgradeMenu, transitionNode;
    private TextureProgress healthBar;
    private RichTextLabel displayText, upgradeName, upgradeDescription;
    private static InGameMenu instance;
    public static InGameMenu Instance { get { return instance; } }
    private float timer = 4f, savingBuffer = 1f;
    private bool saving = false, finishAnim = false;
    public string inputMapAction = "", finishToAnim = "";
    private AnimationPlayer animations;
    [Signal]
    public delegate void TransitionCamera();
    private Tween textTween;
    private TextureRect crossHair;
    public override void _Ready()
    {
        instance = this;
        hud = GetChild<Control>(0);
        menu = GetChild<Control>(1);
        savingRequest = GetChild<Control>(3);
        savingCompleting = GetChild<Control>(4);
        mainNode = menu;
        healthContainer = GetNode<Control>(pathToHealthConatiner);
        healthBar = healthContainer.GetChild<TextureProgress>(0);
        displayText = hud.GetChild<RichTextLabel>(2);
        animations = GetChild<AnimationPlayer>(6);
        upgradeMenu = GetChild<Control>(8);
        upgradeName = upgradeMenu.GetChild(0).GetChild<RichTextLabel>(0);
        upgradeDescription = upgradeMenu.GetChild(0).GetChild<RichTextLabel>(1);
        transitionNode = GetChild<Control>(9);
        textTween = hud.GetChild<Tween>(4);
        crossHair = hud.GetChild<TextureRect>(0);
        CallDeferred(nameof(DeferedSetup));
    }

    private void DeferedSetup()
    {
        upgradeMenu.GetChild(0).GetChild(2).Connect("pressed", GameManager.Instance, nameof(GameManager.ToggleGamePause));
        PlayerController.Instance.ability.AddToStateChange(PlayerStateAnimations);

        foreach (Node c in GetChildren())
        {
            if (c is Control a)
                a.Visible = false;
        }
        GameManager.Instance.currentMenu = this;
        GameManager.Instance.Connect(nameof(GameManager.ToggleGame), this, nameof(ToggleMenu));
        PlayerController.Instance.Connect(nameof(PlayerController.UpdateHealth), this, nameof(UpdateHealth));
        PlayerController.Instance.AttachToDeath(Dead);
        ScanNode.Instance.Connect(nameof(ScanNode.FoundScannable), this, nameof(FoundScans));
        ScanNode.Instance.Connect(nameof(ScanNode.LostScannables), this, nameof(LostScans));
    }

    public void FoundScans(Scannables scans)
    {
        SetCrossHairColor("00bfff");
    }

    public void LostScans()
    {
        SetCrossHairColor("ffffff");
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("Quit") && PlayerController.CharacterPlaying() && GameManager.Instance.allowInputs && !PlayerController.Instance.IsDead())
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
        if (timer < 4f)
        {
            timer += delta;
            if (timer > 4f)
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
        instance.displayText.PercentVisible = 0.0f;
        instance.textTween.InterpolateProperty(instance.displayText, "percent_visible", 0.0f, 1.0f, 2.0f);
        instance.displayText.BbcodeText = "[center]" + text + "[/center]";
        instance.timer = 0f;
        instance.textTween.Start();
    }

    public void UpdateHealth(bool damaged, float newValue)
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
        mainNode = menu;
        hud.Visible = true;
        hud.RectPosition = Vector2.Zero;
    }

    public Control Hud()
    {
        return hud;
    }

    private void PlayerStateAnimations(PlayerState state)
    {
        if (!hud.Visible)
            return;
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

    public void StartCameraTransition()
    {
        hud.Visible = false;
        mainNode.Visible = false;
        mainNode = transitionNode;
        mainNode.Visible = true;
        animations.Play("TransferingCamera");

    }

    public void EmitSignalToTransition()
    {
        EmitSignal("TransitionCamera");
    }

    public void SetCrossHairColor(string color)
    {
        crossHair.Modulate = new Color(color);
    }
}
