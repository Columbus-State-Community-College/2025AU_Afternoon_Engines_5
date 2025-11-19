using UnityEngine;

public class WanderBehaviour : EnemyAgentBase
{
    [Header("Wander")]
    public float wanderRadius = 10f;
    public float changeDirectionInterval = 4f;

    protected override void Update()
    {
        base.Update();
        timer += Time.deltaTime;

        if (agent.remainingDistance <= agent.stoppingDistance || timer >= changeDirectionInterval)
        {
            timer = 0f;
            Vector3 random = Random.insideUnitSphere * wanderRadius;
            random.y = 0f;
            SetDestinationIfOnNavMesh(transform.position + random, wanderRadius);
        }
    }
}
