using UnityEngine;
using DG.Tweening;

public class EnemyPullable : MonoBehaviour
{
    [Header("Pull Settings")]
    public float pullDuration = 0.35f;
    public float returnDuration = 0.35f;
    public Ease pullEase = Ease.InCubic;
    public Ease returnEase = Ease.OutCubic;

    [Header("Tilt Settings")]
    public float maxTiltAngle = 20f;    // độ nghiêng tối đa khi kéo
    public Ease tiltEase = Ease.OutQuad;

    private Vector3 originalPos;
    private bool atAttackPos = false;
    private bool isMoving = false;
    private bool entranceComplete = false;

    private Tween moveTween;
    private Tween rotateTween;

    public bool CanBeTargeted
    {
        get { return entranceComplete && !isMoving && !atAttackPos; }
    }

    private void Awake()
    {
        originalPos = transform.position;
    }

    public void MarkEntranceComplete()
    {
        entranceComplete = true;
    }

    public void TogglePull(Vector3 attackPos)
    {
        if (!entranceComplete) return;
        if (isMoving) return;

        if (moveTween != null && moveTween.IsActive()) moveTween.Kill();
        if (rotateTween != null && rotateTween.IsActive()) rotateTween.Kill();

        isMoving = true;

        if (!atAttackPos)
            PullToAttack(attackPos);
        else
            ReturnToOriginal();
    }

    void PullToAttack(Vector3 attackPos)
    {
        // 👉 Tính hướng kéo
        Vector3 dir = attackPos - transform.position;

        // góc hướng kéo
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 👉 Nghiêng NGƯỢC lại hướng kéo
        float targetZ = -Mathf.Clamp(angle, -maxTiltAngle, maxTiltAngle);

        // ===== MOVE =====
        moveTween = transform.DOMove(attackPos, pullDuration)
                             .SetEase(pullEase)
                             .OnComplete(() =>
        {
            atAttackPos = true;
            isMoving = false;

            // 👉 Đến nơi → ĐỨNG THẲNG NGAY
            transform.rotation = Quaternion.identity;
        });

        // ===== ROTATE =====
        rotateTween = transform.DORotate(
            new Vector3(0, 0, targetZ),
            pullDuration * 0.8f
        )
        .SetEase(tiltEase);
    }

    void ReturnToOriginal()
    {
        // ===== MOVE BACK =====
        moveTween = transform.DOMove(originalPos, returnDuration)
                             .SetEase(returnEase)
                             .OnComplete(() =>
        {
            atAttackPos = false;
            isMoving = false;
        });

        // ===== ROTATE BACK TO 0 =====
        rotateTween = transform.DORotate(Vector3.zero, returnDuration * 0.8f)
                               .SetEase(tiltEase);
    }
}
