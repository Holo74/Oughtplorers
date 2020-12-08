using Godot;
using System;

//This controls how the abilities work and to process all the filtered inputs
public class PlayerAbility : BaseAttatch
{
    private bool noCollision = false;

    public bool GetNoCollide()
    {
        return noCollision;
    }

    public void ChangeNoCollision(bool state = false)
    {
        noCollision = state;
        controller.playMovement.Stop();
        uint l = state ? 0u : 1u;
        controller.CollisionLayer = l;
        controller.CollisionMask = l;
        if (state)
        {
            ChangeState(PlayerState.noClip);
        }
        else
        {
            ChangeState(PlayerState.empty);
        }
    }
    public PlayerAbility(PlayerController controller) : base(controller, true)
    {
    }

    public bool DoubleJumpUsed = false, tripleJumpUsed = false, requestGroundJump = false;
    public float cayoteTime = 0f, requestGroundJumpTimer = 0f;
    private PlayerState currentState = PlayerState.standing;
    private WallRunningData right = new WallRunningData(RayDirections.Right, AreaSensorDirection.Right);
    private WallRunningData left = new WallRunningData(RayDirections.Left, AreaSensorDirection.Left);
    public bool RunningOnRightWall = false;
    private WallRunningData attachedTo;
    private bool canSprint { get { return controller.upgrades.GetUpgrade(PlayerUpgrade.Sprinting); } }
    private float currentWallRunTime = 0f;
    public delegate void StateChange(PlayerState state);
    private StateChange stateChange;
    private Vector3 strafeDirectionRequest;
    public enum ZoomLevel
    {
        normal,
        zoom1,
        zoom2
    }
    private ZoomLevel currentZoom = ZoomLevel.normal;
    private float zoomRatio = 1;
    public float ZoomRatio { get { return zoomRatio; } }

    public void AddToStateChange(StateChange function)
    {
        stateChange += function;
    }

    public override void Update(float delta)
    {
        base.Update(delta);
        if (!controller.playMovement.groundColliding && cayoteTime < PlayerOptions.cayoteMaxTime)
        {
            cayoteTime += time;
        }
        if (currentState == PlayerState.wallRunning)
        {
            currentWallRunTime += time;
        }
        if (!glideLock)
        {
            if (currentState == PlayerState.glide)
            {
                ChangeState(PlayerState.empty);
            }
        }
        if (requestGroundJump)
        {
            requestGroundJumpTimer += time;
            if (requestGroundJumpTimer > PlayerOptions.jumpRegisterTime)
                requestGroundJump = false;
        }
        glideLock = false;
    }

    public void Move(Vector3 direction, bool sprint, bool wallRun = false)
    {
        switch (currentState)
        {
            case PlayerState.noClip:
            case PlayerState.standing:
            case PlayerState.sprinting:
            case PlayerState.walking:
                float acceleration = PlayerOptions.playerWalkingSpeed;
                float maxSpeed = PlayerOptions.playerMaxWalkingSpeed;
                if (canSprint && sprint)
                {
                    acceleration = PlayerOptions.playerSprintAcceleration;
                    maxSpeed = PlayerOptions.playerMaxSprintSpeed;
                    controller.soundRequest.PlayerRequestedSound("Running", true);
                }
                else
                {
                    controller.soundRequest.PlayerRequestedSound("Walking", true);
                }
                if (controller.playMovement.GetCurrentSpeed() > controller.playMovement.GetMaxSpeed())
                {
                    acceleration = 0;
                }
                controller.playMovement.GroundMovement(direction, maxSpeed, acceleration);

                break;
            case PlayerState.crouch:
                float acc = PlayerOptions.playerCrouchSpeed;
                if (controller.playMovement.GetCurrentSpeed() > controller.playMovement.GetMaxSpeed())
                {
                    acc = 0;
                }
                controller.playMovement.GroundMovement(direction, PlayerOptions.playerMaxCrouchSpeed, acc);
                controller.soundRequest.PlayerRequestedSound("Walking", true);
                break;
            case PlayerState.wallRunning:
            case PlayerState.fallingDown:
                if (currentWallRunTime < controller.upgrades.GetWallRunTotalTime())
                {
                    if (controller.upgrades.GetUpgrade(PlayerUpgrade.WallRunning))
                    {
                        if (WallRun(wallRun))
                        {
                            break;
                        }
                    }
                }
                if (currentState == PlayerState.wallRunning)
                {
                    ChangeState(PlayerState.empty);
                }
                goto case PlayerState.fallingUp;
            case PlayerState.fallingUp:
                controller.playMovement.AirMovement(direction, PlayerOptions.airMovementPush);
                break;
            case PlayerState.glide:
                controller.playMovement.AirMovement(direction, PlayerOptions.glidePull);
                break;
            default:
                break;
        }
    }

    public void Strafe(Vector3 direction)
    {
        switch (currentState)
        {
            case PlayerState.standing:
            case PlayerState.walking:
            case PlayerState.sprinting:
            case PlayerState.empty:
                if (controller.upgrades.GetUpgrade(PlayerUpgrade.StrafeJump) && controller.playMovement.GetCurrentSpeed() < controller.playMovement.GetMaxSpeed() + 1f)
                {
                    controller.playMovement.VerticalIncrease(PlayerOptions.playerStrafeStrengthVer);
                    controller.playMovement.HorizontalAccelerationSet(strafeDirectionRequest * PlayerOptions.playerStrafeStrengthHor);
                    controller.soundRequest.PlayerRequestedSound("Jumping");
                    strafeDirectionRequest = direction;
                }
                break;
        }
    }

    private bool WallRun(bool continuing)
    {
        if (!continuing)
            return false;
        RunningOnRightWall = right.Sensed();

        if (RunningOnRightWall || left.Sensed())
        {
            attachedTo = right.Sensed() ? right : left;
            if (attachedTo.Normals().Dot(controller.Transform.basis.z) > PlayerOptions.wallRunningAngleAllowance)
            {
                return false;
            }
            ChangeState(PlayerState.wallRunning);
            controller.playMovement.ChangeStableVectorDirection(attachedTo.Normals().Cross(Vector3.Up) * (attachedTo == right ? 1 : -1));
            controller.playMovement.SetPushing(-attachedTo.Normals() * 10);
            controller.playMovement.HorizontalAccelerationSet(Vector3.Zero);
            return true;
        }
        if (currentState == PlayerState.wallRunning)
        {
            ChangeState(PlayerState.empty);
            ResetJumps();
        }
        return false;
    }
    private bool glideLock = false;
    public void Glide()
    {
        switch (currentState)
        {
            case PlayerState.glide:
            case PlayerState.fallingDown:
                if (controller.upgrades.GetUpgrade(PlayerUpgrade.Glide))
                {
                    currentState = PlayerState.glide;
                    glideLock = true;
                }
                break;
        }
    }

    public void Jump()
    {
        if (!CanJump())
            return;
        switch (currentState)
        {
            case PlayerState.slide:
            case PlayerState.standing:
            case PlayerState.sprinting:
            case PlayerState.walking:
            case PlayerState.crouch:
                controller.playMovement.VerticalIncrease(PlayerOptions.jumpStr);
                cayoteTime = PlayerOptions.cayoteMaxTime;
                break;
            case PlayerState.wallRunning:
                controller.playMovement.VerticalIncrease(PlayerOptions.wallJumpVerStr);
                if (RunningOnRightWall || left.Sensed())
                    controller.playMovement.HorizontalAccelerationSet(PlayerOptions.wallJumpHorStr * attachedTo.Normals());
                ResetJumps();
                break;
            case PlayerState.fallingUp:
            case PlayerState.fallingDown:
                if (cayoteTime < PlayerOptions.cayoteMaxTime)
                {
                    controller.playMovement.VerticalIncrease(PlayerOptions.jumpStr);
                    cayoteTime = PlayerOptions.cayoteMaxTime;
                    break;
                }
                if (controller.upgrades.GetUpgrade(PlayerUpgrade.DoubleJump) && !DoubleJumpUsed)
                {
                    controller.playMovement.VerticalIncrease(PlayerOptions.doubleJumpStr);
                    DoubleJumpUsed = true;
                    break;
                }
                if (controller.upgrades.GetUpgrade(PlayerUpgrade.TripleJump) && !tripleJumpUsed)
                {
                    controller.playMovement.VerticalIncrease(PlayerOptions.tripleJumpStr);
                    tripleJumpUsed = true;
                    break;
                }
                //Jump right as you land
                break;
            default:
                break;
        }
        controller.soundRequest.PlayerRequestedSound("Jumping");
    }

    private void ResetJumps()
    {
        DoubleJumpUsed = false;
        tripleJumpUsed = false;
        currentWallRunTime = 0f;
    }

    public void Land(bool state)
    {
        if (state)
        {
            ResetJumps();
            cayoteTime = 0f;
            if (requestGroundJump && requestGroundJumpTimer < PlayerOptions.jumpRegisterTime)
            {
                Jump();
            }
            requestGroundJumpTimer = 0f;
            requestGroundJump = false;
        }
    }

    public void Crouch()
    {
        switch (currentState)
        {
            case PlayerState.slide:
            case PlayerState.crouch:
                controller.playMovement.Accelerate();
                controller.size.Crouch();
                break;
            case PlayerState.standing:
            case PlayerState.walking:
                controller.size.Crouch();
                break;
            case PlayerState.sprinting:
                if (controller.upgrades.GetUpgrade(PlayerUpgrade.Slide))
                {
                    controller.playMovement.Accelerate(PlayerOptions.slideStr);
                }
                controller.size.Crouch();
                break;
        }
    }

    public void Throw()
    {
        if (controller.gunCamera.CurrentWeapon().WeaponReady())
        {
            controller.gunCamera.UseCurrentWeapon();
        }
    }

    public void UseSecondary()
    {
        if (controller.gunCamera.CurrentWeapon().WeaponReady())
        {
            controller.gunCamera.UseCurrentSecondary();
        }
    }

    public void BookMenu()
    {
        controller.bookMenu.ActivateBook();
    }

    public void SwapWeapon(CurrentWeaponEquiped request)
    {
        string requested = "";
        switch (request)
        {
            case CurrentWeaponEquiped.first:
                requested = PlayerUpgrade.FirstWeapon;
                break;
            case CurrentWeaponEquiped.second:
                requested = PlayerUpgrade.SecondWeapon;
                break;
            case CurrentWeaponEquiped.third:
                requested = PlayerUpgrade.ThirdWeapon;
                break;
            case CurrentWeaponEquiped.fourth:
                requested = PlayerUpgrade.FourthWeapon;
                break;
            case CurrentWeaponEquiped.none:
                controller.gunCamera.EquipWeapon(request);
                return;
        }
        if (controller.upgrades.GetUpgrade(requested))
            controller.gunCamera.EquipWeapon(request);
    }

    public PlayerState GetCurrentState()
    {
        return currentState;
    }
    public void ChangeState(PlayerState state)
    {
        if (currentState == PlayerState.noClip)
        {
            if (state == PlayerState.empty)
            {
                currentState = state;
            }
            return;
        }
        if (currentState != state)
        {
            currentState = state;
            stateChange?.Invoke(state);
        }
    }

    public void ZoomIn()
    {
        switch (currentZoom)
        {
            case ZoomLevel.normal:
                SetZoom(ZoomLevel.zoom1);
                break;
            case ZoomLevel.zoom1:
                SetZoom(ZoomLevel.zoom2);
                break;
            case ZoomLevel.zoom2:
                SetZoom(ZoomLevel.normal);
                break;
        }
    }

    private void SetZoom(ZoomLevel level)
    {
        currentZoom = level;
        float newValue = 0f;
        switch (currentZoom)
        {
            case ZoomLevel.normal:
                newValue = SettingsOptions.GetSetting<float>(SettingsNames.cameraFOV);
                break;
            case ZoomLevel.zoom1:
                newValue = PlayerOptions.level1Zoom;
                break;
            case ZoomLevel.zoom2:
                newValue = PlayerOptions.level2Zoom;
                break;
        }
        controller.anim.ZoomCamera(newValue, controller.camera.Fov);
        zoomRatio = newValue / SettingsOptions.GetSetting<float>(SettingsNames.cameraFOV);
    }

    public bool CanJump()
    {
        return ((controller.upgrades.GetUpgrade(PlayerUpgrade.DoubleJump) && !DoubleJumpUsed)
                || (controller.upgrades.GetUpgrade(PlayerUpgrade.TripleJump) && !tripleJumpUsed)
                || cayoteTime < PlayerOptions.cayoteMaxTime) && !PlayerAreaSensor.GetArea(AreaSensorDirection.Above);
    }

    public void ToggleLight()
    {
        if (controller.headLamp.LightEnergy == 1)
        {
            controller.headLamp.LightEnergy = 0;
        }
        else
        {
            controller.headLamp.LightEnergy = 1;
        }
    }
}

public class WallRunningData
{
    RayDirections ray;
    AreaSensorDirection sensorDirection;
    public WallRunningData(RayDirections direction, AreaSensorDirection sensor)
    {
        this.ray = direction;
        sensorDirection = sensor;
    }

    public Vector3 Normals()
    {
        return RayCastData.SurroundingCasts[ray].normal;
    }

    public bool Sensed()
    {
        return RayCastData.SurroundingCasts[ray].colliding && PlayerAreaSensor.GetArea(sensorDirection);
    }
}

public enum PlayerState
{
    walking,
    standing,
    wallRunning,
    fallingUp,
    fallingDown,
    empty,
    sprinting,
    crouch,
    glide,
    slide,
    noClip
}