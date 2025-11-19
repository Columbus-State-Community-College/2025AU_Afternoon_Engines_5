using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(HealthSystem))]
public class ReaperBoss : EnemyAgentBase
{
    public enum State { Idle, Patrol, Chase, Attack }
    public State state = State.Patrol;

    [Header("Patrol")]
    public float patrolRadius = 15f;
    public float nodeDwellTime = 2f;

    [Header("Chase")]
    public float chaseRadius = 18f;   // bigger than detectRadius for persistence
    public float attackRange = 2.2f;

    [Header("Attack")]
    public float attackCooldown = 1.75f;
    public int attackDamage = 20;

    private float dwellTimer = 0f;
    private float attackTimer = 0f;

    private HealthSystem hp;

    protected override void Start()
    {
        base.Start();
        hp = GetComponent<HealthSystem>();
        hp.isInvulnerable = true;            // cannot be killed/captured
        agent.speed = 2.4f;                // heavy/inevitable feel
        agent.acceleration = 5f;
        agent.angularSpeed = 120f;
        GoToNextPatrolPoint();
    }

    protected override void Update()
    {
        base.Update();
        attackTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Patrol:
                PatrolTick();
                if (PlayerInRange(detectRadius)) state = State.Chase;
                break;

            case State.Chase:
                ChaseTick();
                if (!PlayerInRange(chaseRadius)) state = State.Patrol;
                break;

            case State.Attack:
                AttackTick();
                break;
        }
    }

    void PatrolTick()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            dwellTimer += Time.deltaTime;
            if (dwellTimer >= nodeDwellTime)
            {
                dwellTimer = 0f;
                GoToNextPatrolPoint();
            }
        }
    }

    void ChaseTick()
    {
        if (!player) return;

        SetDestinationIfOnNavMesh(player.position, 3f);

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackRange)
        {
            agent.ResetPath();
            state = State.Attack;
        }
    }

    void AttackTick()
    {
        if (!player) { state = State.Patrol; return; }

        // face player (optional slow turn for menace)
        Vector3 look = player.position - transform.position;
        look.y = 0f;
        if (look.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), 3f * Time.deltaTime);

        if (attackTimer <= 0f)
        {
            // TODO: hook to your player Health/Controller
            if (Vector3.Distance(transform.position, player.position) <= attackRange + 0.2f)
            {
                // Example:
                // player.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
            }
            attackTimer = attackCooldown;
        }

        // if player escapes a bit, resume chase
        if (Vector3.Distance(transform.position, player.position) > attackRange + 0.75f)
            state = State.Chase;
    }

    void GoToNextPatrolPoint()
    {
        Vector3 rnd = Random.insideUnitSphere * patrolRadius; rnd.y = 0f;
        SetDestinationIfOnNavMesh(transform.position + rnd, patrolRadius);
    }
}
