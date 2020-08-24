using Godot;
using Godot.Collections;
using System;

public class StartGameAs : Button
{
    [Export]
    private int saveSlot;
    [Export]
    private bool useDemoLevels = false;
    [Export]
    private string pathwayToDemoLevel = "", textUsed = "";
    public override void _Ready()
    {
        if (!useDemoLevels)
        {
            Text = GameManager.Instance.datas[saveSlot].SavedInPath[SavedData.SavedGameName].ToString();
            Connect("pressed", this, nameof(LoadSave));
        }
        else
        {
            Text = textUsed;
            Connect("pressed", this, nameof(LoadDemo));
        }
    }

    private void LoadSave()
    {
        GameManager.Instance.StartGame(saveSlot);
    }

    private void LoadDemo()
    {
        GameManager.Instance.StartGame(pathwayToDemoLevel);
    }
}
