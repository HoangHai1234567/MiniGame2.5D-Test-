using System.Collections.Generic;
using UnityEngine;

public class SkeletonRagdoll : MonoBehaviour
{
    [Header("Parts")]
    public Transform head;              // skeleton_head
    public Transform[] bodyParts;       // 6 phần còn lại: chest, arm_r, leg_l, pelvis, leg_r, sword_arm_l

    [Header("Physics")]
    public float partGravityScale = 3f;
    public float minUpForce = 3f;
    public float maxUpForce = 6f;
    public float sideForce = 2f;
    public float maxTorque = 200f;
    public float partLifeTime = 4f;

    [Header("Disable on death")]
    public bool disableAnimator = true;
    public bool disableRootCollider = true;
    public MonoBehaviour[] scriptsToDisable;   // kéo Skeleton AI, di chuyển... vào đây

    bool isDead = false;

    /// <summary>
    /// Gọi khi Arrow trúng: arrow găm vào đầu, cả body vỡ thành từng part
    /// </summary>
    public void ActivateRagdoll(Arrow arrow, Vector2 hitDirection)
    {
        if (isDead) return;
        isDead = true;

        // Tắt animator / collider / script logic trên root
        if (disableAnimator)
        {
            Animator anim = GetComponent<Animator>();
            if (anim) anim.enabled = false;
        }

        if (disableRootCollider)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col) col.enabled = false;
        }

        if (scriptsToDisable != null)
        {
            foreach (var s in scriptsToDisable)
                if (s) s.enabled = false;
        }

        // Gom tất cả part: head + body
        List<Transform> parts = new List<Transform>();
        if (head != null) parts.Add(head);
        foreach (var p in bodyParts)
            if (p != null && !parts.Contains(p)) parts.Add(p);

        Rigidbody2D headRb = null;
        Vector2 headPos = head != null ? (Vector2)head.position : (Vector2)transform.position;

        // Tách từng part ra khỏi root, add rigidbody và cho rơi
        foreach (Transform t in parts)
        {
            if (t == null) continue;

            // tách khỏi Skeleton (root)
            t.SetParent(null);

            Rigidbody2D rb = t.GetComponent<Rigidbody2D>();
            if (rb == null) rb = t.gameObject.AddComponent<Rigidbody2D>();

            rb.gravityScale = partGravityScale;
            rb.constraints = RigidbodyConstraints2D.None;

            // random hướng sang trái/phải
            float side = Random.value < 0.5f ? -1f : 1f;
            float up = Random.Range(minUpForce, maxUpForce);
            Vector2 force = new Vector2(side * sideForce, up);

            rb.AddForce(force, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-maxTorque, maxTorque), ForceMode2D.Impulse);

            if (t == head) headRb = rb;

            Destroy(t.gameObject, partLifeTime);
        }

        // Găm arrow vào HEAD
        if (arrow != null && head != null && headRb != null)
        {
            Rigidbody2D arrowRb = arrow.rb;
            if (!arrowRb) arrowRb = arrow.GetComponent<Rigidbody2D>();

            if (arrowRb != null)
            {
                // dừng arrow
                arrowRb.velocity = Vector2.zero;
                arrowRb.angularVelocity = 0f;

                Vector2 dir = hitDirection.sqrMagnitude > 0.0001f
                    ? hitDirection.normalized
                    : Vector2.right;

                float embedOffset = 0.2f;
                Vector2 arrowPos = headPos - dir * embedOffset;

                arrow.transform.position = arrowPos;

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                arrowRb.MoveRotation(angle);

                FixedJoint2D joint = arrow.gameObject.AddComponent<FixedJoint2D>();
                joint.connectedBody = headRb;
                joint.autoConfigureConnectedAnchor = false;
                joint.anchor = Vector2.zero;
                joint.connectedAnchor = headRb.transform.InverseTransformPoint(headPos);
                joint.enableCollision = false;
            }
        }

        // Root chỉ là empty container → hủy sau cùng
        Destroy(gameObject, partLifeTime + 0.5f);
    }
}
