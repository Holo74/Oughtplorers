using Godot;
using System;

//This is the base player class that is used to connect all the classes of the player and to house all of the nodes that are children of this node
public class PlayerController : HealthKinematic
{
    private float timeDelta;
    public delegate void Updating(float delta);
    private event Updating update, physicsUpdate;
    public PlayerOptions options { get; private set; }
    public Rotation headRotation { get; private set; }
    public Rotation bodyRotation { get; private set; }
    public Momentum playMovement { get; private set; }
    public SizeHandler size { get; private set; }
    public PlayerAbility ability { get; private set; }
    public PlayerInput inputs { get; private set; }
    public CameraRotHandler camRot { get; private set; }
    public PlayerUpgrade upgrades = new PlayerUpgrade();
    public Camera camera;
    public WeaponController gunCamera;
    [Export]
    private NodePath gunPath;
    [Signal]
    public delegate void UpdateHealth(bool damaged, float health);
    public delegate void Died();
    private Died death;
    public static PlayerController Instance { get; private set; }
    public delegate void PlayerDamaged(float amount, DamageType type, EnemyProjectiles projectile);
    private PlayerDamaged playerTookDamage;
    private bool characterReady = false;
    private Spatial equipedWeapon;
    public Spatial fireFromLocations;
    public bool playerTrappedInRoom = false;
    public SpotLight headLamp;
    public SoundManager soundRequest;
    public AnimationController anim;
    public PlayersJournal bookMenu;

    public void ToggleCamera(bool state)
    {
        camera.Current = state;
        gunCamera.Current = state;
    }

    public void ReadyPlayer(Transform trans)
    {
        GlobalTransform = new Transform(trans.basis, trans.origin);
        characterReady = true;
        gunCamera.SetCurrentWeapon(CurrentWeaponEquiped.none);
    }

    public override void _Ready()
    {
        Instance = this;
        headRotation = new Rotation(this, true, GetChild<Spatial>(2).GetChild<Spatial>(0), true, -80, 85);
        camera = GetChild<Spatial>(2).GetChild<Spatial>(0).GetChild<Camera>(0);
        headLamp = camera.GetParent().GetChild<SpotLight>(2);
        bodyRotation = new Rotation(this, false, this);
        playMovement = new Momentum(this);
        playMovement.RegisterVerticalChange(HardLanding);
        anim = GetChild<AnimationController>(8);
        size = new SizeHandler(this, GetChild<Spatial>(2));
        ability = new PlayerAbility(this);
        inputs = new PlayerInput(this);
        camRot = new CameraRotHandler(this);
        fireFromLocations = GetChild(2).GetChild(0).GetChild<Spatial>(1);
        CallDeferred(nameof(DeferedSetup));
    }

    private void DeferedSetup()
    {
        options = PlayerOptions.Instance;
        bookMenu = PlayersJournal.instance;
        soundRequest = SoundManager.instance;
        gunCamera = WeaponController.instance;
        SettingsOptions.RegisterUpdatedEvent(UpdateCharacterSettings);
        upgrades.LoadUpgrades(GameManager.Instance.GetDataUsed().upgrades.GetAllUpgrades());
        foreach (string s in GameManager.Instance.GetDataUsed().scans)
        {
            GD.Print(s);
            PlayersJournal.instance.AddPageToBook(AutoLoadScanInfo.PullInfo(s));
        }
        InputHandler.Instance.ConnectToMouseMovement(this, nameof(Rotating));
        Init(100);
    }

    public void UpdateCharacterSettings()
    {
        camera.Fov = SettingsOptions.GetSetting<float>(SettingsNames.cameraFOV);
    }

    public override void _Process(float delta)
    {
        if (GameManager.Instance.playing && characterReady)
        {
            if (!IsDead())
            {
                update?.Invoke(delta);
                timeDelta = delta;
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (GameManager.Instance.playing && characterReady)
            if (!IsDead())
                physicsUpdate?.Invoke(delta);
    }

    public static bool CharacterPlaying()
    {
        if (Instance == null)
            return false;
        return Instance.characterReady;
    }

    public void AddToUpdate(Updating adding)
    {
        update += adding;
    }

    public void AddToPhysicsUpdate(Updating adding)
    {
        physicsUpdate += adding;
    }

    public void Rotating(Vector2 vec)
    {
        if (inputs != null)
            if (GameManager.Instance.playing && characterReady)
                if (!IsDead())
                    inputs.Rotating(vec);
    }

    public void GroundChanging(bool state)
    {
        if (playMovement.GetVerticalMove().y > 0 && state)
            return;
        ability.Land(state);
        playMovement.LandingSignal(state);
    }

    public override bool TakeDamage(float damage, DamageType typing, Node source)
    {
        if (IsDead())
        {
            return false;
        }
        if (source != null)
        {
            if (source is EnemyProjectiles projectiles)
            {
                playerTookDamage?.Invoke(damage, typing, projectiles);
                projectiles.Remove();
            }
        }
        Damaged(damage);
        if (IsDead())
        {
            death?.Invoke();
            playerTrappedInRoom = false;
        }
        else
        {
            EmitSignal(nameof(UpdateHealth), true, GetHealth());
        }
        return true;
    }

    public override bool Heal(float amount)
    {
        if (IsDead())
            return false;
        base.Heal(amount);
        EmitSignal(nameof(UpdateHealth), false, GetHealth());
        return true;
    }

    public void AttachToDeath(Died d)
    {
        death += d;
    }

    public void PlayerTookDamageAdding(PlayerDamaged registered)
    {
        playerTookDamage += registered;
    }

    public void DeloadPlayer()
    {
        Instance = null;
        SettingsOptions.DeRegisterEvent(UpdateCharacterSettings);
    }

    public void HardLanding(float amount)
    {
        if (amount < -15f)
        {
            TakeDamage((Mathf.Pow(amount + 15f, 2f)), DamageType.fall, null);
        }
    }

    public void SetPosAndRot(Transform pos)
    {
        headRotation.LookForward(Vector3.Zero);
        GlobalTransform = new Transform(pos.basis, pos.origin);
    }

    public void ToggleMenuFromDeath()
    {
        GameManager.Instance.ToggleGamePause();
    }

    public float GetProcessTime()
    {
        return timeDelta;
    }
}
