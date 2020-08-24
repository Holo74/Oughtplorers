using Godot;
using System.Collections.Generic;

public class MenuBase : Control
{
    public Control mainNode;
    private Queue<Control> addonMenus = new Queue<Control>();
    public void MakeMainMenu(Control parent)
    {
        mainNode.Visible = false;
        while (addonMenus.Count > 0)
        {
            addonMenus.Dequeue().Visible = false;
        }
        mainNode = parent;
        mainNode.Visible = true;
    }

    public void AddonToCurrent(Control parent)
    {
        parent.Visible = true;
        addonMenus.Enqueue(parent);
    }
}
