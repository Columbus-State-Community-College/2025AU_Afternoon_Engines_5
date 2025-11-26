using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : AudioController
{
    private GameObject _player;
    private HealthSystem _playerHealthSystem;
    private WaveSystem _waveSystem;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void Update()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                if (_playerHealthSystem && _playerHealthSystem.isDead &&
                    !audioSystem.SoundExists(audioLookup["mus_losescreen"]))
                {
                    StopAllMusic();
                    audioSystem.PlayGlobalAudio(audioLookup["mus_losescreen"], AudioType.Music);
                }

                if (_waveSystem && _waveSystem.allWavesCompleted && !audioSystem.SoundExists(audioLookup["mus_credits"]))
                {
                    StopAllMusic();
                    audioSystem.PlayGlobalAudio(audioLookup["mus_credits"], AudioType.Music);
                }

                if (!audioSystem.SoundExists(audioLookup["mus_credits"]) &&
                    !audioSystem.SoundExists(audioLookup["mus_losescreen"]) &&
                    !audioSystem.SoundExists(audioLookup["mus_gameplay"]))
                {
                    audioSystem.PlayGlobalAudio(audioLookup["mus_gameplay"], AudioType.Music);
                }

                break;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            case 0:
                _player = GameObject.FindWithTag("Player");
                _playerHealthSystem = _player.GetComponent<HealthSystem>();
                _waveSystem = _player.GetComponent<WaveSystem>();
                break;
            default:
                _player = null;
                _playerHealthSystem = null;
                _waveSystem = null;
                break;
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        StopAllMusic();
    }

    private void StopAllMusic()
    {
        foreach (var audioClip in audioLookup.Values)
        {
            if (!audioSystem.SoundExists(audioClip)) continue;
            
            audioSystem.StopSound(audioClip);
        }
    }
}
