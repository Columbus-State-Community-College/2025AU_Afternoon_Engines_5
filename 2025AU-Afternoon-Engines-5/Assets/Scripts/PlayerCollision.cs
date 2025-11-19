using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private HealthSystem _healthSystem;

    private void Start()
    {
        _healthSystem = GetComponent<HealthSystem>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        HandleCollision(collision);
    }

    private void HandleCollision(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Enemy")) return;
        
        _healthSystem.TakeDamage(5f);
    }
}
