using Godot;
using System;

public class PlayerSoundControl : BaseAttatch
{
    private AudioStreamSample walking;
    private AudioStreamSample jumped;
    private AudioStreamSample running;
    private AudioStreamSample[] playerStateAudioSamples = new AudioStreamSample[11];
    public PlayerSoundControl(PlayerController controller) : base(controller, false)
    {
        controller.ability.AddToStateChange(UpdateSound);
        playerStateAudioSamples[(int)PlayerState.walking] = ResourceLoader.Load<AudioStreamSample>("res://Sound/Player/walking.wav");
        playerStateAudioSamples[(int)PlayerState.fallingUp] = ResourceLoader.Load<AudioStreamSample>("res://Sound/Player/Jumping.wav");
        playerStateAudioSamples[(int)PlayerState.sprinting] = ResourceLoader.Load<AudioStreamSample>("res://Sound/Player/Running.wav");
    }

    private void UpdateSound(PlayerState state)
    {
        controller.playerSound.Stream = playerStateAudioSamples[(int)state];
        controller.playerSound.Play();
    }

    public void PlaySoundFromFoot(PlayerState state)
    {
        controller.playerSound.Stream = playerStateAudioSamples[(int)state];
        controller.playerSound.Play();
    }
}
