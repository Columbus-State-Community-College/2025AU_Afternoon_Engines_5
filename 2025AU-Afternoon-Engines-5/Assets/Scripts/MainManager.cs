using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    
    // Game settings
    public float sensX = 2f;
    public float sensY = 2f;
    public int musicVolume = 50;
    public int sfxVolume = 50;
    
    // State information
    public bool paused = false;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
