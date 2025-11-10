using UnityEngine;

public class FleeIfNearPlayer : EnemyAgentBase
{
    [Header("Flee")]
    public float fleeTriggerDistance = 6f;
    public float fleeBurstDistance = 8f;

    protected override void Update()
    {
        base.Update();
        if (!player) return;

        if (PlayerInRange(fleeTriggerDistance))
        {
            Vector3 away = (transform.position - player.position).normalized;
            Vector3 target = transform.position + away * fleeBurstDistance;
            SetDestinationIfOnNavMesh(target, fleeBurstDistance);
        }
    }
}
