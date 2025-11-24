using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSystem))]
public class AudioController : MonoBehaviour
{
    public string[] audioTags;
    public AudioClip[] audioClips;

    protected Dictionary<string, AudioClip> audioLookup = new();
    protected AudioSystem audioSystem;

    void Start()
    {
        audioSystem = GetComponent<AudioSystem>();
        
        if (audioTags.Length != audioClips.Length)
        {
            Debug.LogWarning("The audioTags and audioClips arrays have different lengths. Please correct this.");
            return;
        }

        for (var i = 0; i < audioTags.Length; i++)
        {
            audioLookup.Add(audioTags[i], audioClips[i]);
        }
    }
}
