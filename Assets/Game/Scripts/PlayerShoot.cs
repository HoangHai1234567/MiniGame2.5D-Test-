using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Arrow")]
    public GameObject arrowPrefab;
    public Transform firePos;

    [Header("Arc Settings")]
    public float arcHeight = 3f;              // đỉnh parabola cao hơn điểm cao nhất (start/target) bao nhiêu
    public float minArcHeight = 1f;           // tối thiểu để luôn có cong
    public float extraHeightPerUnit = 0.1f;   // tăng thêm theo khoảng cách (tuỳ chỉnh)

    [Header("Visual (optional)")]
    public Transform bow;                     // transform của cung/tay để xoay theo hướng bắn

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShootToMouseArc();
        }
    }

    void ShootToMouseArc()
    {
        if (!arrowPrefab || !firePos) return;
        if (Camera.main == null) return;

        // 1. Lấy vị trí click trong world
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 start = firePos.position;
        Vector2 end = mouseWorld;
        float dx = end.x - start.x;
        float distance = Vector2.Distance(start, end);

        // 2. Lấy gravity thực tế (có tính gravityScale của arrow)
        Rigidbody2D prefabRb = arrowPrefab.GetComponent<Rigidbody2D>();
        float gravityScale = prefabRb ? prefabRb.gravityScale : 1f;
        float gAbs = Mathf.Abs(Physics2D.gravity.y * gravityScale);

        if (gAbs < 0.0001f)
        {
            // không có gravity → fallback bắn thẳng
            Vector2 dir = (end - start).normalized;
            GameObject arrowLinear = Instantiate(arrowPrefab, start, Quaternion.identity);
            Arrow arrowComp = arrowLinear.GetComponent<Arrow>();
            if (arrowComp != null)
                arrowComp.Launch(dir * 10f);   // 🔹 chỉ 1 tham số
            return;
        }

        // 3. Chọn độ cao đỉnh parabola (apexY)
        float baseHeight = Mathf.Max(start.y, end.y);
        float extra = arcHeight + distance * extraHeightPerUnit;
        if (extra < minArcHeight) extra = minArcHeight;

        float apexY = baseHeight + extra;

        // 4. Tính toán vy0 & thời gian bay
        float h1 = apexY - start.y;   // từ start lên đến apex
        float h2 = apexY - end.y;     // từ apex rơi xuống target

        if (h1 < 0.01f) h1 = 0.01f;
        if (h2 < 0.01f) h2 = 0.01f;

        // vy0^2 = 2 * g * h1
        float vy0 = Mathf.Sqrt(2f * gAbs * h1);
        float tUp = vy0 / gAbs;
        float tDown = Mathf.Sqrt(2f * h2 / gAbs);
        float tTotal = tUp + tDown;

        float vx0 = dx / tTotal;

        Vector2 initialVelocity = new Vector2(vx0, vy0);

        // 5. Tạo arrow & launch
        GameObject arrowObj = Instantiate(arrowPrefab, start, Quaternion.identity);
        Arrow arrow = arrowObj.GetComponent<Arrow>();
        if (arrow != null)
        {
            arrow.Launch(initialVelocity);     // 🔹 chỉ 1 tham số
        }

        // 6. Xoay cung theo hướng bắn (không bắt buộc)
        if (bow != null)
        {
            float angle = Mathf.Atan2(initialVelocity.y, initialVelocity.x) * Mathf.Rad2Deg;
            bow.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
