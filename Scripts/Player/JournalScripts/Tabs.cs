using Godot;
using System.Collections.Generic;

public class Tabs : MeshInstance
{
    [Export]
    private JournalSections section;
    [Export]
    private string tabName = "";

    private SpatialMaterial tabColor;
    [Export]
    private Color highlighted, unhighlighted, currentlyOn, empty;
    private bool onPage = false;
    private SortedList<string, ScanInfoResource> pages = new SortedList<string, ScanInfoResource>();
    private int currentPage = 0;

    public override void _Ready()
    {
        tabColor = GetSurfaceMaterial(0) as SpatialMaterial;
        tabColor.AlbedoColor = empty;
        CallDeferred(nameof(DeferredReady));
    }

    public void DeferredReady()
    {
        PlayersJournal.instance.AddTabToBook(this);
    }

    public void MouseEnter(bool state)
    {
        if (onPage || !HasInfo())
            return;
        if (state)
        {
            tabColor.AlbedoColor = highlighted;
        }
        else
        {
            tabColor.AlbedoColor = unhighlighted;
        }
    }

    public void OnTab(bool state)
    {
        onPage = state;
        if (state)
        {
            tabColor.AlbedoColor = currentlyOn;
        }
        else
        {
            if (HasInfo())
                tabColor.AlbedoColor = unhighlighted;
            else
                tabColor.AlbedoColor = empty;
        }
    }

    public void SetToLastPage()
    {
        if (HasInfo())
        {
            currentPage = pages.Count - pages.Count % 2;
        }
    }

    public bool HasInfo()
    {
        return pages.Count > 0;
    }

    public void AddPage(ScanInfoResource page)
    {
        pages.Add(page.GetScanName(), page);
        tabColor.AlbedoColor = unhighlighted;
    }

    public ScanInfoResource GetPageCurrentInfo()
    {
        return pages[pages.Keys[currentPage]];
    }

    public ScanInfoResource GetNextPageInfo()
    {
        if (pages.Count <= currentPage + 1)
        {
            return null;
        }
        return pages[pages.Keys[currentPage + 1]];
    }

    public bool FlipPages(bool reverse)
    {
        if (reverse)
        {
            if (currentPage - 2 < 0)
            {
                return false;
            }
            currentPage -= 2;
        }
        else
        {
            if (currentPage + 2 >= pages.Count)
            {
                return false;
            }
            currentPage += 2;
        }
        return true;
    }

    public int GetCurrentPage()
    {
        return currentPage;
    }

    public int GetOrder()
    {
        return (int)section;
    }
    public string GetTabName()
    {
        return tabName;
    }
}
