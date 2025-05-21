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
    public float waveInterval = 5f;

    public List<Wave> waves;
    public int currentWaveCount = 0;

    private Transform player;

    void Start() {
        CalculateWaveQuota();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update(){
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
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota) {
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups) {
                if (enemyGroup.spawnCount < enemyGroup.enemyCount) {

                    Vector2 randomPoint = Random.insideUnitCircle;
                    Vector3 spawnPoint = new Vector3(randomPoint.x * spawnRadius + player.transform.position.x, randomPoint.y * spawnRadius + player.transform.position.y, 0f);

                    Instantiate(enemyGroup.enemyPrefab, spawnPoint, Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                }
            }
        }
    }
}
