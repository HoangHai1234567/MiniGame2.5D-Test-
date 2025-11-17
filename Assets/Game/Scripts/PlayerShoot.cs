using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Arrow")]
    public Arrow arrowPrefab;
    public Transform firePos;

    [Header("Parabola Settings")]
    public float baseArcHeight = 3f;
    public float extraHeightPerDistance = 0.3f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AutoShootRandomEnemy();
        }
    }

    void AutoShootRandomEnemy()
    {
        EnemyIdentity[] enemies = FindObjectsOfType<EnemyIdentity>();

        if (enemies.Length == 0)
        {
            Debug.Log("[PlayerShoot] Không có enemy → không bắn");
            return;
        }

        EnemyIdentity target = enemies[Random.Range(0, enemies.Length)];
        ShootArrowTo(target);
    }

    void ShootArrowTo(EnemyIdentity enemy)
    {
        Vector3 startPos = firePos.position;
        Vector3 targetPos = enemy.transform.position;

        float dist = Vector2.Distance(startPos, targetPos);
        float arcHeight = baseArcHeight + dist * extraHeightPerDistance;

        // ---- Tính velocity parabol an toàn, không NaN ----
        Vector2 velocity = CalculateParabolaVelocity(startPos, targetPos, arcHeight);

        // Nếu velocity sai → không bắn
        if (float.IsNaN(velocity.x) || float.IsNaN(velocity.y))
        {
            Debug.LogError("[Shoot] Velocity bị NaN! Hủy bắn mũi tên.");
            return;
        }

        // ---- Tạo arrow và bắn ----
        Arrow arrow = Instantiate(arrowPrefab, startPos, Quaternion.identity);
        arrow.SetTargetEnemy(enemy.EnemyId);
        arrow.Launch(velocity);
    }

    // -----------------------------
    // ★ Công thức parabol SAFE
    // -----------------------------
    Vector2 CalculateParabolaVelocity(Vector2 start, Vector2 end, float height)
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y);

        Vector2 displacement = end - start;

        float t1 = Mathf.Sqrt(2 * height / gravity);
        float t2 = Mathf.Sqrt(2 * Mathf.Max(0, height - displacement.y) / gravity);

        float time = t1 + t2;

        float velX = displacement.x / time;
        float velY = Mathf.Sqrt(2 * gravity * height);

        return new Vector2(velX, velY);
    }
}
