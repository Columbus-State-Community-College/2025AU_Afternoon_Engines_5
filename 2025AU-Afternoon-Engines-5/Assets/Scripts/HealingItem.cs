using UnityEngine;

public class HealingItem : MonoBehaviour
{
    public float healAmount = 25f;
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        HealthSystem hs = other.GetComponent<HealthSystem>();
        if (hs != null)
        {
            hs.GiveHealth(healAmount);
        }

        Destroy(gameObject); 
    }
}
