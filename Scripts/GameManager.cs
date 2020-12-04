using Godot;
using System;
using Godot.Collections;

//This is such a fucking mess that I need to fix and redo when the other parts of the game start to come into play
public class GameManager : Node
{
    public static string StartingLevelPath = "res://Scenes/InGameLevels/TreeLevels/CrashLandingLevels/CrashLanding.tscn";
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    public Node Root { get { return GetTree().Root; } }
    public bool playing = false;
    [Signal]
    public delegate void ToggleGame(bool state);
    [Signal]
    public delegate void ReturnToTitle();
    [Signal]
    public delegate void CutSceneSwitch(bool cutsceneEnabled);
    //This is going to be the options menu
    private string savePath = "user://saveData", savePathEnd = ".save";
    public string startingAreaPath;
    public SavedData[] datas = new SavedData[3];
    public bool AllDataLoaded = false, allowInputs = true;
    public MenuBase currentMenu;
    private int dataUsed = -1;
    private static bool inCutscene = false;
    public static bool InCutscene()
    {
        return inCutscene;
    }

    public static void ToggleCutscene()
    {
        PlayerController.Instance.ToggleCamera(inCutscene);
        inCutscene = !inCutscene;
        GameManager.instance.EmitSignal("CutSceneSwitch", inCutscene);
    }

    public SavedData GetDataUsed()
    {
        if (dataUsed == -1)
            return new SavedData();
        return datas[dataUsed];
    }

    public override void _Ready()
    {
        instance = this;
        datas[0] = SavingAndLoading.LoadingGameSaveData(savePath + "1" + savePathEnd);
        datas[1] = SavingAndLoading.LoadingGameSaveData(savePath + "2" + savePathEnd);
        datas[2] = SavingAndLoading.LoadingGameSaveData(savePath + "3" + savePathEnd);
        SavingAndLoading.LoadingOptionsSaveData("user://Option.save");
        AllDataLoaded = true;
        SettingsOptions.RegisterUpdatedEvent(UpdateGameSettings);
    }

    public void StartGame(int i)
    {
        dataUsed = i;
        StartGame(datas[i].SavedInPath[SavedData.SavedAreaPath].ToString());
    }

    public void StartGame(string loadArea = "res://Scenes/WhiteBoxes/TreeLevelWhiteBox.tscn")
    {
        startingAreaPath = loadArea;
        GetTree().ChangeScene("res://Scenes/WorldManager.tscn");
    }

    public void UpdateGameSettings()
    {
        Engine.TargetFps = Mathf.RoundToInt(SettingsOptions.GetSetting<float>(SettingsNames.FPS));
        OS.VsyncEnabled = SettingsOptions.GetSetting<bool>(SettingsNames.VSync);
    }

    //Needs to be checked if in an actual game or can be done on the player as a call
    public void ToggleGamePause()
    {
        if (!PlayerController.CharacterPlaying())
            return;
        playing = !playing;
        if (playing)
        {
            GetTree().Paused = false;
            Input.SetMouseMode(Input.MouseMode.Captured);
        }
        else
        {
            GetTree().Paused = true;
            Input.SetMouseMode(Input.MouseMode.Visible);
        }
        EmitSignal(nameof(ToggleGame), playing);
    }

    public void SetToPlay()
    {
        if (!PlayerController.CharacterPlaying())
            return;
        playing = true;
        GetTree().Paused = false;
        Input.SetMouseMode(Input.MouseMode.Captured);
        EmitSignal(nameof(ToggleGame), playing);
    }

    public void QuitToMenu()
    {
        GetTree().Paused = false;
        Input.SetMouseMode(Input.MouseMode.Visible);
        GetTree().ChangeScene("res://Scenes/Menus/MainMenu.tscn");
        EmitSignal(nameof(ReturnToTitle));
        dataUsed = -1;
    }

    public void QuitGame()
    {
        GetTree().Quit();
    }

    public void SaveGame(string location)
    {
        if (PlayerController.Instance != null)
        {
            Dictionary holder = WorldManager.instance.GetWorldInfo();
            AddToDic(holder, SavedData.SavedAreaPath, location);
            if (holder.Contains(SavedData.SavedGameName))
            {
                if (holder[SavedData.SavedGameName].Equals("New Game"))
                {
                    AddToDic(holder, SavedData.SavedGameName, "Save Game " + (dataUsed + 1));
                }
            }
            SavingAndLoading.SavingGameData(savePath + (dataUsed + 1).ToString() + ".save", PlayerController.Instance.upgrades.GetAllUpgrades(), holder);
            ToggleGamePause();
        }
    }

    private void AddToDic(Dictionary addingTo, object key, object value)
    {
        if (addingTo.Contains(key))
        {
            addingTo[key] = value;
        }
        else
        {
            addingTo.Add(key, value);
        }
    }
}
