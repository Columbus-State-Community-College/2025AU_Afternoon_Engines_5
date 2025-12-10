using UnityEngine;

public class SpeedsterOnHit : MonoBehaviour
{
    [Tooltip("How much stamina to drain from the player per hit.")]
    public float staminaDrain = 25f;

    public void DrainStamina(GameObject player)
    {
        if (player == null) return;

        var movement = player.GetComponent<PlayerMovement>();
        if (movement == null)
        {
            Debug.LogWarning($"{name}: Player has no PlayerMovement component.");
            return;
        }

        movement.energy = Mathf.Max(0f, movement.energy - staminaDrain);
    }
}
