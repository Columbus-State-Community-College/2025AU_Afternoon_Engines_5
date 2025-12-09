using UnityEngine;

public class DollAudioController : AudioController
{
    private DollFinder _dollFinder;

    protected override void Start()
    {
        base.Start();
        _dollFinder = GameObject.FindWithTag("Player").GetComponent<DollFinder>();
        
        audioSystem.PlaySpatialAudio(audioLookup["sfx_doll"], AudioType.SoundEffect, _dollFinder.distortionDistance, true);
    }
}
