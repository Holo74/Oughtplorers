using Godot;
using System;

public class LoadToTitleScreen : Node
{


    public override void _Process(float delta)
    {
        if (GameManager.Instance.AllDataLoaded)
        {
            GetTree().ChangeScene("res://Scenes/Menus/MainMenu.tscn");
        }
    }
}
