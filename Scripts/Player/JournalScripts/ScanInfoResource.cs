using Godot;
using System;

public class ScanInfoResource : Resource
{
    [Export]
    private string Name = "";
    public string GetScanName()
    {
        return Name;
    }
    [Export]
    private JournalSections section = JournalSections.Creatures;
    public JournalSections GetSections()
    {
        return section;
    }
    [Export]
    private Texture pageTexture;
    public Texture GetPageTexture()
    {
        return pageTexture;
    }
}
