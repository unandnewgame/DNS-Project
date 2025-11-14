using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 2f;
    public int enemiesPerWave = 5;

    private int enemiesSpawned = 0;
    private bool spawning = false;

    // Track alive enemies
    private int aliveEnemies = 0;

    public bool IsWaveComplete()
    {
        return !spawning && aliveEnemies <= 0;
    }

    public void StartWave()
    {
        if (!spawning)
            StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        spawning = true;
        enemiesSpawned = 0;

        while (enemiesSpawned < enemiesPerWave)
        {
            SpawnEnemy();
            enemiesSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }

        spawning = false;
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Spawner missing enemyPrefab or spawn points!");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Listen for enemy death
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy != null)
        {
            aliveEnemies++;
            enemy.OnEnemyDeath += HandleEnemyDeath;
        }
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        aliveEnemies--;
        enemy.OnEnemyDeath -= HandleEnemyDeath; // cleanup
    }
}
