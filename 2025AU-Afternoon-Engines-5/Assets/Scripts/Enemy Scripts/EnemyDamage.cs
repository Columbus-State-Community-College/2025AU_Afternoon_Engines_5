using UnityEngine;
using UnityEngine.Events;

public class EnemyDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageAmount = 5f;
    public float damageRadius = 1.5f;
    public float damageCooldown = 1f;
    public string playerTag = "Player";

    [Header("View / POV")]
    [Tooltip("Field of view used to determine if the player is looking at this enemy.")]
    public float playerViewFov = 90f;

    [Header("Events")]
    [Tooltip("Called whenever this enemy successfully damages the player.")]
    public UnityEvent<GameObject> OnHitPlayer;

    private Transform _player;
    private Transform _playerView;
    private float _timer;

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag(playerTag);
        if (p != null)
            _player = p.transform;

        if (Camera.main != null)
            _playerView = Camera.main.transform;
        else
            _playerView = _player;
    }

    private void Update()
    {
        if (_player == null) return;

        _timer += Time.deltaTime;
        if (_timer < damageCooldown) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        if (dist > damageRadius) return;

        if (IsPlayerLookingAtMe()) return;

        var hs = _player.GetComponent<HealthSystem>();
        if (hs != null)
        {
            hs.TakeDamage(damageAmount);

            OnHitPlayer?.Invoke(_player.gameObject);
        }

        _timer = 0f;
    }

    private bool IsPlayerLookingAtMe()
    {
        if (_playerView == null) return false;

        Vector3 toEnemy = (transform.position - _playerView.position).normalized;
        float dot = Vector3.Dot(_playerView.forward, toEnemy);
        float cosHalfFov = Mathf.Cos(playerViewFov * 0.5f * Mathf.Deg2Rad);

        return dot >= cosHalfFov;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
