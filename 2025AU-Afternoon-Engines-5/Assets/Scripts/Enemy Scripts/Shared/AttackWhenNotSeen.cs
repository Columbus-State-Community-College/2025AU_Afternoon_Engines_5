using UnityEngine;

public class AttackWhenNotSeen : EnemyAgentBase
{
    [Header("Chase / Attack Movement")]
    public float chaseRadius = 10f;
    public float stopDistance = 1.5f;

    protected override void Update()
    {
        base.Update();
        if (!player) return;

        if (!PlayerInRange(chaseRadius)) return;

        if (IsPlayerLookingAtMe()) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        if (toPlayer.sqrMagnitude > stopDistance * stopDistance)
        {
            Vector3 targetPos = player.position - toPlayer.normalized * stopDistance;
            SetDestinationIfOnNavMesh(targetPos, 2f);
        }
        else
        {
            agent.ResetPath();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}
