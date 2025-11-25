using UnityEngine;

public class PlayerAudioController : AudioController
{
    private HealthSystem _healthSystem;
    private PlayerMovement _playerMovement;
    private PlayerInventory _inventory;

    private float _lastHealthValue;
    private bool _deathSoundPlayed = false;
    
    protected override void Start()
    {
        base.Start();
        _healthSystem = GetComponent<HealthSystem>();
        _playerMovement = GetComponent<PlayerMovement>();
        _inventory = GetComponent<PlayerInventory>();
        _lastHealthValue = _healthSystem.currentHealth;
        
        _inventory.pickupEvent.AddListener(OnItemPickup);
    }

    private void Update()
    {
        if (_healthSystem.isDead)
        {
            if (_deathSoundPlayed) return;
            
            audioSystem.PlayGlobalAudio(audioLookup["sfx_death"], AudioType.SoundEffect);
            _deathSoundPlayed = true;
        }
        
        if (!Mathf.Approximately(_lastHealthValue, _healthSystem.currentHealth))
        {
            audioSystem.PlayGlobalAudio(
                _lastHealthValue < _healthSystem.currentHealth ? audioLookup["sfx_heal"] : audioLookup["sfx_hurt"],
                AudioType.SoundEffect);

            _lastHealthValue = _healthSystem.currentHealth;
        }

        switch (_playerMovement.playerState)
        {
            case PlayerMovement.MovementState.Walking:
                if (audioSystem.SoundExists(audioLookup["sfx_walk"])) break;
                
                audioSystem.PlayGlobalAudio(audioLookup["sfx_walk"], AudioType.SoundEffect);
                break;
            case PlayerMovement.MovementState.Sprinting:
                if (audioSystem.SoundExists(audioLookup["sfx_run"])) break;
                
                if (audioSystem.SoundExists(audioLookup["sfx_walk"])) audioSystem.StopSound(audioLookup["sfx_walk"]);
                audioSystem.PlayGlobalAudio(audioLookup["sfx_run"], AudioType.SoundEffect);
                break;
            case PlayerMovement.MovementState.Airborne:
            case PlayerMovement.MovementState.Idle:
            default:
                if (audioSystem.SoundExists(audioLookup["sfx_walk"])) audioSystem.StopSound(audioLookup["sfx_walk"]);
                if (audioSystem.SoundExists(audioLookup["sfx_run"])) audioSystem.StopSound(audioLookup["sfx_run"]);
                break;
        }
    }

    private void OnItemPickup()
    {
        audioSystem.PlayGlobalAudio(audioLookup["sfx_pickup"], AudioType.SoundEffect);
    }
}
