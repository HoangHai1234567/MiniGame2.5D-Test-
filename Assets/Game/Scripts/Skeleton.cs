using UnityEngine;

public class Skeleton : MonoBehaviour
{
    [Header("Flip")]
    public bool faceRight = true;
    public float flipOffsetX = 1f;

    private Transform tower;
    private Vector3 originalScale;
    private Animator animator;

    [Header("State Flags")]
    public bool isFloating = false;          // đang bị kéo / float
    public bool isAtAttackPosition = false;  // đang đứng tại AttackPos
    public bool entranceDone = false;        // đã xong entrance chưa

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask towerLayer;

    void Awake()
    {
        originalScale = transform.localScale;
        animator = GetComponent<Animator>();

        GameObject towerObj = GameObject.FindGameObjectWithTag("Tower");
        if (towerObj != null)
        {
            tower = towerObj.transform;
        }
        else
        {
            Debug.LogWarning("[Skeleton] Không tìm được object có Tag = 'Tower'");
        }

        // nếu bạn quên gán thì dùng tạm vị trí chính enemy
        if (attackPoint == null)
            attackPoint = transform;
    }

    void Update()
    {
        if (tower == null) return;

        // Chỉ flip khi đang bình thường (không float / không bị kéo)
        if (!isFloating)
        {
            bool shouldFaceRight = tower.position.x > transform.position.x;
            if (shouldFaceRight != faceRight)
            {
                Flip(shouldFaceRight);
            }
        }
    }

    void Flip(bool lookRight)
    {
        faceRight = lookRight;

        Vector3 scale = originalScale;
        scale.x = faceRight ? Mathf.Abs(originalScale.x) : -Mathf.Abs(originalScale.x);
        transform.localScale = scale;
    }

    // ------------------------------------------------------------
    // HÀM NÀY ĐỂ SCRIPT KHÁC GỌI KHI ĐÃ TỚI ATTACK POS
    // (ví dụ EnemyPullable gọi khi enemy chạm AttackPos)
    // ------------------------------------------------------------
    public void PlayAttackAnimation()
    {
        if (animator == null) return;

        // chỉ cho phép đánh khi đã xong entrance + đang ở AttackPos + không bị float
        if (!entranceDone) return;
        if (isFloating) return;
        if (!isAtAttackPosition) return;

        // isAttacking trong Animator NHỚ để kiểu Trigger
        animator.SetTrigger("isAttacking");
    }

    // ------------------------------------------------------------
    // HÀM NÀY GỌI BẰNG ANIMATION EVENT TRONG CLIP Enemy_Attack
    // ------------------------------------------------------------
    public void AttackTower()
    {
        // bảo hiểm điều kiện state
        if (!entranceDone) return;
        if (isFloating) return;
        if (!isAtAttackPosition) return;
        if (tower == null) return;

        if (attackPoint == null) attackPoint = transform;

        // chỉ tìm Tower trong khoảng attackRange, theo layer towerLayer
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRange, towerLayer);
        if (hit == null) return;

        TowerHealth towerHealth = hit.GetComponent<TowerHealth>();
        if (towerHealth != null)
        {
            towerHealth.TakeDamage(attackDamage);
            Debug.Log($"[Enemy Attack] {name} gây {attackDamage} damage lên Tower!");
        }
        else
        {
            Debug.LogWarning("[Enemy Attack] Collider trong attackRange không có TowerHealth!");
        }
    }

    // ------------------------------------------------------------

    public void TakeDamage(GameObject arrow)
    {
        if (arrow != null)
        {
            arrow.transform.SetParent(transform);

            var rbArrow = arrow.GetComponent<Rigidbody2D>();
            if (rbArrow != null)
                rbArrow.simulated = false;

            // cho mũi tên găm lên phía trên 1 chút
            arrow.transform.localPosition = Vector3.up * 0.2f;
        }

        // báo cho EnemyManager biết enemy đã chết
        EnemyManager.Instance.EnemyDied();
    }

    // Vẽ vùng tấn công trong Scene view
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
