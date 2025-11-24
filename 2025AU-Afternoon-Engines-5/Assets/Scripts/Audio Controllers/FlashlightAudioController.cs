using UnityEngine;

public class FlashlightAudioController : AudioController
{
    private Flashlight _flashlight;
    
    protected override void Start()
    {
        base.Start();
        _flashlight = GetComponent<Flashlight>();
    }
    
    private void Update()
    {
        switch (_flashlight.damaging)
        {
            case true when !audioSystem.SoundExists(audioLookup["sfx_damaging"]):
                audioSystem.PlayGlobalAudio(audioLookup["sfx_damaging"], AudioType.SoundEffect, true);
                break;
            case false when audioSystem.SoundExists(audioLookup["sfx_damaging"]):
                audioSystem.StopSound(audioLookup["sfx_damaging"]);
                break;
        }
    }
}
