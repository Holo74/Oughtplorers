using System;
using Godot.Collections;

public class SavedData
{
    public static string SavedAreaPath = "SavedAreaPath";
    public static string SavedGameName = "SavedGameName";
    public SavedData()
    {
        SavedInPath = new Dictionary();
        upgrades = new PlayerUpgrade();
        SavedInPath.Add(SavedAreaPath, GameManager.StartingLevelPath);
        SavedInPath.Add(SavedGameName, "New Game");
    }
    public SavedData(Dictionary playerUpgrades, Dictionary additionalInfo)
    {
        upgrades = new PlayerUpgrade(playerUpgrades);
        SavedInPath = additionalInfo;

    }

    public PlayerUpgrade upgrades;
    public Dictionary SavedInPath;
}
