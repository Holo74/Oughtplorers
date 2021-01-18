using Godot;
using System.Collections.Generic;

public class PlayersJournal : Node
{
    public static PlayersJournal instance;
    private bool mouseIn = false, bookActivated = false;
    private PlayerController controller { get { return PlayerController.Instance; } }
    private AnimationPlayer bookAnim;
    private int currentTabOrder = 0;
    private Tabs currentSection;
    private List<ScanInfoResource> allScans = new List<ScanInfoResource>();
    [Export]
    private NodePath rightPage, leftPage;
    private SpatialMaterial rightPageMat, leftPageMat;
    public Godot.Collections.Array scanNames = new Godot.Collections.Array();

    private Tabs[] allTabs = new Tabs[5];

    public bool ActiveJournal()
    {
        return bookActivated;
    }

    public void AddTabToBook(Tabs tabs)
    {
        if (allTabs[tabs.GetOrder()] == null)
        {
            allTabs[tabs.GetOrder()] = tabs;
        }
        else
        {
            GD.PrintErr("Tab has the same numbers of " + tabs.GetOrder() + ".\tThe names are " + allTabs[tabs.GetOrder()].GetTabName() + " and " + tabs.GetTabName());
        }
    }

    public override void _Ready()
    {
        bookAnim = GetChild<AnimationPlayer>(0);
        rightPageMat = GetNode<MeshInstance>(rightPage).GetSurfaceMaterial(0) as SpatialMaterial;
        leftPageMat = GetNode<MeshInstance>(leftPage).GetSurfaceMaterial(0) as SpatialMaterial;
        CallDeferred(nameof(DeferredReady));
        instance = this;
    }

    public void DeferredReady()
    {
        controller.anim.Connect(nameof(AnimationController.GunAnimationFinished), this, nameof(HolsterAnimationFinished));
        bookAnim.Connect("animation_finished", this, nameof(HolsterAnimationFinished));
    }

    public void ActivateBook()
    {
        if (controller.playMovement.GetCurrentSpeed() > 0.1f || !controller.playMovement.groundColliding)
            return;
        if (currentSection == null)
        {
            currentSection = allTabs[currentTabOrder];
            currentSection.OnTab(true);
        }
        bookActivated = !bookActivated;
        if (bookActivated)
        {
            GameManager.Instance.allowInputs = false;
            controller.anim.PlayGunAnimation("Holstering Temp");
        }
        else
        {
            bookAnim.Play("Hold Down");
        }
    }

    public void HolsterAnimationFinished(string name)
    {
        switch (name)
        {
            case "Holstering Temp":
                bookAnim.Play("Bring Up");
                break;
            case "Hold Down":
                controller.anim.PlayGunAnimation("Equiping");
                GameManager.Instance.allowInputs = true;
                Input.SetMouseMode(Input.MouseMode.Captured);
                break;
            case "Bring Up":
                Input.SetMouseMode(Input.MouseMode.Visible);
                break;
            case "FlipToTheLeft":
            case "FlipToTheRight":
                UpdatePages();
                break;
        }
    }

    public void AdvancePages(bool reverse)
    {
        if (currentSection.FlipPages(reverse))
        {
            if (reverse)
                bookAnim.Play("FlipToTheRight");
            else
                bookAnim.Play("FlipToTheLeft");
        }
        else
        {
            if (!(currentTabOrder == 0 && reverse) && !(currentTabOrder == 5 && !reverse))
            {
                int tab = ValidTab(reverse, currentTabOrder + (reverse ? -1 : 1));
                if (tab == -1)
                    return;
                SetCurrentTab(tab);
                if (reverse)
                    currentSection.SetToLastPage();
            }
        }
    }

    private int ValidTab(bool reverse, int place)
    {
        if (place >= 0 && place < 5)
        {
            if (allTabs[place].HasInfo())
            {
                return place;
            }
            return ValidTab(reverse, place + (reverse ? -1 : 1));
        }
        return -1;
    }

    private void UpdatePages()
    {
        if (currentSection != allTabs[currentTabOrder])
        {
            currentSection = allTabs[currentTabOrder];
        }
        leftPageMat.AlbedoTexture = currentSection.GetPageCurrentInfo().GetPageTexture();
        if (currentSection.GetNextPageInfo() != null)
            rightPageMat.AlbedoTexture = currentSection.GetNextPageInfo().GetPageTexture();
        else
            rightPageMat.AlbedoTexture = null;
    }

    public void SetCurrentTab(int tab)
    {
        if (allTabs[tab] == currentSection)
            return;
        allTabs[tab].OnTab(true);
        currentSection.OnTab(false);
        if (currentSection == null)
        {
            bookAnim.Play("FlipToTheLeft");
        }
        else
        {
            if (currentSection.GetOrder() < allTabs[tab].GetOrder())
            {
                bookAnim.Play("FlipToTheLeft");
            }
            else
            {
                bookAnim.Play("FlipToTheRight");
            }
        }
        currentTabOrder = allTabs[tab].GetOrder();
    }

    public void SetCurrentTab(Tabs tab)
    {
        SetCurrentTab(tab.GetOrder());
    }

    public void AddPageToBook(ScanInfoResource scan)
    {
        allTabs[(int)scan.GetSections()].AddPage(scan);
        scanNames.Add(AutoLoadScanInfo.AppendToFrontScan + scan.GetScanName());
    }
}

public enum JournalSections
{
    Index,
    Creatures,
    Flora,
    Structures,
    Items,

}