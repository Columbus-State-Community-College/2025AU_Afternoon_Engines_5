using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WanderingGhost : MonoBehaviour
{
    [Header("Movement Settings")]
    public float wanderRadius = 10f;
    public float changeDirectionInterval = 4f;
    public float avoidDistance = 6f;
    public float hoverHeight = 1.5f;

    private NavMeshAgent agent;
    private Transform player;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;

        // Ghost should not rotate to face movement direction
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Keep a consistent floating height (turn off gravity)
        MaintainHoverHeight();

        PickNewWanderLocation();
    }

    void Update()
    {
        timer += Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < avoidDistance)
        {
            MoveAwayFromPlayer();
        }
        else if (timer >= changeDirectionInterval || agent.remainingDistance <= agent.stoppingDistance)
        {
            PickNewWanderLocation();
        }

        MaintainHoverHeight();
    }

    void PickNewWanderLocation()
    {
        timer = 0f;

        // Pick a random point within the wanderRadius
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        randomDirection.y = transform.position.y;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);
        }
    }

    void MoveAwayFromPlayer()
    {
        Vector3 directionAway = (transform.position - player.position).normalized;
        Vector3 newTarget = transform.position + directionAway * wanderRadius;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(newTarget, out navHit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);
        }
    }

    void MaintainHoverHeight()
    {
        Vector3 pos = transform.position;
        pos.y = hoverHeight;
        transform.position = pos;
    }
}
