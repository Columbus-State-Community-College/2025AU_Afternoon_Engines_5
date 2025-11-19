using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class DeathManager : MonoBehaviour
{
    public float deathDuration = 1f;
    
    private HealthSystem _healthSystem;
    private NavMeshAgent _agent;
    private bool _deathCalled = false;
    
    private void Start()
    {
        _healthSystem = GetComponent<HealthSystem>();
        _agent = GetComponent<NavMeshAgent>();
        
        if (!_healthSystem) Destroy(this);
    }

    private void Update()
    {
        if (!_healthSystem.isDead || _deathCalled) return;

        _agent.isStopped = true;
        _deathCalled = true;
        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        var timer = 0f;
        var initialScale = transform.localScale;
        var targetScale = Vector3.zero;

        while (timer < deathDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, timer / deathDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        
        transform.localScale = targetScale;
        Destroy(gameObject);
    }
}
