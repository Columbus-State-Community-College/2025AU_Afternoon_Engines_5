using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    public string[] audioTags;
    public AudioClip[] audioClips;

    private readonly Dictionary<string, AudioClip> _audioLookup = new();
    private List<AudioObject> _activeAudioObjects;
    private bool _globalPause = false;

    private void Start()
    {
        if (audioTags.Length != audioClips.Length)
        {
            Debug.LogWarning("The audioTags and audioClips arrays have different lengths. Please correct this.");
            return;
        }

        for (var i = 0; i < audioTags.Length; i++)
        {
            _audioLookup.Add(audioTags[i], audioClips[i]);
        }
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

        if (_globalPause) return;

        for (var i = _activeAudioObjects.Count - 1; i >= 0; i--)
        {
            var audioObject = _activeAudioObjects[i];

            if (audioObject.paused || audioObject.audioSource.isPlaying) continue;
            Destroy(audioObject.audioSource);
            _activeAudioObjects.RemoveAt(i);
        }
    }

    public void PlayGlobalAudio(string audioTag, AudioType type, bool loop = false)
    {
        var audioSource = gameObject.AddComponent<AudioSource>();
        AudioObject audioObject = new(audioSource)
        {
            audioSource =
            {
                clip = _audioLookup[audioTag],
                loop = loop,
                volume = (type == 0 ? MainManager.Instance.sfxVolume : MainManager.Instance.musicVolume) / 100f
            }
        };

        audioObject.audioSource.Play();
        _activeAudioObjects.Add(audioObject);
    }

    public void PlaySpatialAudio(string audioTag, AudioType type, float maxDistance, bool loop = false)
    {
        var audioSource = gameObject.AddComponent<AudioSource>();
        AudioObject audioObject = new(audioSource)
        {
            audioSource =
            {
                clip = _audioLookup[audioTag],
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

    public void PauseSound(string audioTag)
    {
        foreach (var audioObject in _activeAudioObjects)
        {
            if (audioObject.paused || audioObject.audioSource.clip != _audioLookup[audioTag]) continue;

            audioObject.audioSource.Pause();
            audioObject.paused = true;
            break;
        }
    }

    public void ResumeSound(string audioTag)
    {
        foreach (var audioObject in _activeAudioObjects)
        {
            if (!audioObject.paused || audioObject.audioSource.clip != _audioLookup[audioTag]) continue;

            audioObject.audioSource.UnPause();
            audioObject.paused = false;
            break;
        }
    }

    public void StopSound(string audioTag)
    {
        foreach (var audioObject in _activeAudioObjects)
        {
            if (audioObject.audioSource.clip != _audioLookup[audioTag]) continue;
            
            audioObject.audioSource.UnPause();
            audioObject.audioSource.Stop();
            break;
        }
    }

    public void FadeOutSound(string audioTag, float fadeTime)
    {
        foreach (var audioObject in _activeAudioObjects)
        {
            if (audioObject.audioSource.clip != _audioLookup[audioTag]) continue;
            
            StartCoroutine(FadeOutSoundCoroutine(audioObject.audioSource, fadeTime));
            break;
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
    public bool paused = false;

    public AudioObject(AudioSource audioSource, bool paused = false)
    {
        this.audioSource = audioSource;
        this.paused = paused;
    }
}

public enum AudioType : int
{
    SoundEffect = 0,
    Music = 1
}