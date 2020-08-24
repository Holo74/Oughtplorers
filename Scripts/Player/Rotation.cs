using Godot;
using System;

//I was lazy so I made a more complicated solution to have the camera rotate with the mouse, it should work though
public class Rotation : BaseAttatch
{
    private bool xRotation, rotationLimit;
    private float currentRotation, minRotation, maxRotation;
    private Spatial source;
    public Rotation(PlayerController controller, bool rotateOnX, Spatial source, bool rotationLimit = false, float minDegreeRotation = 0, float maxDegreeRotation = 0) : base(controller, false)
    {
        this.rotationLimit = rotationLimit;
        minRotation = Mathf.Deg2Rad(minDegreeRotation);
        maxRotation = Mathf.Deg2Rad(maxDegreeRotation);
        this.source = source;
        xRotation = rotateOnX;
    }

    public void LookForward(Vector3 rotation)
    {
        currentRotation = 0;
        source.Rotation = rotation;
    }

    public void RotateAmount(float amount)
    {
        if (rotationLimit)
        {
            if (amount + currentRotation > maxRotation || amount + currentRotation < minRotation)
            {
                amount = Mathf.Clamp(amount + currentRotation, minRotation, maxRotation) - currentRotation;
            }
            currentRotation += amount;
        }
        if (xRotation)
        {
            source.RotateX(amount);
        }
        else
        {
            source.RotateY(amount);
        }
    }
}
