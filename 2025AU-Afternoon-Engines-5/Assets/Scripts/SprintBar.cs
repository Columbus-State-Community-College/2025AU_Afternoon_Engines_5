using UnityEngine;
using UnityEngine.UI;

public class SprintBar : MonoBehaviour
{
    private Slider _slider;
    private PlayerMovement _playerMovement;
    
    void Start()
    {
        _slider = GetComponent<Slider>();
        _playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();

        _slider.maxValue = 100f;
        _slider.value = _playerMovement.energy;
    }
    
    void Update()
    {
        if (Mathf.Approximately(_playerMovement.energy, _slider.value)) return;
        
        _slider.value = _playerMovement.energy;
    }
}
