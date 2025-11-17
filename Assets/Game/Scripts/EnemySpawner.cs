using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    public GameObject enemyPrefab;        // Prefab enemy
    public Transform enemyParent;         // (optional) parent để chứa tất cả enemy

    [Header("Spawn Points")]
    public Transform[] spawnPoints;       // 9 điểm spawn, kéo vào Inspector

    [Header("Settings")]
    public bool spawnOnStart = true;      // Spawn wave đầu khi Start
    public float checkInterval = 1f;      // Khoảng thời gian kiểm tra còn enemy hay không

    void Start()
    {
        if (spawnOnStart)
        {
            SpawnWave();
        }

        // Bắt đầu vòng lặp kiểm tra và respawn
        StartCoroutine(CheckAndRespawnLoop());
    }

    void SpawnWave()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawner: Chưa gán enemyPrefab!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: Chưa gán spawnPoints!");
            return;
        }

        foreach (Transform point in spawnPoints)
        {
            if (point == null) continue;

            if (enemyParent != null)
            {
                Instantiate(enemyPrefab, point.position, point.rotation, enemyParent);
            }
            else
            {
                Instantiate(enemyPrefab, point.position, point.rotation);
            }
        }
    }

    IEnumerator CheckAndRespawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            // Nếu không còn enemy nào trong scene thì spawn wave mới
            if (!HasAliveEnemies())
            {
                SpawnWave();
            }
        }
    }

    bool HasAliveEnemies()
    {
        // Dùng Tag "Enemy" cho tất cả enemy
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        return enemies.Length > 0;
    }
}
