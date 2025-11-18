using System.Collections.Generic;
using UnityEngine;

public class SkeletonRagdoll : MonoBehaviour
{
    [Header("Skeleton Parts")]
    public Transform head;
    public Transform[] bodyParts;

    [Header("Physics")]
    public float gravity = 3f;
    public float upForceMin = 6f;
    public float upForceMax = 10f;
    public float sideForce = 3f;
    public float torque = 200f;
    public float lifeTime = 4f;

    [Header("Disable On Death")]
    public Animator anim;
    public MonoBehaviour[] scriptsToDisable;
    public Collider2D rootCollider;

    bool isDead = false;

    public void ActivateRagdoll(Arrow arrow, Vector2 hitDirection)
    {
        if (isDead) return;
        isDead = true;

        // Disable logic scripts
        if (anim != null) anim.enabled = false;
        if (rootCollider != null) rootCollider.enabled = false;

        foreach (var s in scriptsToDisable)
            if (s != null) s.enabled = false;

        // Collect all parts
        List<Transform> parts = new List<Transform>();
        if (head != null) parts.Add(head);
        foreach (var p in bodyParts)
            if (p != null) parts.Add(p);

        Rigidbody2D headRB = null;
        Vector2 headPos = head != null ? head.position : transform.position;

        // Detach body parts
        foreach (Transform part in parts)
        {
            if (part == null) continue;

            part.SetParent(null);

            Rigidbody2D rb = part.GetComponent<Rigidbody2D>();
            if (rb == null) rb = part.gameObject.AddComponent<Rigidbody2D>();

            rb.gravityScale = gravity;
            rb.constraints = RigidbodyConstraints2D.None;

            float up = Random.Range(upForceMin, upForceMax);
            float side = Random.value < 0.5f ? -sideForce : sideForce;

            rb.AddForce(new Vector2(side, up), ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-torque, torque), ForceMode2D.Impulse);

            if (part == head) headRB = rb;

            Destroy(part.gameObject, lifeTime);
        }

        // Attach arrow to head
        if (arrow != null && headRB != null)
        {
            Rigidbody2D arrowRB = arrow.rigidBody2D;
            arrowRB.velocity = Vector2.zero;
            arrowRB.angularVelocity = 0f;

            Vector2 dir = hitDirection.normalized;
            float offset = 0.25f;

            Vector2 arrowPos = headPos - dir * offset;

            arrow.transform.position = arrowPos;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrowRB.MoveRotation(angle);

            FixedJoint2D joint = arrow.gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = headRB;
            joint.enableCollision = false;
        }

        // Destroy(gameObject, lifeTime + 0.3f);
        Destroy(gameObject);
    }
}

