using UnityEngine;

public class EnemyPullController : MonoBehaviour
{
    public Transform attackPos;

    private EnemyPullable currentEnemy;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PullOneEnemy();
        }
    }

    void PullOneEnemy()
    {
        if (attackPos == null)
        {
            Debug.LogWarning("Chưa gán AttackPos!");
            return;
        }

        // Nếu enemy cũ chết thì chọn lại
        if (currentEnemy == null || !currentEnemy.gameObject.activeInHierarchy)
        {
            EnemyPullable[] allEnemies = FindObjectsOfType<EnemyPullable>();
            if (allEnemies.Length == 0)
            {
                Debug.Log("Không có enemy nào!");
                return;
            }

            // lấy random
            currentEnemy = allEnemies[Random.Range(0, allEnemies.Length)];
        }

        currentEnemy.TogglePull(attackPos.position);
    }
}


