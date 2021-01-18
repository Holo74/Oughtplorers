using Godot;
using System;

//It works... don't ask questions on why or how it works because I don't even know anymore
public class Momentum : BaseAttatch
{
    public Momentum(PlayerController controller) : base(controller, true) { }
    private Vector3 groundMove = new Vector3();
    private Vector3 horizontalAcc = new Vector3();
    private Vector3 verticalMove = new Vector3(), verticalAddition = new Vector3();
    private Vector3 stableMove = new Vector3();
    private Vector3 pushing = new Vector3();
    private Vector3 fallbackMovement = new Vector3(), knockback = new Vector3();
    private Vector3 floorMovement = new Vector3();
    private float currentSpeed = 0;
    private bool moved = false;
    private float movedBuffer = 0f;
    private float NinetyDegreesToRad = Mathf.Deg2Rad(-90);
    private RayInfo groundData { get { return RayCastData.SurroundingCasts[RayDirections.Bottom]; } }
    // private bool groundColliding { get { return PlayerAreaSensor.GetArea(AreaSensorDirection.Bottom) || controller.ability.GetNoCollide(); } }
    private bool onFloor = false;
    public bool groundColliding { get { return onFloor || controller.ability.GetNoCollide(); } }
    private float maxSpeed = 1;
    private float maxAirSpeed = 0;
    private float accelerate = 1f, currentAccelerationTime = 6f;
    private bool jumpRequest = false, passThrough = false;
    private float jumpTimer, jumpStrRequest;
    public delegate void JumpRequest();
    private JumpRequest JumpAccepted;
    public delegate void VerticalChange(float amount);
    private VerticalChange vertCha;
    public void RegisterVerticalChange(VerticalChange function)
    {
        vertCha += function;
    }
    protected override void Setup(PlayerController controller, bool needsUpdate)
    {
        this.controller = controller;
        controller.AddToPhysicsUpdate(Update);
    }
    public Vector3 GetVerticalMove()
    {
        return verticalMove;
    }

    public Vector3 GetGroundMove()
    {
        return groundMove;
    }
    public override void Update(float delta)
    {
        base.Update(delta);
        Vector3 finalMove = Vector3.Zero;
        Vector3 snapDown = Vector3.Zero;
        //All things that happen when the player is on the ground
        if (groundColliding)
        {
            //For when the player stops moving.
            if (!controller.inputs.moved)
            {
                if (currentSpeed < .1f)
                {
                    switch (controller.ability.GetCurrentState())
                    {
                        case PlayerState.sprinting:
                        case PlayerState.walking:
                            Stop();
                            SetState(PlayerState.standing);
                            break;
                        case PlayerState.crouch:
                            Stop();
                            break;
                    }
                }
                if (controller.ability.GetCurrentState() != PlayerState.slide)
                {
                    if (currentSpeed < .1f)
                        currentSpeed = 0;
                    else
                        currentSpeed -= currentSpeed * time * PlayerOptions.walkingDeceleration;
                }
            }
            else //This is what happens when they do move
            {
                //This section is for changing the players state
                if (controller.size.crouched)
                {
                    if (accelerate > 1.1f)
                    {
                        SetState(PlayerState.slide);
                    }
                    else
                    {
                        SetState(PlayerState.crouch);
                    }
                }
                else
                {
                    if (maxSpeed == PlayerOptions.playerMaxWalkingSpeed)
                    {
                        SetState(PlayerState.walking);
                    }
                    else
                    {
                        SetState(PlayerState.sprinting);
                    }
                }
            }

            //Testing whether we need to modify the angle that the player walks
            if (!groundData.colliding || controller.ability.GetNoCollide())
            {
                finalMove += stableMove.Normalized() * currentSpeed * accelerate;
                //This is the mod that allows walking up slopes needs to be zero'd out when not in use
                verticalAddition *= 0;
            }
            else
            {
                //The mod being calculated
                verticalAddition = groundData.normal.Cross(stableMove.Rotated(Vector3.Up, NinetyDegreesToRad)).Normalized();
                finalMove += verticalAddition * currentSpeed * accelerate;
            }
            //Used primarely quick bursts of speed but drops when on the ground
            horizontalAcc -= horizontalAcc * time * .9f;

            //Universal knockback that only starts to slow when on the ground
            if (knockback.Length() < .1f)
                knockback *= 0;
            else
            {
                knockback -= knockback * time * 2f;
            }

            if (verticalMove.Length() < 0.01f)
            {
                snapDown = Vector3.Down;
            }
        }
        else //No longer on the ground
        {
            //This sets the state to falling up and down so long as the player isnt' wall running or gliding
            if (controller.ability.GetCurrentState() != PlayerState.wallRunning && controller.ability.GetCurrentState() != PlayerState.glide)
            {
                if (verticalMove.y > 0)
                {
                    SetState(PlayerState.fallingUp);
                }
                else
                {
                    SetState(PlayerState.fallingDown);
                }
            }
            //For when the player is in the air and is moving
            finalMove += stableMove * accelerate;

            //Controlling the vertical movement of the player depending on the state they are currently in
            switch (controller.ability.GetCurrentState())
            {
                case PlayerState.wallRunning:
                    verticalMove += Vector3.Down * time * PlayerOptions.wallRunningGravity;
                    break;
                case PlayerState.glide:
                    verticalMove = Vector3.Down * PlayerOptions.glideFallSpeed;
                    break;
                default:
                    verticalMove += Vector3.Down * time * PlayerOptions.gravity;
                    break;
            }
        }
        //Some edge cases where the player is on the ground and gets hit with knockback or something that modifies vertical move
        if (PlayerAreaSensor.GetArea(AreaSensorDirection.Above) && verticalMove.y > 0)
            verticalMove = Vector3.Zero;
        finalMove += horizontalAcc + verticalMove + pushing + knockback;
        //Moves the player finally
        controller.MoveAndSlideWithSnap(finalMove, snapDown, Vector3.Up, true);
        //Will only work if you have a force applying downward to the character?
        if (controller.IsOnFloor() != onFloor)
        {
            onFloor = controller.IsOnFloor();
            controller.GroundChanging(onFloor);
            floorMovement.y = controller.GetFloorVelocity().y;
        }
        if (currentAccelerationTime < PlayerOptions.slideMaxTime)
        {
            currentAccelerationTime += time;
        }
        else
        {
            accelerate = Mathf.Clamp(accelerate - PlayerOptions.slideDeceleration * time, 1, accelerate);
        }
        pushing = Vector3.Zero;
        if (controller.ability.GetCurrentState() != PlayerState.slide)
        {
            if (moved)
            {
                movedBuffer += 1;
                if (movedBuffer > 60f - Engine.GetFramesPerSecond())
                {
                    moved = false;
                    movedBuffer = 0f;
                }
            }
        }

    }

    public void GroundMovement(Vector3 direction, float maxSpeed, float acceleration)
    {
        if (!moved && groundColliding)
        {
            fallbackMovement = stableMove;
            stableMove = Vector3.Zero;
        }
        moved = true;
        if (groundColliding)
        {
            stableMove += direction;
            this.maxSpeed = maxSpeed;
            currentSpeed = Mathf.Clamp(direction.Length() * controller.GetProcessTime() * acceleration + currentSpeed, 0, 100);
        }
    }

    public void AirMovement(Vector3 direction, float control)
    {
        moved = true;
        if (stableMove == Vector3.Zero)
            stableMove = fallbackMovement;
        stableMove += direction * time * control * (Mathf.Clamp(currentSpeed, 1, maxAirSpeed) / Mathf.Clamp(maxAirSpeed, 1, 100));
        if (stableMove.Length() > maxAirSpeed)
            stableMove *= (maxAirSpeed / stableMove.Length());
    }

    public void Stop()
    {
        verticalMove = Vector3.Zero;
        horizontalAcc = Vector3.Zero;
        stableMove = Vector3.Zero;
        pushing = Vector3.Zero;
        knockback = Vector3.Zero;
        accelerate = 1f;
        currentSpeed = 0;
        controller.soundRequest.StopOneSound(PlayerSoundFrom.feet);
    }

    public void Accelerate(float amount = 1)
    {
        accelerate = amount;
        if (accelerate != 1)
        {
            currentAccelerationTime = 0;
        }

    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public float GetAccelerate()
    {
        return accelerate;
    }
    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public void HorizontalAccelerationSet(Vector3 vec)
    {
        horizontalAcc = vec;
    }

    public void SetPushing(Vector3 vec)
    {
        pushing = vec;
    }
    public float GetFallVerticalSpeed()
    {
        return verticalMove.Length();
    }

    public float GetHorAcceleration()
    {
        return horizontalAcc.Length();
    }

    public void VerticalIncrease(float amount)
    {
        verticalMove = (Vector3.Up * amount) + verticalAddition * Vector3.Up;
        if (onFloor)
        {
            verticalMove += controller.GetFloorVelocity();
        }
        SetState(PlayerState.fallingUp);
        vertCha?.Invoke(verticalMove.y);
    }

    public void AddVer(Vector3 additive)
    {
        verticalMove += additive;
    }

    public void SetKnockback(Vector3 k)
    {
        if (groundColliding)
        {
            knockback = k;
            stableMove *= 0.1f;
            verticalMove = Vector3.Zero;
        }
        else
        {
            knockback = k * (Vector3.One - Vector3.Up);
            verticalMove = k * Vector3.Up;
        }
    }

    public void ChangeStableVectorDirection(Vector3 direction)
    {
        float speed = stableMove.Length();
        stableMove = direction.Normalized();
        stableMove *= speed;
    }

    private void SetState(PlayerState state)
    {
        controller.ability.ChangeState(state);
    }

    public void LandingSignal(bool state)
    {
        if (controller.ability.GetNoCollide())
            return;
        if (state)
        {
            if (verticalMove.y > 0)
                return;
            vertCha?.Invoke(verticalMove.y);
            currentSpeed = stableMove.Length() + horizontalAcc.Length();

            if (currentSpeed > .1f)
            {
                SetState(PlayerState.walking);
                horizontalAcc = Vector3.Zero;
                verticalMove = Vector3.Zero;
                knockback = Vector3.Zero;
            }
            else
            {
                SetState(PlayerState.standing);
                Stop();
            }

        }
        else
        {
            maxAirSpeed = currentSpeed;
            stableMove = stableMove.Normalized() * currentSpeed;
            if (knockback.Length() > .1f)
            {
                verticalMove = knockback * Vector3.Up;
                knockback *= (Vector3.One - Vector3.Up);
            }
        }
    }
}
