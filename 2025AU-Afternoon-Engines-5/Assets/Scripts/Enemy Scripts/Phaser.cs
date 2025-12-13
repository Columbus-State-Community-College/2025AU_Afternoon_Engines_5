using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class PhaserGhost : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Renderer[] renderers;       
    public string playerTag = "Player";

    [Header("Phasing Settings")]
    public float minPhaseInterval = 3f;    
    public float maxPhaseInterval = 7f;    
    public float phaseDuration = 0.5f;     
    public float teleportRadius = 5f;      
    public bool teleportNearPlayer = false;
    public float teleportNearPlayerRadius = 8f;

    [HideInInspector] public UnityEvent phaseOutEvent = new();
    [HideInInspector] public UnityEvent phaseInEvent = new();
    
    private Transform _player;
    private float _nextPhaseTime;
    private bool _isPhasing = false;

    void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        GameObject p = GameObject.FindGameObjectWithTag(playerTag);
        if (p != null) _player = p.transform;

        ScheduleNextPhase();
    }

    void Update()
    {
        if (_isPhasing) return;

        if (Time.time >= _nextPhaseTime)
        {
            StartCoroutine(PhaseCoroutine());
        }
    }

    private System.Collections.IEnumerator PhaseCoroutine()
    {
        _isPhasing = true;

        bool prevStopped = agent.isStopped;
        agent.isStopped = true;

        SetVisible(false);
        
        phaseOutEvent.Invoke();

        float t = 0f;
        while (t < phaseDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        Vector3 targetPos = ChooseTeleportPosition();

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, teleportRadius, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }

        SetVisible(true);
        agent.isStopped = prevStopped;

        _isPhasing = false;
        ScheduleNextPhase();
        
        phaseInEvent.Invoke();
    }

    Vector3 ChooseTeleportPosition()
    {
        if (teleportNearPlayer && _player != null)
        {
            Vector2 rnd = Random.insideUnitCircle * teleportNearPlayerRadius;
            Vector3 aroundPlayer = _player.position + new Vector3(rnd.x, 0f, rnd.y);
            return aroundPlayer;
        }
        else
        {
            Vector2 rnd = Random.insideUnitCircle * teleportRadius;
            Vector3 aroundSelf = transform.position + new Vector3(rnd.x, 0f, rnd.y);
            return aroundSelf;
        }
    }

    void ScheduleNextPhase()
    {
        _nextPhaseTime = Time.time + Random.Range(minPhaseInterval, maxPhaseInterval);
    }

    void SetVisible(bool visible)
    {
        if (renderers == null || renderers.Length == 0) return;

        foreach (var r in renderers)
        {
            if (r == null) continue;
            r.enabled = visible;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, teleportRadius);

        if (teleportNearPlayer && _player != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_player.position, teleportNearPlayerRadius);
        }
    }
}
