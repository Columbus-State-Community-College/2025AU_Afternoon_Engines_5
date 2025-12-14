using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    private GameObject[] _creditsScreens = new GameObject[3];
    private int _screenIndex = 0;
    private Button _leftButton;
    private Button _rightButton;
    
    private void Start()
    {
        _creditsScreens[0] = GameObject.Find("/Canvas/Credits Screen/Credits 1");
        _creditsScreens[1] = GameObject.Find("/Canvas/Credits Screen/Credits 2");
        _creditsScreens[2] = GameObject.Find("/Canvas/Credits Screen/Credits 3");
        _leftButton = GameObject.Find("/Canvas/Credits Screen/Left Button").GetComponent<Button>();
        _rightButton = GameObject.Find("/Canvas/Credits Screen/Right Button").GetComponent<Button>();
        
        _creditsScreens[0].SetActive(true);
        _creditsScreens[1].SetActive(false);
        _creditsScreens[2].SetActive(false);
        
        _leftButton.interactable = false;
    }

    public void NextScreen()
    {
        if (_screenIndex == _creditsScreens.Length - 1) return;
        
        _creditsScreens[_screenIndex].SetActive(false);
        _screenIndex++;
        _creditsScreens[_screenIndex].SetActive(true);

        _leftButton.interactable = true;
        
        if (_screenIndex == _creditsScreens.Length - 1)
        {
            _rightButton.interactable = false;
        }
    }

    public void PreviousScreen()
    {
        if (_screenIndex == 0) return;
        
        _creditsScreens[_screenIndex].SetActive(false);
        _screenIndex--;
        _creditsScreens[_screenIndex].SetActive(true);
        
        _rightButton.interactable = true;

        if (_screenIndex == 0)
        {
            _leftButton.interactable = false;
        }
    }
}
