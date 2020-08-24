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
    private Vector3 fallbackMovement = new Vector3();
    private float currentSpeed = 0;
    private bool moved = false;
    private float NinetyDegreesToRad = Mathf.Deg2Rad(-90);
    private RayInfo groundData { get { return RayCastData.SurroundingCasts[RayDirections.Bottom]; } }
    private bool groundColliding { get { return PlayerAreaSensor.GetArea(AreaSensorDirection.Bottom) || controller.ability.GetNoCollide(); } }
    private float maxSpeed = 1;
    private float maxAirSpeed = 0;
    private float accelerate = 1f, currentAccelerationTime = 6f;
    private bool jumpRequest = false;
    private float jumpTimer, jumpStrRequest;
    public delegate void JumpRequest();
    private JumpRequest JumpAccepted;
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
        if (groundColliding)
        {
            if (!moved)
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
            else
            {
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
                if (currentSpeed > maxSpeed)
                    currentSpeed = Mathf.Clamp(currentSpeed - time * PlayerOptions.playerDeccelerationAboveMax, maxSpeed, currentSpeed);
            }
            if (!groundData.colliding || controller.ability.GetNoCollide())
            {
                controller.MoveAndSlide(stableMove.Normalized() * currentSpeed * accelerate);
                verticalAddition *= 0;
            }
            else
            {
                verticalAddition = groundData.normal.Cross(stableMove.Rotated(Vector3.Up, NinetyDegreesToRad)).Normalized();
                controller.MoveAndSlide(verticalAddition * currentSpeed * accelerate);
            }
            horizontalAcc -= horizontalAcc * time * .9f;
        }
        else
        {
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
            controller.MoveAndSlide(stableMove * accelerate);
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
        if (jumpRequest && jumpTimer < .3f)
        {
            jumpTimer += time;
            if (!PlayerAreaSensor.GetArea(AreaSensorDirection.Above))
            {
                switch (controller.ability.GetCurrentState())
                {
                    case PlayerState.fallingDown:
                    case PlayerState.fallingUp:
                    case PlayerState.glide:
                        if (controller.ability.CanJump())
                        {
                            JumpFinished();
                        }
                        break;
                    case PlayerState.wallRunning:
                        JumpFinished();
                        controller.ability.WallRunningJumpUsed();
                        break;
                    default:
                        JumpFinished();
                        break;
                }
            }
        }
        if (PlayerAreaSensor.GetArea(AreaSensorDirection.Above) && verticalMove.y > 0)
            verticalMove = Vector3.Zero;
        controller.MoveAndSlide(verticalMove);
        controller.MoveAndSlide(horizontalAcc);
        controller.MoveAndSlide(pushing);
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
            moved = false;
        }

    }

    private void JumpFinished()
    {
        if (groundColliding)
            verticalMove = (Vector3.Up * jumpStrRequest) + verticalAddition * Vector3.Up;
        jumpRequest = false;
        SetState(PlayerState.fallingUp);
        JumpAccepted();
        JumpAccepted = null;
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
            currentSpeed = Mathf.Clamp(direction.Length() * time * acceleration + currentSpeed, 0, 100);
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
        accelerate = 1f;
        currentSpeed = 0;
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

    public void VerticalIncrease(float amount, JumpRequest function)
    {
        jumpRequest = true;
        jumpStrRequest = amount;
        jumpTimer = 0;
        JumpAccepted = function;
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
            if (verticalMove.y < -15f)
            {
                controller.TakeDamage((Mathf.Pow(verticalMove.y + 15f, 2f)), DamageType.fall, null);
            }
            currentSpeed = stableMove.Length() + horizontalAcc.Length();
            horizontalAcc = Vector3.Zero;
            verticalMove = Vector3.Zero;

            if (currentSpeed > .1f)
            {
                SetState(PlayerState.walking);
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
        }
    }
}
