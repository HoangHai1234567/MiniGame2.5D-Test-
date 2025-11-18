using UnityEngine;
using System.Collections.Generic;

public class PlayerShoot : MonoBehaviour
{
    [Header("Arrow")]
    public Arrow arrowPrefab;
    public Transform firePos;

    [Header("Parabola Settings")]
    public float baseArcHeight = 3f;
    public float extraHeightPerDistance = 0.3f;

    // các enemy đã có arrow đang bay tới
    private HashSet<int> reservedEnemyIds = new HashSet<int>();

    public static PlayerShoot Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AutoShootEnemyAvoidReserved();
        }
    }

    void AutoShootEnemyAvoidReserved()
    {
        EnemyIdentity[] enemies = FindObjectsOfType<EnemyIdentity>();

        if (enemies.Length == 0)
        {
            Debug.Log("[PlayerShoot] Không có enemy → không bắn");
            return;
        }

        // 1. “Ma trận” vị trí (nếu cần dùng thêm)
        Vector2[] enemyPositions = new Vector2[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            enemyPositions[i] = enemies[i].transform.position;
        }

        // 2. Lọc enemy hợp lệ:
        //    - Collider enable (=> entrance xong, đang có thể va chạm)
        //    - Không nằm trong reservedEnemyIds (chưa bị mũi tên nào lock)
        //    - Nếu có EnemyPullable thì phải CanBeTargeted == true
        List<int> candidateIndices = new List<int>();

        for (int i = 0; i < enemies.Length; i++)
        {
            int id = enemies[i].EnemyId;

            // đang bị "reserve" bởi mũi tên khác
            if (reservedEnemyIds.Contains(id))
                continue;

            // Collider (trên root / parent)
            Collider2D col = enemies[i].GetComponentInParent<Collider2D>();
            if (col == null || !col.enabled)
            {
                // Enemy đang entrance hoặc bị disable collider → không bắn
                continue;
            }

            // Kiểm tra trạng thái float/attack từ EnemyPullable
            EnemyPullable pullable = enemies[i].GetComponentInParent<EnemyPullable>();
            if (pullable != null && !pullable.CanBeTargeted)
            {
                // Enemy đang bị kéo hoặc đứng ở attack pos → không bắn
                continue;
            }

            candidateIndices.Add(i);
        }

        if (candidateIndices.Count == 0)
        {
            Debug.Log("[PlayerShoot] Không có enemy nào sẵn sàng bị bắn.");
            return;
        }

        // 3. Chọn ngẫu nhiên 1 enemy trong nhóm hợp lệ
        int chosenIndex = candidateIndices[Random.Range(0, candidateIndices.Count)];
        EnemyIdentity target = enemies[chosenIndex];
        Vector2 targetPos = enemyPositions[chosenIndex];

        // Đánh dấu enemy này đã được gán 1 mũi tên
        reservedEnemyIds.Add(target.EnemyId);

        ShootArrowTo(target, targetPos);
    }

    void ShootArrowTo(EnemyIdentity enemy, Vector2 targetPos)
    {
        Vector3 startPos = firePos.position;

        float dist = Vector2.Distance(startPos, targetPos);
        float arcHeight = baseArcHeight + dist * extraHeightPerDistance;

        Vector2 velocity = CalculateParabolaVelocity(startPos, targetPos, arcHeight);

        if (float.IsNaN(velocity.x) || float.IsNaN(velocity.y))
        {
            Debug.LogError("[PlayerShoot] Velocity NaN, hủy bắn.");
            return;
        }

        Arrow arrow = Instantiate(arrowPrefab, startPos, Quaternion.identity);
        arrow.SetTargetEnemy(enemy.EnemyId);
        arrow.Launch(velocity);
    }

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

    // 🔁 Cho Arrow gọi lại nếu mũi tên bị destroy mà KHÔNG trúng (miss)
    public void ReleaseReservedTarget(int enemyId)
    {
        reservedEnemyIds.Remove(enemyId);
    }
}
