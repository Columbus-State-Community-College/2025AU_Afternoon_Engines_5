using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pickup : MonoBehaviour
{
    [Header("Item")]
    public ItemDefinition definition;
    public int amount = 1;

    [Header("Pickup Options")]
    public bool autoPickup = false;   // if true, picks up on enter; else press key
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    [Header("UI Prompt (optional)")]
    public GameObject promptUI; // ex/ "Press E to pick up Battery"

    Collider col;
    bool playerInTrigger = false;
    Transform player;

    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true; //item pickup uses triggers
        if (promptUI) promptUI.SetActive(false);
    }

    void Update()
    {
        if (!playerInTrigger || autoPickup) return;

        if (Input.GetKeyDown(interactKey))
        {
            TryGiveToPlayer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInTrigger = true; player = other.transform;

        if (autoPickup)
        {
            TryGiveToPlayer();
        }
        else if (promptUI) promptUI.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInTrigger = false; player = null;
        if (promptUI) promptUI.SetActive(false);
    }

    void TryGiveToPlayer()
    {
        if (!definition) { Debug.LogWarning($"Pickup {name} missing ItemDefinition"); return; }

        var inv = player ? player.GetComponentInChildren<PlayerInventory>() : null;
        if (inv)
        {
            inv.Add(definition, amount);
        }
        else
        {
            Debug.Log($"Player has no PlayerInventory; destroying item anyway for demo.");
        }

        if (promptUI) promptUI.SetActive(false);
        Destroy(gameObject);
    }
}
