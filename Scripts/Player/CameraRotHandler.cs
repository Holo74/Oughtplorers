using Godot;
using System;

//Poor name for being used only for wall running
public class CameraRotHandler : BaseAttatch
{
    private bool rotating = false;
    private bool leaning = false;
    private float rotationAmount = 20f, currentRotation = 0f, rotatingFrom = 0;
    private bool leanRight = false;
    public CameraRotHandler(PlayerController controller) : base(controller, true)
    {
        controller.ability.AddToStateChange(WallRunning);
    }

    private void WallRunning(PlayerState state)
    {

        if (!leaning && state == PlayerState.wallRunning)
        {
            if (!SettingsOptions.leanWhileRunning)
                return;
            leaning = !leaning;
            rotating = true;
            if (controller.ability.RunningOnRightWall)
            {
                leanRight = false;
            }
            else
            {
                leanRight = true;
            }
            leaningTime = 0;
        }
        if (leaning && state != PlayerState.wallRunning)
        {
            leaning = false;
            rotating = true;
            leanRight = !leanRight;
            leaningTime = 0;
            rotatingFrom = currentRotation;
        }
    }
    private float leaningTime = 0;

    public override void Update(float delta)
    {
        if (rotating)
        {
            float amount = 0;
            leaningTime += delta * 2;
            if (leaning)
            {
                if (leanRight)
                {
                    amount = Mathf.Lerp(0, -rotationAmount, leaningTime) - currentRotation;
                }
                else
                {
                    amount = Mathf.Lerp(0, rotationAmount, leaningTime) - currentRotation;
                }
            }
            else
            {
                amount = Mathf.Lerp(rotatingFrom, 0, leaningTime) - currentRotation;
            }

            currentRotation += amount;
            controller.camera.RotateObjectLocal(controller.camera.Transform.basis.z, Mathf.Deg2Rad(amount));
            if (leaningTime > 1)
            {
                rotating = false;
            }
        }
    }
}
