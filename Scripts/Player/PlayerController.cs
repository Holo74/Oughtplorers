using Godot;
using System;

//This is the base player class that is used to connect all the classes of the player and to house all of the nodes that are children of this node
public class PlayerController : HealthKinematic
{
    private float timeDelta;
    public delegate void Updating(float delta);
    private event Updating update, physicsUpdate;
    private Spatial[] weapons = new Spatial[4];
    public PlayerOptions options { get; private set; }
    public Rotation headRotation { get; private set; }
    public Rotation bodyRotation { get; private set; }
    public Momentum playMovement { get; private set; }
    public SizeHandler size { get; private set; }
    public PlayerAbility ability { get; private set; }
    public PlayerInput inputs { get; private set; }
    public CameraRotHandler camRot { get; private set; }
    public PlayerSoundControl soundControl { get; private set; }
    public PlayerUpgrade upgrades = new PlayerUpgrade();
    public Camera camera;
    [Export]
    private NodePath headPath, headRotationPath, cameraPath, gunPath;
    [Signal]
    public delegate void TakingDamage();
    public delegate void Died();
    private Died death;
    public static PlayerController Instance { get; private set; }
    public delegate void PlayerDamaged(float amount, DamageType type, EnemyProjectiles projectile);
    private PlayerDamaged playerTookDamage;
    private bool characterReady = false;
    public AudioStreamPlayer playerSound { private set; get; }
    public AnimationTree animationNode { private set; get; }
    public PlayerAnimationController animationController { private set; get; }
    private Spatial equipedWeapon;
    public Spatial fireFromLocations;

    public void ReadyPlayer(Transform trans)
    {
        GlobalTransform = new Transform(trans.basis, trans.origin);
        characterReady = true;
    }

    public override void _Ready()
    {
        Instance = this;
        options = PlayerOptions.Instance;
        headRotation = new Rotation(this, true, GetChild<Spatial>(2).GetChild<Spatial>(0), true, -80, 85);
        camera = GetChild<Spatial>(2).GetChild<Spatial>(0).GetChild<Camera>(0);
        playerSound = GetChild<AudioStreamPlayer>(5);
        animationNode = GetChild<AnimationTree>(7);
        bodyRotation = new Rotation(this, false, this);
        InputHandler.Instance.ConnectToMouseMovement(this, nameof(Rotating));
        playMovement = new Momentum(this);
        playMovement.RegisterVerticalChange(HardLanding);
        size = new SizeHandler(this, GetChild<Spatial>(2));
        PlayerAreaSensor.GetPlayerSensor(AreaSensorDirection.Bottom).RegisterStateChange(this, nameof(GroundChanging));
        ability = new PlayerAbility(this);
        inputs = new PlayerInput(this);
        camRot = new CameraRotHandler(this);
        soundControl = new PlayerSoundControl(this);
        animationController = new PlayerAnimationController(this);
        Init(100);
        for (int i = 0; i < 4; i++)
        {
            weapons[i] = GetNode(gunPath).GetChild<Spatial>(i);
            weapons[i].Scale = Vector3.Zero;
        }
        fireFromLocations = GetChild<Spatial>(2).GetChild<Spatial>(0).GetChild<Spatial>(1);
        ability.AddToWeaponChange(WeaponChanged);
        SettingsOptions.RegisterUpdatedEvent(UpdateCharacterSettings);
        upgrades.LoadUpgrades(GameManager.Instance.GetDataUsed().upgrades.GetAllUpgrades());
    }

    private void WeaponChanged(CurrentWeaponEquiped weapon)
    {
        if (equipedWeapon != null)
            equipedWeapon.Scale = Vector3.Zero;
        equipedWeapon = weapons[(int)weapon];
        equipedWeapon.Scale = Vector3.One;
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
        }
        else
        {
            EmitSignal(nameof(TakingDamage), GetHealth());
        }
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

    public void FinishAnimation(string name)
    {
        animationController.SetAnimationToFalse(name);
    }

    public void ReadyWeapon()
    {
        ability.ReadyHoldingWeapon();
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
}
