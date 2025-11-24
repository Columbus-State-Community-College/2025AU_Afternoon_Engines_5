using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Transform _orientation;

    private float _xRotation;
    private float _yRotation;

    private void Start()
    {
        _orientation = GameObject.Find("Player").transform.Find("Orientation").transform;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        var mouseX = Input.GetAxisRaw("Mouse X") * MainManager.Instance.sensX;
        var mouseY = Input.GetAxisRaw("Mouse Y") * MainManager.Instance.sensY;

        _yRotation += mouseX;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        
        transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
        _orientation.rotation = Quaternion.Euler(0f, _yRotation, _xRotation);
    }
}


