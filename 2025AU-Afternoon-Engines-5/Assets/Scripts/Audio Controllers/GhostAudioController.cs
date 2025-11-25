using System;
using Unity.VisualScripting;
using UnityEngine;

public class GhostAudioController : AudioController
{
    public float deathSfxDistance = 3f;
    public float hurtSfxDistance = 3f;
    public float wanderSfxDistance = 10f;
    public int randomMax = 5000;
    
    private HealthSystem _healthSystem;
    private EnemyAgentBase _enemyAgentBase;

    private bool _playedDeathSound = false;
    private bool _fadingWanderingSound = false;
    private float _lastHealthValue;
    
    protected override void Start()
    {
        base.Start();
        // This script uses the random function to play the wandering sound. To make sure that all the ghosts don't have
        // the same random seed (due to initializing at the same time), we use the ghost's transform.
        UnityEngine.Random.InitState(DateTime.Now.Millisecond * (int)transform.localPosition.x);
        _healthSystem = GetComponent<HealthSystem>();
        _enemyAgentBase = GetComponent<EnemyAgentBase>();
        _lastHealthValue = _healthSystem.currentHealth;
    }
    
    private void Update()
    {
        switch (_healthSystem.isDead)
        {
            case true when !_playedDeathSound:
                _playedDeathSound = true;
                audioSystem.PlaySpatialAudio(audioLookup["sfx_death"], AudioType.SoundEffect, deathSfxDistance);
                RemoveOtherSounds();
                return;
            case true when _playedDeathSound:
                return;
        }

        switch (_enemyAgentBase.agent.speed > 0)
        {
            case true when !audioSystem.SoundExists(audioLookup["sfx_wandering"]):
                // The sound effects should be random, otherwise all ghosts will play this sound at the same time.
                // The odds need to be super low because this is checked every frame.
                if (UnityEngine.Random.Range(0, randomMax) != 0) break;
                audioSystem.PlaySpatialAudio(audioLookup["sfx_wandering"], AudioType.SoundEffect, wanderSfxDistance);
                _fadingWanderingSound = false;
                break;
            case false when audioSystem.SoundExists(audioLookup["sfx_wandering"]) && !_fadingWanderingSound:
                audioSystem.FadeOutSound(audioLookup["sfx_wandering"], 1f);
                _fadingWanderingSound = true;
                break;
        }

        switch (Mathf.Approximately(_lastHealthValue, _healthSystem.currentHealth))
        {
            case true when audioSystem.SoundExists(audioLookup["sfx_hurt"]) && audioSystem.LoopEnabled(audioLookup["sfx_hurt"]):
                audioSystem.DisableLoop(audioLookup["sfx_hurt"]);
                break;
            case false:
                _lastHealthValue = _healthSystem.currentHealth;

                if (audioSystem.SoundExists(audioLookup["sfx_hurt"])) break;
                
                audioSystem.PlaySpatialAudio(audioLookup["sfx_hurt"], AudioType.SoundEffect, hurtSfxDistance, true);
                break;
        }
    }

    private void RemoveOtherSounds()
    {
        if (audioSystem.SoundExists(audioLookup["sfx_wandering"]))
        {
            audioSystem.FadeOutSound(audioLookup["sfx_wandering"], 1f);
        }

        if (audioSystem.SoundExists(audioLookup["sfx_hurt"]))
        {
            audioSystem.DisableLoop(audioLookup["sfx_hurt"]);
        }
    }
}
