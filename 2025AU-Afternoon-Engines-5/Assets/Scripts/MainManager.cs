using System;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    
    // Game settings
    public float sensX = 2f;
    public float sensY = 2f;
    public int musicVolume = 50;
    public int sfxVolume = 50;
    public bool capFPS = false;
    public int fps = 60;
    
    // State information
    public bool paused = false;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }

    public void PauseGame()
    {
        if (paused) return;

        var pauseMenu = GameObject.Find("Pause Menu");
        
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        paused = true;
    }

    public void ResumeGame()
    {
        if (!paused) return;
        
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        paused = false;
    }

    public void SetCapFPS(bool capFps)
    {
        Instance.capFPS = capFps;

        if (Instance.capFPS)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = Instance.fps;
        }
        else
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = -1;
        }
    }

    public void SetFPSTarget(int fpsTarget)
    {
        Instance.fps = fpsTarget;

        if (!Instance.capFPS) return;
        
        Application.targetFrameRate = Instance.fps;
    }

    private void LoadSettings()
    {
        SetCapFPS(Convert.ToBoolean(PlayerPrefs.GetInt("CapFPS", 1)));
        SetFPSTarget(PlayerPrefs.GetInt("FPSTarget", 60));
        Instance.sensX = PlayerPrefs.GetFloat("sensX", 3f);
        Instance.sensY = PlayerPrefs.GetFloat("sensY", 3f);
        Instance.musicVolume = PlayerPrefs.GetInt("musicVolume", 50);
        Instance.sfxVolume = PlayerPrefs.GetInt("sfxVolume", 50);
    }
}
