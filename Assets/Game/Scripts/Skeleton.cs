using UnityEngine;

public class Skeleton : MonoBehaviour
{
    [Header("Flip")]
    public bool faceRight = true;      // đang nhìn sang phải?
    public float flipOffsetX = 1f;     // có thể dùng nếu prefab scaleX khác 1

    private Transform tower;
    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;

        GameObject towerObj = GameObject.FindGameObjectWithTag("Tower");
        if (towerObj != null)
        {
            tower = towerObj.transform;
        }
        else
        {
            Debug.LogWarning("[Skeleton] Không tìm được object có Tag = 'Tower'");
        }
    }

    void Update()
    {
        if (tower == null) return;

        // nếu tower ở bên phải -> nhìn phải
        if (tower.position.x > transform.position.x)
        {
            if (!faceRight)
                Flip(true);
        }
        else // tower ở bên trái -> nhìn trái
        {
            if (faceRight)
                Flip(false);
        }
    }

    void Flip(bool lookRight)
    {
        faceRight = lookRight;

        Vector3 scale = originalScale;

        // scale.x dương khi nhìn phải, âm khi nhìn trái
        scale.x = faceRight ? Mathf.Abs(originalScale.x) : -Mathf.Abs(originalScale.x);

        transform.localScale = scale;
    }

    public void TakeDamage(GameObject arrow)
    {
        // 1. Găm tên vào đầu (nếu muốn)
        if (arrow != null)
        {
            arrow.transform.SetParent(transform);
            arrow.GetComponent<Rigidbody2D>().simulated = false;
            arrow.transform.localPosition = Vector3.up * 0.2f; // chỉnh cho đẹp
        }

        // 2. Báo EnemyManager
        EnemyManager.Instance.EnemyDied();
    }

}
