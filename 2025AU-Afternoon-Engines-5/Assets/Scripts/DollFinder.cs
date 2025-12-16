using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class DollFinder : MonoBehaviour
{
    public float distortionDistance = 20f;
    public float restorationTime = 1f;
    public float postExposureTarget = -1f;
    public float chromaticAberrationTarget = 1f;
    public bool useTint = false;
    public Color tintColor = new(0.41f, 0f, 0f, 0f);
    public float tintTarget = 0.25f;
    
    private ItemSpawner _itemSpawner;
    private List<GameObject> _dolls = new();
    
    private Volume _globalVolume;
    private VolumeProfile _volumeProfile;
    private ColorAdjustments _colorAdjustments;
    private ChromaticAberration _chromaticAberration;
    private float _origPostExposure;
    private float _origChromaticAberration;
    private float _currentPostExposure;
    private float _currentChromaticAberration;
    private float _origTint;
    private float _currentTint;

    private Image _tintImage;
    

    private void Start()
    {
        _itemSpawner = GameObject.Find("/Spawners/Doll Spawner").GetComponent<ItemSpawner>();
        _globalVolume = GameObject.Find("/Post Processing/Global Volume").GetComponent<Volume>();
        _tintImage = GameObject.Find("/UI/Tint").GetComponent<Image>();
        _volumeProfile = _globalVolume.profile;
        _volumeProfile.TryGet(out _colorAdjustments);
        _volumeProfile.TryGet(out _chromaticAberration);
        
        _itemSpawner.onSpawn.AddListener(OnItemSpawn);
        _origPostExposure = _colorAdjustments.postExposure.value;
        _origChromaticAberration = _chromaticAberration.intensity.value;
    }

    private void Update()
    {
        var closestDollDistance = FindClosestDollDistance();

        if (closestDollDistance > distortionDistance) return;
        
        DistortVision(closestDollDistance);
    }

    private void OnItemSpawn(GameObject item)
    {
        if (!item.name.Contains("Doll")) return;

        item.GetComponent<Doll>().onDestroy.AddListener(OnDollDestroy);
        _dolls.Add(item);
    }

    private void OnDollDestroy(GameObject doll)
    {
        _dolls.Remove(doll);
        StartCoroutine(RestoreVisionCoroutine());
    }

    private float FindClosestDollDistance()
    {
        var closestDistance = Mathf.Infinity;

        foreach (var doll in _dolls)
        {
            var distance = Vector3.Distance(transform.position, doll.transform.position);

            if (closestDistance <= distance) continue;

            closestDistance = distance;
        }
        
        return closestDistance;
    }

    private void DistortVision(float distance)
    {
        _currentPostExposure = Mathf.Lerp(postExposureTarget, _origPostExposure, distance / distortionDistance);
        _currentChromaticAberration = Mathf.Lerp(chromaticAberrationTarget, _origChromaticAberration, distance / distortionDistance);
        
        _colorAdjustments.postExposure.value = _currentPostExposure;
        _chromaticAberration.intensity.value = _currentChromaticAberration;

        if (!useTint) return;
        
        _currentTint = Mathf.Lerp(tintTarget, _origTint, distance / distortionDistance);
        tintColor.a = _currentTint;
        _tintImage.color = tintColor;
    }

    private IEnumerator RestoreVisionCoroutine()
    {
        var timer = 0f;

        while (timer < restorationTime)
        {
            timer += Time.deltaTime;
            _colorAdjustments.postExposure.value = Mathf.Lerp(_currentPostExposure, _origPostExposure, timer / restorationTime);
            _chromaticAberration.intensity.value = Mathf.Lerp(_currentPostExposure, _origPostExposure, timer / restorationTime);

            if (useTint)
            {
                tintColor.a = Mathf.Lerp(_currentTint, _origTint, timer / restorationTime);
                _tintImage.color = tintColor;
            }
            
            yield return null;
        }
        
        _colorAdjustments.postExposure.value = _origPostExposure;
        _chromaticAberration.intensity.value = _origChromaticAberration;
        _currentPostExposure = _origPostExposure;
        _currentChromaticAberration = _origChromaticAberration;

        if (!useTint) yield break;
        
        tintColor.a = _origTint;
        _tintImage.color = tintColor;
    }
}
