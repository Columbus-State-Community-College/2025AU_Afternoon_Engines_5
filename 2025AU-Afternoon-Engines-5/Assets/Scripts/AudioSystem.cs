using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    private List<AudioObject> _activeAudioObjects = new();
    private bool _globalPause = false;
    private int _lastMusicVolume;
    private int _lastSfxVolume;

    private void Start()
    {
        _lastMusicVolume = MainManager.Instance.musicVolume;
        _lastSfxVolume = MainManager.Instance.sfxVolume;
    }

    private void Update()
    {
        switch (MainManager.Instance.paused)
        {
            case true when !_globalPause:
            {
                _globalPause = true;
                foreach (var audioObject in _activeAudioObjects)
                {
                    if (audioObject.audioType == AudioType.Music) continue;
                    
                    audioObject.audioSource.Pause();
                }

                break;
            }
            case false when _globalPause:
            {
                _globalPause = false;
                foreach (var audioObject in _activeAudioObjects)
                {
                    if (audioObject.paused) continue;

                    audioObject.audioSource.UnPause();
                }

                break;
            }
        }

        if (_lastMusicVolume != MainManager.Instance.musicVolume || _lastSfxVolume != MainManager.Instance.sfxVolume)
        {
            _lastMusicVolume = MainManager.Instance.musicVolume;
            _lastSfxVolume = MainManager.Instance.sfxVolume;
            UpdateVolume();
        }

        if (_globalPause) return;

        for (var i = _activeAudioObjects.Count - 1; i >= 0; i--)
        {
            var audioObject = _activeAudioObjects[i];

            if (audioObject.paused || audioObject.audioSource.isPlaying) continue;
            Destroy(audioObject.audioSource);
            _activeAudioObjects.RemoveAt(i);
        }
    }

    public void PlayGlobalAudio(AudioClip audioClip, AudioType type, bool loop = false)
    {
        var audioSource = gameObject.AddComponent<AudioSource>();
        AudioObject audioObject = new(audioSource, type)
        {
            audioSource =
            {
                clip = audioClip,
                loop = loop,
                volume = (type == 0 ? MainManager.Instance.sfxVolume : MainManager.Instance.musicVolume) / 100f
            }
        };

        audioObject.audioSource.Play();
        _activeAudioObjects.Add(audioObject);
    }

    public void PlaySpatialAudio(AudioClip audioClip, AudioType type, float maxDistance, bool loop = false)
    {
        var audioSource = gameObject.AddComponent<AudioSource>();
        AudioObject audioObject = new(audioSource, type)
        {
            audioSource =
            {
                clip = audioClip,
                spatialize = true,
                spatialBlend = 1f,
                maxDistance = maxDistance,
                loop = loop,
                volume = (type == 0 ? MainManager.Instance.sfxVolume : MainManager.Instance.musicVolume) / 100f
            }
        };

        audioObject.audioSource.Play();
        _activeAudioObjects.Add(audioObject);
    }

    public void PauseSound(AudioClip audioClip)
    {
        foreach (var audioObject in _activeAudioObjects)
        {
            if (audioObject.paused || audioObject.audioSource.clip != audioClip) continue;

            audioObject.audioSource.Pause();
            audioObject.paused = true;
            break;
        }
    }

    public void ResumeSound(AudioClip audioClip)
    {
        foreach (var audioObject in _activeAudioObjects)
        {
            if (!audioObject.paused || audioObject.audioSource.clip != audioClip) continue;

            audioObject.audioSource.UnPause();
            audioObject.paused = false;
            break;
        }
    }

    public void StopSound(AudioClip audioClip)
    {
        foreach (var audioObject in _activeAudioObjects)
        {
            if (audioObject.audioSource.clip != audioClip) continue;

            audioObject.paused = false;
            audioObject.audioSource.Stop();
            break;
        }
    }

    public void FadeOutSound(AudioClip audioClip, float fadeTime)
    {
        foreach (var audioObject in _activeAudioObjects)
        {
            if (audioObject.audioSource.clip != audioClip) continue;

            if (audioObject.fading) break;
            
            StartCoroutine(FadeOutSoundCoroutine(audioObject.audioSource, fadeTime));
            audioObject.paused = false;
            audioObject.fading = true;
            break;
        }
    }

    public bool SoundExists(AudioClip audioClip)
    {
        foreach (var audioObject in _activeAudioObjects)
        {
            if (audioObject.audioSource.clip != audioClip) continue;

            return true;
        }

        return false;
    }

    public void DisableLoop(AudioClip audioClip)
    {
        foreach (var audioObject in _activeAudioObjects)
        {
            if (audioObject.audioSource.clip != audioClip) continue;
            
            audioObject.audioSource.loop = false;
        }
    }

    public bool LoopEnabled(AudioClip audioClip)
    {
        foreach (var audioObject in _activeAudioObjects)
        {
            if (audioObject.audioSource.clip != audioClip) continue;
            
            return audioObject.audioSource.loop;
        }

        return false;
    }

    private void UpdateVolume()
    {
        var sfxVolume = MainManager.Instance.sfxVolume / 100f;
        var musicVolume = MainManager.Instance.musicVolume / 100f;
        
        foreach (var audioObject in _activeAudioObjects)
        {
            switch (audioObject.audioType)
            {
                case AudioType.SoundEffect:
                    if (Mathf.Approximately(audioObject.audioSource.volume, sfxVolume)) continue;
                    
                    audioObject.audioSource.volume = sfxVolume;
                    break;
                case AudioType.Music:
                    if (Mathf.Approximately(audioObject.audioSource.volume, musicVolume)) continue;
                    
                    audioObject.audioSource.volume = musicVolume;
                    break;
                default:
                    break;
            }
        }
    }
    
    private IEnumerator FadeOutSoundCoroutine(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;
        float timeElapsed = 0;

        while (audioSource.volume > 0f)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timeElapsed / fadeTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.Stop();
    }
}

public class AudioObject
{
    public AudioSource audioSource;
    public AudioType audioType;
    public bool paused = false;
    public bool fading = false;

    public AudioObject(AudioSource audioSource, AudioType audioType, bool paused = false)
    {
        this.audioSource = audioSource;
        this.audioType = audioType;
        this.paused = paused;
    }
}

public enum AudioType : int
{
    SoundEffect = 0,
    Music = 1
}