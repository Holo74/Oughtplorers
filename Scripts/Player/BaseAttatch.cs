using Godot;
using System;

//Pretty much the base thing that I wrote so that I didn't need to write controller 700 times
public abstract class BaseAttatch
{
    protected float time = 0;
    protected PlayerController controller;
    public BaseAttatch(PlayerController controller, bool needsUpdate)
    {
        Setup(controller, needsUpdate);
    }

    protected virtual void Setup(PlayerController controller, bool needsUpdate)
    {
        this.controller = controller;
        if (needsUpdate)
            controller.AddToUpdate(Update);
    }

    public virtual void Update(float delta) { time = delta; }
}
