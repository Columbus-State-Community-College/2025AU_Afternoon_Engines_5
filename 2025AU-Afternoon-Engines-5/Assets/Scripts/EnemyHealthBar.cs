using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HealthSystem))]
public class EnemyHealthBar : MonoBehaviour
{
    public Canvas healthBarCanvas;
    public float visibleDistance = 5f;
    public float yValue = 1f;

    private Canvas _healthBarCanvasObject;
    private GameObject _healthBar;
    private Slider _healthBarSlider;
    private HealthSystem _healthSystem;
    private GameObject _player;
    private Transform _camera;
    
    private void Start()
    {
        _healthSystem = GetComponent<HealthSystem>();
        _player = GameObject.FindWithTag("Player");

        if (Camera.main != null) _camera = Camera.main.transform;

        _healthBarCanvasObject = Instantiate(healthBarCanvas, transform);
        _healthBar = _healthBarCanvasObject.transform.GetChild(0).gameObject;
        _healthBarSlider = _healthBar.GetComponent<Slider>();

        var healthBarPosition = transform.position;
        healthBarPosition.y += yValue;
        _healthBarCanvasObject.transform.position = healthBarPosition;
        
        _healthBarSlider.maxValue = _healthSystem.maxHealth;
        _healthBarSlider.value = _healthSystem.currentHealth;
    }
    
    private void LateUpdate()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) > visibleDistance)
        {
            if (_healthBar.activeSelf) _healthBar.SetActive(false);
            return;
        }
        
        if (!_healthBar.activeSelf) _healthBar.SetActive(true);
        
        _healthBarCanvasObject.transform.LookAt(_healthBarCanvasObject.transform.position + _camera.forward);

        if (Mathf.Approximately(_healthSystem.currentHealth, _healthBarSlider.value)) return;
        
        _healthBarSlider.value = _healthSystem.currentHealth;
    }
}
