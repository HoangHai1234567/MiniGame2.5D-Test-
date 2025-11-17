using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Lifetime")]
    public float maxLifeTime = 5f;

    private bool launched = false;
    private float lifeTimer = 0f;

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!launched) return;

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= maxLifeTime)
        {
            Destroy(gameObject);
            return;
        }
    }

    void FixedUpdate()
    {
        if (!launched) return;
        RotateArrow();
    }

    // Gọi từ PlayerShoot
    public void Launch(Vector2 initialVelocity)
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();

        rb.velocity = initialVelocity;
        launched = true;

        RotateArrow();
    }

    void RotateArrow()
    {
        if (!rb) return;

        Vector2 vel = rb.velocity;
        if (vel.sqrMagnitude < 0.0001f) return;

        float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
        rb.MoveRotation(angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        // tìm SkeletonRagdoll trên root
        SkeletonRagdoll ragdoll = other.GetComponent<SkeletonRagdoll>();
        if (ragdoll == null)
            ragdoll = other.GetComponentInParent<SkeletonRagdoll>();
        if (ragdoll == null) return;

        Vector2 hitDir = rb != null ? rb.velocity : Vector2.right;

        // kích hoạt ragdoll: tách head + 6 part, arrow găm vào head
        ragdoll.ActivateRagdoll(this, hitDir);

        // tắt collider arrow để không va chạm thêm
        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        // KHÔNG Destroy arrow — nó đang dính vào head
    }


}
