using UnityEngine;

[RequireComponent(typeof(PhaserGhost))]
public class PhaserAudioController : AudioController
{
    public float phaseSfxDistance = 5f;
    
    private PhaserGhost _phaser;
    
    protected override void Start()
    {
        base.Start();
        _phaser = GetComponent<PhaserGhost>();

        _phaser.phaseOutEvent.AddListener(OnPhaseOut);
        _phaser.phaseInEvent.AddListener(OnPhaseIn);
        
    }

    private void OnPhaseOut()
    {
        audioSystem.PlaySpatialAudio(audioLookup["sfx_phase_out"], AudioType.SoundEffect, phaseSfxDistance);
        audioSystem.ModifyRolloff(audioLookup["sfx_phase_out"], AudioRolloffMode.Linear);
    }

    private void OnPhaseIn()
    {
        audioSystem.PlaySpatialAudio(audioLookup["sfx_phase_in"], AudioType.SoundEffect, phaseSfxDistance);
        audioSystem.ModifyRolloff(audioLookup["sfx_phase_out"], AudioRolloffMode.Linear);
    }
}
