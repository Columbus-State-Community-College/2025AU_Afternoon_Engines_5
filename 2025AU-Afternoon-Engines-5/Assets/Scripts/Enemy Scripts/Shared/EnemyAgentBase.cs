using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAgentBase : MonoBehaviour
{
    [Header("Common")]
    public float hoverHeight = 1.5f;
    public string playerTag = "Player";

    [Header("Perception")]
    public float detectRadius = 10f;

    public NavMeshAgent agent;
    protected Transform player;
    protected float timer;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false; // keeps movement planar
    }

    protected virtual void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag(playerTag);
        if (p) player = p.transform;
        MaintainHoverHeight();
    }

    protected virtual void Update()
    {
        MaintainHoverHeight();
    }

    protected bool PlayerInRange(float radius)
    {
        if (!player) return false;
        return Vector3.Distance(transform.position, player.position) <= radius;
    }

    protected void SetDestinationIfOnNavMesh(Vector3 worldPos, float sampleRadius = 4f)
    {
        if (NavMesh.SamplePosition(worldPos, out var hit, sampleRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }

    protected void MaintainHoverHeight()
    {
        var pos = transform.position;
        pos.y = hoverHeight;
        transform.position = pos;
    }
}
