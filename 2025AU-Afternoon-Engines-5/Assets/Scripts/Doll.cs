using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DollDestroyEvent : UnityEvent<GameObject> { }

public class Doll : MonoBehaviour
{
    public DollDestroyEvent onDestroy = new();

    private void OnDestroy()
    {
        onDestroy.Invoke(gameObject);
    }
}
