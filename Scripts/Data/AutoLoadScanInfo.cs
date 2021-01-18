using Godot;
using System.Collections.Generic;

public class AutoLoadScanInfo : Node
{
    public const string AppendToFrontScan = "scan:";
    [Export]
    private ScanInfoResource[] completeScanList;
    private static Dictionary<string, ScanInfoResource> pullList = new Dictionary<string, ScanInfoResource>();
    public override void _Ready()
    {
        foreach (ScanInfoResource scan in completeScanList)
        {
            pullList.Add(AppendToFrontScan + scan.GetScanName(), scan);
            GD.Print(AppendToFrontScan + scan.GetScanName() + " This is in data");
        }
    }

    public static ScanInfoResource PullInfo(string name)
    {
        if (pullList.ContainsKey(name))
        {
            return pullList[name];
        }
        GD.PrintErr("Scan info " + name + " is not in the database");
        return null;
    }
}
