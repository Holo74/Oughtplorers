using Godot;
using Godot.Collections;
using System;

public class SavingAndLoading
{
    public static void SavingOptionData(string savePath)
    {
        File saveData = new File();
        saveData.Open(savePath, File.ModeFlags.Write);
        saveData.StoreLine(JSON.Print(SettingsOptions.GetData()));
        saveData.Close();
    }

    public static void LoadingOptionsSaveData(string savePath)
    {
        File saveData = new File();
        if (saveData.FileExists(savePath))
        {
            saveData.Open(savePath, File.ModeFlags.Read);
            Dictionary settingsOption = (Dictionary)JSON.Parse(saveData.GetLine()).Result;
            SettingsOptions.SetData(settingsOption);
        }
        saveData.Close();
    }

    public static void SavingGameData(string savePath, Dictionary playerUpgrades, Dictionary additionalInfo)
    {
        File saveData = new File();
        saveData.Open(savePath, File.ModeFlags.Write);
        saveData.StoreLine(JSON.Print(playerUpgrades));
        saveData.StoreLine(JSON.Print(additionalInfo));
        saveData.Close();
    }

    public static SavedData LoadingGameSaveData(string savePath)
    {
        File saveData = new File();
        SavedData returning;
        if (saveData.FileExists(savePath))
        {
            saveData.Open(savePath, File.ModeFlags.Read);
            Dictionary playerUpgrades = (Dictionary)JSON.Parse(saveData.GetLine()).Result;
            Dictionary additionalInfo = (Dictionary)JSON.Parse(saveData.GetLine()).Result;
            returning = new SavedData(playerUpgrades, additionalInfo);
        }
        else
        {
            returning = new SavedData();
        }
        saveData.Close();
        return returning;
    }
}
