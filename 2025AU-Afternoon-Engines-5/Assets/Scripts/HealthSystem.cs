using System.Collections;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public float invulnerabilityTime;
    public bool isInvulnerable = false;
    public bool isDead = false;
    
    private float _currentHealth;
    
    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float damage)
    {
        if (isInvulnerable || isDead) return;
        
        currentHealth -= damage;
        isInvulnerable = true;
        StartCoroutine(InvulnerabilityTimerCoroutine());
    }

    public void GiveHealth(float amount)
    {
        if (isDead) return;
        
        currentHealth += amount;
    }

    private IEnumerator InvulnerabilityTimerCoroutine()
    {
        float timer = 0;

        while (timer < invulnerabilityTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        isInvulnerable = false;
    }
}
