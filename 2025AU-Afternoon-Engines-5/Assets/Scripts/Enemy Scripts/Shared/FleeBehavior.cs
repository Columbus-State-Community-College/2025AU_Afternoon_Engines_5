using UnityEngine;

public class FleeBehavior : EnemyAgentBase
{
    [Header("Flee")]
    public float fleeTriggerDistance = 6f;
    public float fleeBurstDistance = 8f;

    protected override void Update()
    {
        base.Update();
        if (!player) return;

        // Only flee if close AND the player is looking at this ghost
        if (PlayerInRange(fleeTriggerDistance) && IsPlayerLookingAtMe())
        {
            Vector3 away = (transform.position - player.position).normalized;
            Vector3 target = transform.position + away * fleeBurstDistance;
            SetDestinationIfOnNavMesh(target, fleeBurstDistance);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, fleeTriggerDistance);
    }
}
