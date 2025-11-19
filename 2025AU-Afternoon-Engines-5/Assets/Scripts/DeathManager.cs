using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

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
        
        _deathCalled = true;
        
        if (transform.CompareTag("Player"))
        {
            PlayerDeath();
        }
        else
        {
            if (_agent)
            {
                _agent.isStopped = true;
            }
            StartCoroutine(EnemyDeathCoroutine());
        }
    }

    private IEnumerator EnemyDeathCoroutine()
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

    private void PlayerDeath()
    {
        var resultText = GameObject.Find("UI").transform.Find("ResultsScreen").transform.Find("ResultText").GetComponent<TextMeshProUGUI>();
        resultText.text = "You Lose!";
        Time.timeScale = 0f;
    }
}
