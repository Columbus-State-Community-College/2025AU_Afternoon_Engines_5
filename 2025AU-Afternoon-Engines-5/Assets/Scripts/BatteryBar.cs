using UnityEngine;
using UnityEngine.UI;

public class BatteryBar : MonoBehaviour
{
    private Flashlight _flashlight;
    private Slider _batterySlider;

    private void Start()
    {
        _flashlight = GameObject.FindWithTag("Player").transform.Find("Flashlight").GetComponent<Flashlight>();
        _batterySlider = GetComponent<Slider>();
        
        _batterySlider.maxValue = _flashlight.maxBattery;
        _batterySlider.value = _flashlight.battery;
    }

    private void Update()
    {
        if (Mathf.Approximately(_flashlight.battery, _batterySlider.value)) return;
        
        _batterySlider.value = _flashlight.battery;
    }
}
