using System;
using UnityEngine;

public class GhostAudioController : AudioController
{
    public float deathSfxDistance = 3f;
    public float wanderSfxDistance = 10f;
    
    private HealthSystem _healthSystem;
    private EnemyAgentBase _enemyAgentBase;

    private bool _playedDeathSound = false;
    private bool _fadingWanderingSound = false;
    
    protected override void Start()
    {
        base.Start();
        _healthSystem = GetComponent<HealthSystem>();
        _enemyAgentBase = GetComponent<EnemyAgentBase>();
    }
    
    private void Update()
    {
        switch (_healthSystem.isDead)
        {
            case true when !_playedDeathSound:
                _playedDeathSound = true;
                audioSystem.PlaySpatialAudio(audioLookup["sfx_death"], AudioType.SoundEffect, deathSfxDistance);
                break;
        }

        switch (_enemyAgentBase.agent.speed > 0)
        {
            case true when !audioSystem.SoundExists(audioLookup["sfx_wandering"]):
                if (UnityEngine.Random.Range(0, 3) != 0) break;   // The sound effects should be random, otherwise all ghosts will play this sound at the same time.
                audioSystem.PlaySpatialAudio(audioLookup["sfx_wandering"], AudioType.SoundEffect, wanderSfxDistance);
                _fadingWanderingSound = false;
                break;
            case false when audioSystem.SoundExists(audioLookup["sfx_wandering"]) && !_fadingWanderingSound:
                audioSystem.FadeOutSound(audioLookup["sfx_wandering"], 1f);
                _fadingWanderingSound = true;
                break;
        }
    }
}
