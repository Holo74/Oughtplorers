using Godot;
using System;

//Filters all of the inputs and makes sure that any of the inputs are correct and then sends the request to the ability
public class PlayerInput : BaseAttatch
{
    // private bool acceptingInput = true;
    // public bool Inputs()
    // {
    //     return acceptingInput;
    // }
    private float timeDelta;
    InputHandler input { get { return InputHandler.Instance; } }
    PlayerOptions options { get { return controller.options; } }
    public PlayerInput(PlayerController controller) : base(controller, true)
    {
        controller.playMovement.RegisterVerticalChange(HardLanding);
    }
    private bool sprintToggleOn = false;
    private bool sprintLock = false;
    private float inputLockTimer = -1f;

    // public void ToggleInputAccepting()
    // {
    //     acceptingInput = !acceptingInput;
    // }

    public override void Update(float delta)
    {
        if (GameManager.InCutscene())
            return;
        if (inputLockTimer > 0f)
        {
            inputLockTimer -= delta;
            return;
        }
        timeDelta = delta;
        bool sprint = SprintOutput();
        bool movedForwardOrBack = false;
        if (Input.IsActionJustPressed("Debug"))
        {
            controller.ability.ChangeNoCollision(!controller.ability.GetNoCollide());
        }
        if (controller.ability.GetNoCollide())
        {
            if (Input.IsActionPressed("MoveForward"))
            {
                controller.ability.Move(-controller.camera.GlobalTransform.basis.z, sprint);
            }
            if (Input.IsActionPressed("MoveBack"))
            {
                controller.ability.Move(controller.camera.GlobalTransform.basis.z, sprint);
            }
            if (Input.IsActionPressed("MoveLeft"))
            {
                controller.ability.Move(-controller.camera.GlobalTransform.basis.x, sprint);
            }
            if (Input.IsActionPressed("MoveRight"))
            {
                controller.ability.Move(controller.camera.GlobalTransform.basis.x, sprint);
            }
            return;
        }
        bool jumped = false;
        if (Input.IsActionPressed("MoveForward"))
        {
            controller.ability.Move(-controller.Transform.basis.z, sprint, true);
            movedForwardOrBack = true;
        }
        if (Input.IsActionPressed("MoveBack"))
        {
            controller.ability.Move(controller.Transform.basis.z, sprint);
            movedForwardOrBack = true;
        }
        if (Input.IsActionPressed("MoveLeft"))
        {
            controller.ability.Move(-controller.Transform.basis.x, sprint);
            if (Input.IsActionJustPressed("Strafe") && !movedForwardOrBack)
                controller.ability.Strafe(-controller.Transform.basis.x);
        }
        if (Input.IsActionPressed("MoveRight"))
        {
            controller.ability.Move(controller.Transform.basis.x, sprint);
            if (Input.IsActionJustPressed("Strafe") && !movedForwardOrBack)
                controller.ability.Strafe(controller.Transform.basis.x);
        }
        if (Input.IsActionJustPressed("Jump"))
        {
            controller.ability.Jump();
            jumped = true;
        }
        if (Input.IsActionPressed("Jump") && !jumped)
        {
            controller.ability.Glide();
        }
        if (Input.IsActionJustPressed("Crouch"))
            controller.ability.Crouch();
        if (Input.IsActionJustPressed("Throwing"))
        {
            controller.ability.Throw();
        }
        if (Input.IsActionJustPressed("Weapon1"))
        {
            controller.ability.SwapWeapon(CurrentWeaponEquiped.first);
        }
        if (Input.IsActionJustPressed("Weapon2"))
        {
            controller.ability.SwapWeapon(CurrentWeaponEquiped.second);
        }
        if (Input.IsActionJustPressed("Weapon3"))
        {
            controller.ability.SwapWeapon(CurrentWeaponEquiped.third);
        }
        if (Input.IsActionJustPressed("Weapon4"))
        {
            controller.ability.SwapWeapon(CurrentWeaponEquiped.fourth);
        }
        if (Input.IsActionJustPressed("Holster"))
        {
            controller.ability.SwapWeapon(CurrentWeaponEquiped.none);
        }
        if (Input.IsActionJustReleased("CycleWeaponUp"))
        {
            controller.ability.SwapWeapon(true);
        }
        if (Input.IsActionJustReleased("CycleWeaponDown"))
        {
            controller.ability.SwapWeapon(false);
        }
    }

    private bool SprintOutput()
    {
        if (SettingsOptions.GetSetting<bool>(SettingsNames.toggleSprint))
        {
            if (Input.IsActionJustPressed("Sprint"))
            {
                sprintToggleOn = !sprintToggleOn;
            }
            return sprintToggleOn;
        }
        return Input.IsActionPressed("Sprint");
    }

    public void Rotating(Vector2 vec)
    {
        if (GameManager.InCutscene())
            return;
        if (GameManager.Instance.playing && inputLockTimer <= 0f)
        {
            controller.bodyRotation.RotateAmount(vec.x * timeDelta * SettingsOptions.GetSetting<float>(SettingsNames.mouseXSensitivity) *
            (SettingsOptions.GetSetting<bool>(SettingsNames.invertX) ? -1 : 1));
            controller.headRotation.RotateAmount(vec.y * timeDelta * SettingsOptions.GetSetting<float>(SettingsNames.mouseYSensitivity) *
            (SettingsOptions.GetSetting<bool>(SettingsNames.invertY) ? -1 : 1));
        }
    }

    public void HardLanding(float amount)
    {
        if (amount < -20)
        {
            inputLockTimer = 1f;
        }
    }
}
