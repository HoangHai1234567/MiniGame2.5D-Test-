using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    public GameObject enemyPrefab;        
    public Transform enemyParent;         

    [Header("Spawn Points (kéo 9 điểm vào)")]
    public Transform[] spawnPoints;

    [Header("Settings")]
    public float spawnDelay = 0.1f;   // delay nhỏ giữa từng enemy
    public float waveDelay = 0.3f;    // delay trước wave mới

    private bool isSpawning = false;
    private int currentWave = 0;

    void Start()
    {
        Debug.Log("[Spawner] Bắt đầu game → spawn wave đầu");
        SpawnWave();
    }

    /// <summary>
    /// Hàm spawn 1 wave mới
    /// </summary>
    public void SpawnWave()
    {
        if (isSpawning)
        {
            Debug.LogWarning("[Spawner] Đang spawn wave khác, không spawn thêm");
            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("[Spawner] ❌ enemyPrefab = NULL!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[Spawner] ❌ Không có spawnPoints!");
            return;
        }

        currentWave++;
        Debug.Log($"[Spawner] ▶ Spawn Wave #{currentWave}");

        StartCoroutine(SpawnWaveRoutine());
    }

    IEnumerator SpawnWaveRoutine()
    {
        isSpawning = true;

        yield return new WaitForSeconds(waveDelay);

        foreach (Transform point in spawnPoints)
        {
            if (point == null)
            {
                Debug.LogWarning("[Spawner] ⚠ spawnPoint NULL → bỏ qua");
                continue;
            }

            // spawn enemy tại 1 vị trí
            GameObject e = Instantiate(
                enemyPrefab,
                point.position,
                Quaternion.identity,
                enemyParent
            );

            // Đăng ký enemy vào EnemyManager
            EnemyManager.Instance.RegisterEnemy();

            Debug.Log($"[Spawner] ✓ Spawned enemy tại {point.position}");

            yield return new WaitForSeconds(spawnDelay);
        }

        Debug.Log("[Spawner] ✔ Hoàn tất 1 wave");
        isSpawning = false;
    }
}