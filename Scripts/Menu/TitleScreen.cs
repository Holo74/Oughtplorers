using Godot;
using Godot.Collections;
using System.Collections.Generic;

//The title screen that currently needs the options menu and loading menu and other fancy stuff
public class TitleScreen : MenuBase
{
    private bool dataLoaded = false;

    public override void _Ready()
    {
        GameManager.Instance.currentMenu = this;
        mainNode = GetChild<Control>(0);
        int count = GetChildCount();
        for (int i = 1; i < count; i++)
        {
            GetChild<Control>(i).Visible = false;
        }
    }

    public void PlayGame(int save)
    {
        GameManager.Instance.StartGame(save);
    }

    public void GoToDebug()
    {
        GameManager.Instance.StartGame();
    }
}
