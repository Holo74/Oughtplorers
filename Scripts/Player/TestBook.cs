using Godot;
using System;

public class TestBook : Spatial
{
    bool mouseIn = false, bookActivated = false;

    public void MouseEnter(bool state)
    {
        mouseIn = state;
        GD.Print(state + " This is the new state");
    }
    
    public void ActivateBook()
    {
        if (PlayerController.Instance.playMovement.GetCurrentSpeed() > 0.1f || !PlayerController.Instance.playMovement.groundColliding)
            return;
        bookActivated = !bookActivated;
        if (bookActivated)
        {
            GameManager.Instance.allowInputs = false;
            Input.SetMouseMode(Input.MouseMode.Visible);
        }
        else
        {
            GameManager.Instance.allowInputs = true;
            Input.SetMouseMode(Input.MouseMode.Captured);
        }
    }

    public void MouseEvent(Node camera, InputEvent @event, Vector3 clickPosition, Vector3 clickNormal, int shapeIdX)
    {
        GD.Print("Testing");
        if (@event is InputEventMouseButton click)
        {
            GD.Print("Testing event passed");
        }
    }

}
