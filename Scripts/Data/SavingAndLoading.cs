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

    public static void SavingGameData(string savePath, Dictionary playerUpgrades, Dictionary additionalInfo, Godot.Collections.Array scans)
    {
        File saveData = new File();
        saveData.Open(savePath, File.ModeFlags.Write);
        saveData.StoreLine(JSON.Print(playerUpgrades));
        saveData.StoreLine(JSON.Print(additionalInfo));
        saveData.StoreLine(JSON.Print(scans));
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
            Godot.Collections.Array scans = (Godot.Collections.Array)JSON.Parse(saveData.GetLine()).Result;
            GD.Print(scans + " These are the scans loading");
            returning = new SavedData(playerUpgrades, additionalInfo, scans);
        }
        else
        {
            returning = new SavedData();
        }
        saveData.Close();
        return returning;
    }
}
