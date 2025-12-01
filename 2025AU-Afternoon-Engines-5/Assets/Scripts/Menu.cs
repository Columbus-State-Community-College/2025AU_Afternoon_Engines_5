using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Resume()
    {
        MainManager.Instance.ResumeGame();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        MainManager.Instance.ResumeGame();
        SceneManager.LoadScene(0);
    }
}
