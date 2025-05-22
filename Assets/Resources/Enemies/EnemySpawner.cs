using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave {

        public string waveName;
        public List<EnemyGroup> enemyGroups;
        public int waveQuota;
        public float spawnInterval;
        public int spawnCount;
    }
    [System.Serializable]
    public class EnemyGroup {
        public GameObject enemyPrefab;
        public string enemyName;
        public int enemyCount;
        public int spawnCount;
    }

    public float spawnRadius = 10f;
    private float spawnTimer = 0f;
    public int enemiesAlive = 0;
    public int maxEnemiesAllowed = 100;
    public bool maxEnemiesReached = false;

    public List<Wave> waves;
    public int currentWaveCount = 0;
    public float waveInterval = 60f;

    private Transform player;

    void Start() {
        CalculateWaveQuota();
        player = FindObjectOfType<PlayerCore>().transform;
    }

    void Update(){
        if (currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0) StartCoroutine(BeginNextWave());

        spawnTimer += Time.deltaTime;

        if(spawnTimer >= waves[currentWaveCount].spawnInterval) {
            SpawnEnemies();
            spawnTimer = 0f;
        }
    }

    void CalculateWaveQuota() {
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups) {
            waves[currentWaveCount].waveQuota += enemyGroup.enemyCount;
        }
        Debug.Log($"Wave {waves[currentWaveCount].waveName} quota: {waves[currentWaveCount].waveQuota}");
    }

    void SpawnEnemies() {
        if(maxEnemiesReached){
            if(enemiesAlive < maxEnemiesAllowed) maxEnemiesReached = false;
            else return;
        }

        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota) {
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups) {
                if (enemyGroup.spawnCount < enemyGroup.enemyCount) {

                    if(enemiesAlive >= maxEnemiesAllowed) {
                        maxEnemiesReached = true;
                        return;
                    }

                    Vector2 randomPoint = Random.insideUnitCircle;
                    Vector3 spawnOffset = (Vector3)randomPoint.normalized * spawnRadius;
                    Vector3 spawnPoint = player.transform.position + spawnOffset;

                    Instantiate(enemyGroup.enemyPrefab, spawnPoint, Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;
                }
            }
        }
    }

    IEnumerator BeginNextWave() {
        yield return new WaitForSeconds(waveInterval);
        if (currentWaveCount < waves.Count - 1) {
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }
}
