using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveSystem : MonoBehaviour
{
    public List<Wave> waves;
    public int currentWave;
    public float waveBreakTime = 30f;
    [DoNotSerialize] public float remainingBreakTime;

    private List<GameObject> _enemyList = new();
    private bool _overflowEnemies = true;
    private bool _preparingWave;
    private Bounds _spawnBounds;
    private bool _waveActive;

    private void Start()
    {
        _spawnBounds = GameObject.Find("Spawn Bounds").GetComponent<Collider>().bounds;

        StartWave();
    }

    private void Update()
    {
        if (!_waveActive || _preparingWave) return;

        for (var i = 0; i < _enemyList.Count; i++)
        {
            if (_enemyList[i] is null)
            {
                _enemyList.RemoveAt(i);
            }
        }
        
        if (_enemyList.Count < waves[currentWave].maxSpawn && _overflowEnemies) SpawnEnemies();
    }

    private void StartWave()
    {
        if (_waveActive) return;

        SpawnEnemies();
        _preparingWave = false;
        _waveActive = true;
        _overflowEnemies = true;
    }

    public void EndWave()
    {
        _waveActive = false;
        remainingBreakTime = waveBreakTime;
        currentWave++;
        DestroyEnemies();
        StartCoroutine(WaveBreakCoroutine());
    }

    private IEnumerator WaveBreakCoroutine()
    {
        var timer = 0f;

        while (timer < waveBreakTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        StartWave();
    }

    private void SpawnEnemies()
    {
        for (var i = 0; i < waves[currentWave].maxSpawn - _enemyList.Count; i++)
        {
            var totalCount = waves[currentWave].enemyCounts.Sum();

            if (totalCount < waves[currentWave].maxSpawn && _overflowEnemies && !waves[currentWave].continuous)
                _overflowEnemies = false;

            if (totalCount == 0) return;

            var randEnemyNumber = Random.Range(1, totalCount + 1);
            var oddsCheck = 0;
            GameObject enemy = null;

            for (var j = 0; j < waves[currentWave].enemyCounts.Count; j++)
            {
                oddsCheck += waves[currentWave].enemyCounts[j];
                if (randEnemyNumber > oddsCheck && j < waves[currentWave].enemyCounts.Count - 1) continue;

                enemy = waves[currentWave].enemies[j];

                if (!waves[currentWave].continuous) waves[currentWave].enemyCounts[j] -= 1;

                break;
            }

            if (enemy is null)
            {
                enemy = waves[currentWave].enemies[0];
                waves[currentWave].enemyCounts[0] -= 1;
            }
            
            var offsetX = Random.Range(-_spawnBounds.extents.x, _spawnBounds.extents.x);
            var offsetZ = Random.Range(-_spawnBounds.extents.z, _spawnBounds.extents.z);
            Vector3 spawnPosition = new(offsetX, 0, offsetZ);
            var offsetY = Terrain.activeTerrain.SampleHeight(spawnPosition);
            spawnPosition.y = offsetY + enemy.transform.lossyScale.y / 2;
            var enemyObject = Instantiate(enemy);
            enemyObject.transform.position = spawnPosition;
            
            _enemyList.Add(enemyObject);
        }
    }

    private void DestroyEnemies()
    {
        foreach (var enemy in _enemyList) Destroy(enemy);
        _enemyList.Clear();
    }
}

[Serializable]
public class Wave
{
    public List<GameObject> enemies;
    public List<int> enemyCounts;
    public int maxSpawn;
    public bool continuous;
}