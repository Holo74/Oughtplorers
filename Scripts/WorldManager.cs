using Godot;
using System.Collections.Generic;
using System.Threading;

public class WorldManager : Node
{
    public static WorldManager instance { get; private set; }
    public ObjectPool<PlayerProjectiles> shots;
    public ObjectPool<EnemyProjectiles> enemyProjectilePool;
    private ResourceInteractiveLoader loader;
    private ProgressBar loadingBar;
    private Queue<string> loadingPaths = new Queue<string>();
    private float waitFrame = 1;
    private float loaded = 0;

    private Vector3 loadLocation, loadRotation;
    [Signal]
    public delegate void AreaLoaded(bool loaded);
    private AreaLoaded loadingDone;
    private Node previousRoom, currentRoom, nextRoom;
    private string previousRoomFile, currentRoomFile, nextRoomFile;
    private Godot.Collections.Dictionary additionalWorldInfo;
    private Navigation navigation;
    public delegate void SwitchCurrentRoom(Node room);
    private event SwitchCurrentRoom swapRoom;
    private System.Threading.Thread loadingInfoThread;
    private object lockObject = new object();
    Queue<Node> loadedNodes = new Queue<Node>();
    private bool startThread = false, objectInLoading = false;

    public void RegisterSwitchingRoomEvent(SwitchCurrentRoom function)
    {
        swapRoom += function;
    }

    public void DeregisterSwitchingRoomEvent(SwitchCurrentRoom function)
    {
        swapRoom -= function;
    }

    public void ThreadsafeAdd(in Queue<Node> list, string path)
    {
        lock (loadedNodes)
        {
            PackedScene r = ResourceLoader.Load<PackedScene>(path);
            list.Enqueue(r.Instance());
            r.Dispose();
        }
    }

    public override void _Ready()
    {
        instance = this;
        additionalWorldInfo = GameManager.Instance.GetDataUsed().SavedInPath;
        loadingBar = GetChild(0).GetChild<ProgressBar>(1);
        navigation = GetChild<Navigation>(1);
        loadingPaths.Enqueue("res://Scenes/Player.tscn");
        loadingPaths.Enqueue("res://Scenes/Menus/InGameMenu.tscn");
        loadingPaths.Enqueue(GameManager.Instance.startingAreaPath);
        currentRoomFile = GameManager.Instance.startingAreaPath;
        waitFrame = 4;
        //loader = ResourceLoader.LoadInteractive();
        loadingInfoThread = new System.Threading.Thread(new ThreadStart(() => ThreadsafeAdd(loadedNodes, loadingPaths.Dequeue())));
        shots = new ObjectPool<PlayerProjectiles>("TempProjectile.tscn");
        enemyProjectilePool = new ObjectPool<EnemyProjectiles>("EnemyTempProj.tscn");
        loaded = 0;
        GameManager.Instance.Connect(nameof(GameManager.ReturnToTitle), this, nameof(StoppingWorld));
        startThread = true;
        loadingInfoThread.Start();
    }
    public override void _Process(float delta)
    {
        if (waitFrame > 0)
        {
            waitFrame -= 1;
            if (loadingBar.Value == 100 && waitFrame <= 0)
            {
                Spatial spawn = new Spatial();
                if (GetTree().HasGroup("Spawn"))
                {
                    spawn = (Spatial)GetTree().GetNodesInGroup("Spawn")[0];
                }
                else
                {
                    GD.Print("No spawn detected");
                    GetTree().Quit();
                }
                PlayerController.Instance.ReadyPlayer(spawn.GlobalTransform);
                PlayerController.Instance.UpdateCharacterSettings();
                RemoveChild(GetChild(0));
                InGameMenu.Instance.ReadyMenu();
                GameManager.Instance.SetToPlay();
            }
            return;
        }
        if (!loadingInfoThread.IsAlive && !PlayerController.CharacterPlaying() && startThread)
        {
            lock (loadedNodes)
            {
                startThread = false;
                if (loadedNodes.Count > 0)
                {
                    Node node = loadedNodes.Dequeue();
                    navigation.AddChild(node);
                    if (loadingPaths.Count == 0)
                    {
                        currentRoom = node;
                        loadingBar.Value = 100;
                        waitFrame = 30f;
                    }
                    else
                    {

                        loadingBar.Value = (4 - loadingPaths.Count) / 4;
                        loadingInfoThread = new System.Threading.Thread(new ThreadStart(() => ThreadsafeAdd(loadedNodes, loadingPaths.Dequeue())));
                        startThread = true;
                    }
                }

            }
            if (startThread)
            {
                loadingInfoThread.Start();
            }
            // Error error = loader.Poll();
            // switch (error)
            // {
            //     case Error.FileEof:
            //         PackedScene holder = loader.GetResource().Duplicate() as PackedScene;
            //         loader.Dispose();
            //         Node node = holder.Instance();
            //         loader = null;
            //         navigation.AddChild(node);
            //         if (loadingPaths.Count == 0)
            //         {
            //             currentRoom = node;
            //             loadingBar.Value = 100;
            //             waitFrame = 30f;
            //         }
            //         else
            //         {
            //             loader = ResourceLoader.LoadInteractive(loadingPaths.Dequeue());
            //         }
            //         break;
            //     case Error.Ok:
            //         float temp = (loader.GetStage() / (float)loader.GetStageCount()) * 100 / (float)(loadingPaths.Count + 1);
            //         if (loaded < temp)
            //             loaded = temp;
            //         loadingBar.Value = loaded;
            //         break;
            //     default:
            //         loader = null;
            //         GD.Print("Error");
            //         break;
            // }
        }
        else
        {
            if (!objectInLoading || loadingInfoThread.IsAlive) return;
            lock (loadedNodes)
            {
                if (loadedNodes.Count > 0)
                {
                    Spatial node = (Spatial)loadedNodes.Dequeue();
                    node.Translate(loadLocation);
                    node.Rotation = loadRotation;
                    navigation.AddChild(node);
                    EmitSignal(nameof(AreaLoaded));
                    loadingDone?.Invoke(true);
                    loadingDone = null;
                    nextRoom = node;
                    objectInLoading = false;
                }
            }
            // if (!startThread) return;
            // Error error = loader.Poll();
            // switch (error)
            // {
            //     case Error.FileEof:
            //         PackedScene holder = loader.GetResource().Duplicate() as PackedScene;
            //         loader.Dispose();
            //         Spatial node = (Spatial)holder.Instance();
            //         loader = null;
            //         node.Translate(loadLocation);
            //         node.Rotation = loadRotation;
            //         navigation.AddChild(node);
            //         EmitSignal(nameof(AreaLoaded));
            //         loadingDone?.Invoke(true);
            //         loadingDone = null;
            //         nextRoom = node;
            //         loader = null;
            //         break;
            //     case Error.Ok:
            //         break;
            //     default:
            //         break;
            // }
        }
    }

    public void SetNewCurrentRoom(Node setting)
    {
        swapRoom?.Invoke(setting);
        if (setting.Name == currentRoom.Name)
            return;
        if (previousRoom != null)
        {
            if (setting.Name == previousRoom.Name)
            {
                string path = previousRoomFile;
                previousRoom = currentRoom;
                currentRoom = setting;
                previousRoomFile = currentRoomFile;
                currentRoomFile = path;
                return;
            }
        }
        if (nextRoom.Name == setting.Name)
        {
            if (previousRoom != null)
                previousRoom.QueueFree();
            previousRoom = currentRoom;
            previousRoomFile = currentRoomFile;
            currentRoom = setting;
            currentRoomFile = nextRoomFile;
            nextRoom = null;
        }
    }

    public void LoadArea(string path, Vector3 loc, Vector3 rot)
    {
        if (currentRoomFile == path || previousRoomFile == path || nextRoomFile == path)
            return;
        if (nextRoom != null)
            nextRoom.QueueFree();
        loadingInfoThread = new System.Threading.Thread(new ThreadStart(() => ThreadsafeAdd(loadedNodes, path)));
        loadLocation = loc;
        loadRotation = rot;
        nextRoomFile = path;
        loadingInfoThread.Start();
        objectInLoading = true;
    }

    public void LoadArea(string path, Vector3 loc, Vector3 rot, AreaLoaded load)
    {
        loadingDone += load;
        if (currentRoomFile == path || previousRoomFile == path || nextRoomFile == path)
        {
            load(false);
            loadingDone = null;
            return;
        }
        LoadArea(path, loc, rot);
    }

    public void StoppingWorld()
    {
        instance = null;
        PlayerController.Instance.DeloadPlayer();
        RayCastData.ClearDic();
        PlayerAreaSensor.ResetSensors();
    }

    public string GetCurrentRoomFile()
    {
        return currentRoomFile;
    }

    public void AddToWorldInfo(string name, object stored)
    {
        if (additionalWorldInfo.Contains(name))
            additionalWorldInfo[name] = stored;
        else
            additionalWorldInfo.Add(name, stored);
    }

    public T GetWorldInfoData<T>(string name)
    {
        return (T)additionalWorldInfo[name];
    }

    public bool WorldInfoHas(string name)
    {
        return additionalWorldInfo.Contains(name);
    }

    public Godot.Collections.Dictionary GetWorldInfo()
    {
        return additionalWorldInfo;
    }

    public Navigation GetNavigation()
    {
        return navigation;
    }
}