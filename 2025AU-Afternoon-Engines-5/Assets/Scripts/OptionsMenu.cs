using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    private Toggle _fpsCapToggle;
    private Slider _fpsSlider;
    private Slider _xSensSlider;
    private Slider _ySensSlider;
    private Slider _musicVolumeSlider;
    private Slider _sfxVolumeSlider;

    private void Awake()
    {
        _fpsCapToggle = GameObject.Find("Options Menu/FPS Cap Toggle").GetComponent<Toggle>();
        _fpsSlider = GameObject.Find("Options Menu/FPS Slider").GetComponent<Slider>();
        _xSensSlider = GameObject.Find("Options Menu/X Sensitivity Slider").GetComponent<Slider>();
        _ySensSlider = GameObject.Find("Options Menu/Y Sensitivity Slider").GetComponent<Slider>();
        _musicVolumeSlider = GameObject.Find("Options Menu/Music Volume Slider").GetComponent<Slider>();
        _sfxVolumeSlider = GameObject.Find("Options Menu/SFX Volume Slider").GetComponent<Slider>();

        ReadSettings();
    }

    private void SetFPSCap()
    {
        _fpsCapToggle.isOn = MainManager.Instance.capFPS;
    }

    private void SetFPS()
    {
        _fpsSlider.value = MainManager.Instance.fps;
        _fpsSlider.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = MainManager.Instance.fps.ToString();
    }

    private void SetXSens()
    {
        _xSensSlider.value = MainManager.Instance.sensX;
        _xSensSlider.transform.Find("Value").GetComponent<TextMeshProUGUI>().text =
            MainManager.Instance.sensX.ToString(".0");
    }

    private void SetYSens()
    {
        _ySensSlider.value = MainManager.Instance.sensY;
        _ySensSlider.transform.Find("Value").GetComponent<TextMeshProUGUI>().text =
            MainManager.Instance.sensY.ToString(".0");
    }

    private void SetMusicVolume()
    {
        _musicVolumeSlider.value = MainManager.Instance.musicVolume;
        _musicVolumeSlider.transform.Find("Value").GetComponent<TextMeshProUGUI>().text =
            MainManager.Instance.musicVolume.ToString();
    }

    private void SetSfxVolume()
    {
        _sfxVolumeSlider.value = MainManager.Instance.sfxVolume;
        _sfxVolumeSlider.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = MainManager.Instance.sfxVolume.ToString();
    }

    public void UpdateFPSCap()
    {
        MainManager.Instance.SetCapFPS(_fpsCapToggle.isOn);
        SetFPSCap();
    }

    public void UpdateFPS()
    {
        MainManager.Instance.SetFPSTarget((int)_fpsSlider.value);
        SetFPS();
    }

    public void UpdateXSens()
    {
        MainManager.Instance.sensX = _xSensSlider.value;
        SetXSens();
    }

    public void UpdateYSens()
    {
        MainManager.Instance.sensY = _ySensSlider.value;
        SetYSens();
    }

    public void UpdateMusicVolume()
    {
        MainManager.Instance.musicVolume = (int)_musicVolumeSlider.value;
        SetMusicVolume();
    }

    public void UpdateSfxVolume()
    {
        MainManager.Instance.sfxVolume = (int)_sfxVolumeSlider.value;
        SetSfxVolume();
    }

    private void ReadSettings()
    {
        SetFPSCap();
        SetFPS();
        SetXSens();
        SetYSens();
        SetMusicVolume();
        SetSfxVolume();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("CapFPS", MainManager.Instance.capFPS ? 1 : 0);
        PlayerPrefs.SetInt("FPSTarget", MainManager.Instance.fps);
        PlayerPrefs.SetFloat("sensX", MainManager.Instance.sensX);
        PlayerPrefs.SetFloat("sensY", MainManager.Instance.sensY);
        PlayerPrefs.SetInt("musicVolume", MainManager.Instance.musicVolume);
        PlayerPrefs.SetInt("sfxVolume", MainManager.Instance.sfxVolume);
    }
}
