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
        scans = new Godot.Collections.Array();
    }
    public SavedData(Dictionary playerUpgrades, Dictionary additionalInfo, Godot.Collections.Array scans)
    {
        upgrades = new PlayerUpgrade(playerUpgrades);
        SavedInPath = additionalInfo;
        this.scans = scans;
    }

    public PlayerUpgrade upgrades;
    public Dictionary SavedInPath;
    public Godot.Collections.Array scans;
}
