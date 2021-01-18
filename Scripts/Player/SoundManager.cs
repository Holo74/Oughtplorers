using Godot;
using System;

public class SoundManager : Node
{
    //Handles all sound that the player has.  It will not try to determine when to play it but requests must be made
    private PlayerSoundControl feet, speaking, gun;
    public static SoundManager instance;
    public override void _Ready()
    {
        feet = GetChild<PlayerSoundControl>(0);
        speaking = GetChild<PlayerSoundControl>(1);
        gun = GetChild<PlayerSoundControl>(2);
        instance = this;
    }

    public void AudioFinished(PlayerSoundControl sound)
    {
        if (sound.Repeat())
        {
            sound.Play();
        }
    }

    public void PlayerRequestedSound(string soundName, bool repeat = false)
    {
        feet.PlaySound(soundName, repeat);
        speaking.PlaySound(soundName, repeat);
        gun.PlaySound(soundName, repeat);
    }

    public void StopOneSound(PlayerSoundFrom from)
    {
        switch (from)
        {
            case PlayerSoundFrom.feet:
                feet.StopCompletely();
                break;
            case PlayerSoundFrom.gun:
                feet.StopCompletely();
                break;
            case PlayerSoundFrom.head:
                feet.StopCompletely();
                break;
        }
    }

    public void StopAllSound()
    {
        feet.Stop();
        speaking.Stop();
        gun.Stop();
    }
}

public enum PlayerSoundFrom
{
    feet,
    head,
    gun
}
