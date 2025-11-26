using UnityEngine;

public class EnemyDamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageAmount = 5f;
    public float damageRadius = 1.5f;
    public float damageCooldown = 1f;
    public string playerTag = "Player";

    private Transform _player;
    private float _timer;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag(playerTag);
        if (p != null)
            _player = p.transform;
    }

    void Update()
    {
        if (_player == null) return;

        _timer += Time.deltaTime;

        // Only damage when cooldown expires
        if (_timer < damageCooldown) return;

        float dist = Vector3.Distance(transform.position, _player.position);

        if (dist <= damageRadius)
        {
            HealthSystem hs = _player.GetComponent<HealthSystem>();
            if (hs != null)
            {
                hs.TakeDamage(damageAmount);
                // Debug.Log($"{name} dealt {damageAmount} damage to player.");
            }

            _timer = 0f; // reset cooldown
        }
    }

    // Draw damage radius in Scene View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
