using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    
    // Game settings
    public float sensX = 2f;
    public float sensY = 2f;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
