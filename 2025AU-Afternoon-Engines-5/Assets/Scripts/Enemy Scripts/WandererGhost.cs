using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class Ghost_Wanderer : MonoBehaviour
{
    // This script exists mostly to flag the prefab as "Wanderer"
    // and to host ghost-specific tuning if needed later.
    [Header("Ghost Visuals")]
    public float idleSpinSpeed = 20f;

    void Update()
    {
        // subtle idle spin for ghostly vibe
        transform.Rotate(0f, idleSpinSpeed * Time.deltaTime, 0f, Space.World);
    }
}
