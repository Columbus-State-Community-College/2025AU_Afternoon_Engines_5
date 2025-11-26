using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveSystem : MonoBehaviour
{
    public List<Wave> waves;
    public int currentWave = 0;
    public float waveBreakTime = 30f;
    [HideInInspector] public float remainingBreakTime;
    [HideInInspector] public bool allWavesCompleted = false;
    public string winCollectibleId;
    public int winCollectibleCount;

    private List<GameObject> _enemyList = new();
    private bool _overflowEnemies = true;
    private bool _preparingWave;
    private Bounds _spawnBounds;
    private bool _waveActive;
    private TextMeshProUGUI _resultText;
    private PlayerInventory _playerInventory;

    private void Start()
    {
        _spawnBounds = GameObject.Find("Spawn Bounds").GetComponent<Collider>().bounds;
        _resultText = GameObject.Find("UI").transform.Find("ResultsScreen").transform.Find("ResultText").GetComponent<TextMeshProUGUI>();
        _playerInventory = GetComponent<PlayerInventory>();

        StartWave();
    }

    private void Update()
    {
        if (allWavesCompleted) ShowWinScreen();
        
        if (!_waveActive || _preparingWave) return;

        for (var i = 0; i < _enemyList.Count; i++)
        {
            if (_enemyList[i]) continue;
            
            _enemyList.RemoveAt(i);
        }

        if (_enemyList.Count < waves[currentWave].maxSpawn && _overflowEnemies) SpawnEnemies();

        if (CheckForWin()) EndWave();
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
        if (currentWave > waves.Count - 1) allWavesCompleted = true;
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
        for (var i = 0; i < waves[currentWave].maxSpawn - _enemyList.Count; ++i)
        {
            var totalCount = waves[currentWave].enemyCounts.Sum();
            
            if (totalCount <= 0)
            {
                _overflowEnemies = false;
                return;
            }

            var randEnemyNumber = Random.Range(0, totalCount + 1);
            var oddsCheck = 0;
            GameObject enemy = null;

            if (waves[currentWave].enemies[0].name == "Reaper" && waves[currentWave].enemyCounts[0] > 0)
            {
                enemy = waves[currentWave].enemies[0];
                waves[currentWave].enemyCounts[0] -= 1;
            }
            else
            {
                for (var j = 0; j < waves[currentWave].enemies.Count; j++)
                {
                    if (waves[currentWave].enemyCounts[j] == 0) continue;
                    
                    oddsCheck += waves[currentWave].enemyCounts[j];
                
                    if (oddsCheck < randEnemyNumber) continue;
                    
                    enemy = waves[currentWave].enemies[j];

                    if (!waves[currentWave].continuous) waves[currentWave].enemyCounts[j] -= 1;

                    break;
                }
            }

            if (enemy is null) return;

            var offsetX = Random.Range(-_spawnBounds.extents.x, _spawnBounds.extents.x);
            var offsetZ = Random.Range(-_spawnBounds.extents.z, _spawnBounds.extents.z);
            Vector3 spawnPosition = new(offsetX, 0, offsetZ);
            var offsetY = Terrain.activeTerrain.SampleHeight(spawnPosition);
            spawnPosition.y = offsetY;
            var enemyObject = Instantiate(enemy, spawnPosition, Quaternion.identity);
            
            _enemyList.Add(enemyObject);
        }
    }

    private void DestroyEnemies()
    {
        foreach (var enemy in _enemyList) Destroy(enemy);
        _enemyList.Clear();
    }

    private void ShowWinScreen()
    {
        _resultText.text = "You win!";
        Time.timeScale = 0f;
    }

    /*private bool CheckForWin()
    {
        foreach (var enemy in _enemyList)
        {
            if (!enemy.name.Contains("Reaper")) return false;
        }

        return true;
    }*/

    private bool CheckForWin()
    {
        foreach (var stack in _playerInventory.items)
        {
            if (stack.def.id != winCollectibleId || stack.count < winCollectibleCount) continue;

            return true;
        }

        return false;
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