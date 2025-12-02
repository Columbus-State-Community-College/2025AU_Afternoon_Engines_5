using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveSystem : MonoBehaviour
{
    public List<Wave> waves;
    public int currentWave = 0;
    public float waveBreakTime = 30f;
    [HideInInspector] public bool allWavesCompleted = false;
    public ItemDefinition winCollectibleDef;
    public int winCollectibleCount;

    private List<GameObject> _enemyList = new();
    private bool _overflowEnemies = true;
    private bool _preparingWave;
    private Bounds _spawnBounds;
    private bool _waveActive;
    private GameObject _resultsScreen;
    private TextMeshProUGUI _resultText;
    private TextMeshProUGUI _waveInfoText;
    private TextMeshProUGUI _collectibleText;
    private PlayerInventory _playerInventory;

    private void Start()
    {
        _spawnBounds = GameObject.Find("Spawn Bounds").GetComponent<Collider>().bounds;
        _playerInventory = GetComponent<PlayerInventory>();
        _playerInventory.pickupEvent.AddListener(OnItemPickup);
        _waveInfoText = GameObject.Find("/UI/Wave Info").GetComponent<TextMeshProUGUI>();
        _collectibleText = GameObject.Find("/UI/Doll Counter").GetComponent<TextMeshProUGUI>();

        var resultsScreen = Resources.FindObjectsOfTypeAll<Menu>().FirstOrDefault(item => item.name == "ResultsScreen");

        if (resultsScreen is not null)
        {
            _resultsScreen = resultsScreen.gameObject;
            _resultText = resultsScreen.transform.Find("ResultText").GetComponent<TextMeshProUGUI>();
        }

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
        UpdateWaveInfo();
        UpdateItemCounter();
        _preparingWave = false;
        _waveActive = true;
        _overflowEnemies = true;
    }

    public void EndWave()
    {
        _waveActive = false;
        currentWave++;
        DestroyEnemies();
        if (currentWave > waves.Count - 1)
        {
            allWavesCompleted = true;
            return;
        }
        StartCoroutine(WaveBreakCoroutine());
    }

    private IEnumerator WaveBreakCoroutine()
    {
        var timer = 0f;

        while (timer < waveBreakTime)
        {
            timer += Time.deltaTime;
            _waveInfoText.text = $"Next wave in {(int)(waveBreakTime - timer)}s";
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
        MainManager.Instance.PauseGame();
        _resultText.text = "You win!";
        _resultsScreen.SetActive(true);
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
            if (stack.def.id != winCollectibleDef.id || stack.count < winCollectibleCount) continue;
            
            _playerInventory.items.Remove(stack);
            return true;
        }

        return false;
    }

    private void OnItemPickup()
    {
        foreach (var stack in _playerInventory.items)
        {
            if (stack.def.id != winCollectibleDef.id) continue;

            UpdateItemCounter(stack);
            return;
        }
    }

    private void UpdateItemCounter([CanBeNull] PlayerInventory.Stack stack = null)
    {
        _collectibleText.text = stack is null ? $"0/{winCollectibleCount} {winCollectibleDef.displayName}s found" : $"{stack.count}/{winCollectibleCount} {winCollectibleDef.displayName}s found";
    }

    private void UpdateWaveInfo()
    {
        _waveInfoText.text = $"Wave {currentWave + 1}";
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