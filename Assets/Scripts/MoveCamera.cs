using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private Transform _cameraPosition;

    private void Start()
    {
        _cameraPosition = GameObject.Find("Player").transform.Find("CameraPosition").transform;
    }

    private void Update()
    {
        transform.position = _cameraPosition.position;
    }
}
