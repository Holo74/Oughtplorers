using Godot;
using System;

//Used for the player to crouch
public class SizeHandler : BaseAttatch
{
    private Vector3 headMinPos = new Vector3(0, .3f, 0), headMaxPos = new Vector3(0, 1f, 0);
    private Spatial headPos;
    private bool atPosition = true;
    private Vector3 GetNeededPos { get { return crouched ? headMinPos : headMaxPos; } }
    public bool crouched { get; private set; }
    public SizeHandler(PlayerController controller, Spatial head) : base(controller, true)
    {
        headPos = head;
        crouched = false;
        controller.playMovement.RegisterVerticalChange(HardLanding);
    }

    public override void Update(float delta)
    {
        if (!atPosition)
        {
            if (!crouched && RayCastData.SurroundingCasts[RayDirections.HeadSpace].colliding)
                Crouch();

            Vector3 difference = GetNeededPos - headPos.Translation;
            headPos.Translation += difference * PlayerOptions.playerCrouchSpeed * delta;
            if (Mathf.Abs(headPos.Translation.y - GetNeededPos.y) < .01f)
            {
                atPosition = true;
                headPos.Translation = GetNeededPos;
            }
        }
    }

    private void HardLanding(float amount)
    {
        if (amount < -15 && !crouched)
        {
            Crouch();
        }
    }

    public void Crouch()
    {
        atPosition = false;
        crouched = !crouched;
    }
}
