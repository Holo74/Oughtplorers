using Godot;
using System;

public class UpgradePickupBase : Spatial
{
    [Export]
    private UpgradesValues upgradeName = UpgradesValues.DoubleJump;
    [Export]
    private string name = "", description = "";

    private Spatial mesh;
    public override void _Ready()
    {
        string upgrade = PlayerUpgrade.EnumToStringUpgrades(upgradeName);
        if (WorldManager.instance.WorldInfoHas(upgrade))
        {
            if (WorldManager.instance.GetWorldInfoData<bool>(upgrade))
            {
                QueueFree();
                return;
            }
        }
        GetChild<Area>(0).Connect("body_entered", this, nameof(UpgradeCollected));
        GetChild<AnimationPlayer>(2).CurrentAnimation = "Hovering";
    }

    private void UpgradeCollected(Node body)
    {
        if (body.Name.Equals("Player"))
        {
            string upgrade = PlayerUpgrade.EnumToStringUpgrades(upgradeName);
            if (PlayerController.Instance.upgrades.GetUpgrade(upgrade))
            {
                QueueFree();
                return;
            }
            PlayerController.Instance.upgrades.SetUpgradeTo(upgrade, true);
            WorldManager.instance.AddToWorldInfo(upgrade, true);
            InGameMenu.Instance.GetUpgrade(name, description);
            QueueFree();
        }
    }

    public void MoveUpgrade(Vector3 offset)
    {
        GlobalTranslate(offset);
    }
}
