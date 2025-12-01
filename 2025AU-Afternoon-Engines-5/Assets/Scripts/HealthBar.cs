using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private HealthSystem _playerHealthSystem;
    private Slider _healthBarSlider;

    private void Start()
    {
        _playerHealthSystem = GameObject.FindWithTag("Player").GetComponent<HealthSystem>();
        _healthBarSlider = GetComponent<Slider>();
        
        _healthBarSlider.maxValue = _playerHealthSystem.maxHealth;
        _healthBarSlider.value = _playerHealthSystem.currentHealth;
    }

    private void Update()
    {
        if (Mathf.Approximately(_playerHealthSystem.currentHealth, _healthBarSlider.value)) return;
        
        _healthBarSlider.value = _playerHealthSystem.currentHealth;
    }
}
