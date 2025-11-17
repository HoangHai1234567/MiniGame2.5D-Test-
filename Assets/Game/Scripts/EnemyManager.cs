using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [Header("Spawner")]
    public EnemySpawner spawner;

    [Header("Tracking")]
    public int currentEnemyCount = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(StartSpawn());
    }

    IEnumerator StartSpawn()
    {
        yield return null;
        spawner.SpawnWave();
    }

    public void RegisterEnemy()
    {
        currentEnemyCount++;
    }

    public void EnemyDied()
    {
        currentEnemyCount--;

        if (currentEnemyCount <= 0)
        {
            StartCoroutine(SpawnNextWave());
        }
    }

    IEnumerator SpawnNextWave()
    {
        yield return new WaitForSeconds(0.5f);
        spawner.SpawnWave();
    }
}
