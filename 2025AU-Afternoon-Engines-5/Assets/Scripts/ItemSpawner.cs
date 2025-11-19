using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnEntry
    {
        public ItemDefinition item;
        public GameObject pickupPrefab; // prefab with Pickup.cs referencing 'item'
        [Range(0f, 1f)] public float chance = 1f; // spawn probability per roll
        public int minAmount = 1;
        public int maxAmount = 1;
    }

    [Header("Spawn List")]
    public List<SpawnEntry> entries = new List<SpawnEntry>();

    [Header("Points OR Area")]
    public Transform[] spawnPoints;     // optional fixed points
    public bool useArea = false;
    public Vector2 areaSize = new Vector2(10, 10); // XZ area centered on this object

    [Header("Counts & Timing")]
    public int totalToSpawn = 5;
    public bool allowRespawn = false;
    public float respawnDelay = 30f;

    [Header("Height")]
    public float dropHeight = 0.1f;  // keep items slightly above ground
    public LayerMask groundMask = ~0;

    void Start()
    {
        SpawnBatch(totalToSpawn);

        if (allowRespawn)
            StartCoroutine(RespawnLoop());
    }

    IEnumerator RespawnLoop()
    {
        while (allowRespawn)
        {
            yield return new WaitForSeconds(respawnDelay);
            SpawnBatch(1);
        }
    }

    void SpawnBatch(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var entry = RollEntry();
            if (entry == null || entry.pickupPrefab == null) continue;

            Vector3 pos = useArea ? RandomPointInArea() : NextPointFromArray();
            pos = SnapToGround(pos, dropHeight);

            var go = Instantiate(entry.pickupPrefab, pos, Quaternion.identity);

            // ensure the instance has the correct item & amount
            var p = go.GetComponent<Pickup>();
            if (p)
            {
                p.definition = entry.item;
                p.amount = Random.Range(entry.minAmount, entry.maxAmount + 1);
            }
        }
    }

    SpawnEntry RollEntry()
    {
        // simple roll: pick random entry that passes chance; retry a few times
        for (int tries = 0; tries < 10; tries++)
        {
            if (entries.Count == 0) return null;
            var idx = Random.Range(0, entries.Count);
            var e = entries[idx];
            if (e != null && Random.value <= e.chance) return e;
        }
        return null;
    }

    int lastPoint = -1;
    Vector3 NextPointFromArray()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            lastPoint = (lastPoint + 1) % spawnPoints.Length;
            return spawnPoints[lastPoint].position;
        }
        return transform.position;
    }

    Vector3 RandomPointInArea()
    {
        Vector2 r = new Vector2(
            Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f),
            Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f)
        );
        return transform.position + new Vector3(r.x, 0f, r.y);
    }

    Vector3 SnapToGround(Vector3 pos, float offset)
    {
        // raycast down to land on terrain/geometry
        if (Physics.Raycast(pos + Vector3.up * 5f, Vector3.down, out var hit, 20f, groundMask))
            return hit.point + Vector3.up * offset;

        return pos + Vector3.up * offset;
    }

    void OnDrawGizmosSelected()
    {
        if (useArea)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, 0.1f, areaSize.y));
        }

        if (spawnPoints != null)
        {
            Gizmos.color = Color.cyan;
            foreach (var t in spawnPoints)
                if (t) Gizmos.DrawSphere(t.position, 0.2f);
        }
    }
}
