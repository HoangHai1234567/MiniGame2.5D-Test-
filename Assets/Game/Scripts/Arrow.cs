using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Rigidbody2D rigidBody2D;
    private int targetEnemyId = -1;
    private bool hasHit = false;

    public float fallDestroyY = -10f;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    public void SetTargetEnemy(int id)
    {
        targetEnemyId = id;
    }

    public void Launch(Vector2 velocity)
    {
        rigidBody2D.velocity = velocity;
    }

    private void Update()
    {
        if (!hasHit)
        {
            RotateTowardsVelocity();

            // Hủy arrow nếu rơi quá sâu
            if (transform.position.y < fallDestroyY)
                Destroy(gameObject);
        }
    }

    private void RotateTowardsVelocity()
    {
        Vector2 v = rigidBody2D.velocity;

        if (v.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        if (!other.CompareTag("Enemy")) return;

        EnemyIdentity identity = other.GetComponentInParent<EnemyIdentity>();
        if (identity != null && targetEnemyId != -1)
        {
            if (identity.EnemyId != targetEnemyId)
                return;
        }

        hasHit = true;

        SkeletonRagdoll ragdoll = other.GetComponentInParent<SkeletonRagdoll>();
        if (ragdoll != null)
        {
            ragdoll.ActivateRagdoll(this, rigidBody2D.velocity.normalized);
        }

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.EnemyDied();

        rigidBody2D.velocity = Vector2.zero;
        rigidBody2D.angularVelocity = 0f;

        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        Destroy(gameObject, 2f);
    }

    private void OnDestroy()
    {
        if (!hasHit && targetEnemyId != -1 && PlayerShoot.Instance != null)
        {
            PlayerShoot.Instance.ReleaseReservedTarget(targetEnemyId);
        }
    }
}
