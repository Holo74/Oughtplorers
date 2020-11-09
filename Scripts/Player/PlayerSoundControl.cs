using Godot;
using System.Collections.Generic;

public class PlayerSoundControl : AudioStreamPlayer
{
    /*
    *Change so that it works on a single audio stream player and init a list of sounds from a 
    *list that is put in through the editor
    *Then you can play a sound by requesting it through a function by passing a string
    */
    [Signal]
    public delegate void SoundFinishedSignal(PlayerSoundControl player);

    [Export]
    private string[] names;

    private Dictionary<string, AudioStreamSample> sounds = new Dictionary<string, AudioStreamSample>();
    private bool repeat;
    private string current = "";

    public override void _Ready()
    {
        base._Ready();
        foreach (string n in names)
        {
            sounds.Add(n, ResourceLoader.Load<AudioStreamSample>("res://Sound/Player/" + n + ".wav"));
        }
        Connect(nameof(SoundFinishedSignal), GetParent(), nameof(SoundManager.AudioFinished));
    }

    public void PlaySound(string name, bool repeat)
    {
        if (sounds.ContainsKey(name))
        {
            if (current == name)
                return;
            Stop();
            Stream = sounds[name];
            Play();
            this.repeat = repeat;
            current = name;
        }
    }

    public bool Repeat()
    {
        return repeat;
    }

    public void StopCompletely()
    {
        current = "";
        repeat = false;
        Stop();
    }

    public void FinishedSound()
    {
        EmitSignal(nameof(SoundFinishedSignal), this);
    }

    public bool SoundPlayerContainsSound(string name)
    {
        return sounds.ContainsKey(name);
    }
}
