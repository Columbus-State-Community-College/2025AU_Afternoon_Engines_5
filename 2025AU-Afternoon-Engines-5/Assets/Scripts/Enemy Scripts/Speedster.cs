using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Ghost_Speedster : MonoBehaviour
{
    [Header("Speed Boost")]
    public float speedMultiplier = 1.7f; // faster than base ghost
    public float accelMultiplier = 1.4f;

    private NavMeshAgent agent;
    private float baseSpeed, baseAccel;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        baseSpeed = agent.speed;
        baseAccel = agent.acceleration;
    }

    void OnEnable()
    {
        agent.speed = baseSpeed * speedMultiplier;
        agent.acceleration = baseAccel * accelMultiplier;
    }

    void OnDisable()
    {
        agent.speed = baseSpeed;
        agent.acceleration = baseAccel;
    }
}
