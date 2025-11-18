using UnityEngine;
using DG.Tweening;

public class EnemyEntranceAssembler : MonoBehaviour
{
    [System.Serializable]
    public class BodyPart
    {
        public Transform part;            // Transform của từng bộ phận
        public float startYOffset = 2f;   // float xuống từ trên cao bao nhiêu
        public float moveDuration = 0.4f; // thời gian trôi xuống
        public float delayAfter = 0.1f;   // chờ trước khi tới part tiếp theo

        [HideInInspector] public Vector3 originalLocalPos;
        [HideInInspector] public SpriteRenderer sr;
    }

    [Header("Body Parts (theo thứ tự xuất hiện)")]
    public BodyPart[] parts;

    [Header("General")]
    public float startDelay = 0f;
    public Ease moveEase = Ease.OutQuad;
    public Ease fadeEase = Ease.InQuad;
    public bool playOnEnable = true;

    [Header("Activation after assembled")]
    public Animator animator;                      // animator của enemy
    public string idleStateName = "Idle";          // tên state idle (nếu muốn play thẳng)
    public MonoBehaviour[] scriptsToEnable;        // ví dụ: AI, move, attack...
    public Collider2D[] collidersToEnable;         // collider thân, hurtbox...

    private Sequence entranceSeq;

    void Awake()
    {
        // Lưu vị trí local gốc & SpriteRenderer
        foreach (var p in parts)
        {
            if (p.part == null) continue;
            p.originalLocalPos = p.part.localPosition;
            p.sr = p.part.GetComponent<SpriteRenderer>();
        }

        // Nếu chưa gán Animator thì tự tìm
        if (!animator)
            animator = GetComponent<Animator>();

        // ❌ Tắt Animator + script + collider trước khi entrance hoàn thành
        if (animator) animator.enabled = false;

        if (scriptsToEnable != null)
        {
            foreach (var s in scriptsToEnable)
                if (s != null) s.enabled = false;
        }

        if (collidersToEnable != null)
        {
            foreach (var c in collidersToEnable)
                if (c != null) c.enabled = false;
        }
    }

    void OnEnable()
    {
        if (playOnEnable)
        {
            PlayEntrance();
        }
    }

    public void PlayEntrance()
    {
        if (entranceSeq != null && entranceSeq.IsActive())
            entranceSeq.Kill();

        entranceSeq = DOTween.Sequence().SetTarget(this).SetDelay(startDelay);

        foreach (var p in parts)
        {
            if (p.part == null) continue;

            // Đặt part lên cao + tắt alpha
            Vector3 startLocal = p.originalLocalPos + Vector3.up * p.startYOffset;
            p.part.localPosition = startLocal;

            if (p.sr != null)
            {
                Color c = p.sr.color;
                c.a = 0f;
                p.sr.color = c;
            }

            // Tween trôi xuống + fade-in
            var moveTween = p.part
                .DOLocalMove(p.originalLocalPos, p.moveDuration)
                .SetEase(moveEase);

            entranceSeq.Append(moveTween);

            if (p.sr != null)
            {
                entranceSeq.Join(
                    p.sr.DOFade(1f, p.moveDuration).SetEase(fadeEase)
                );
            }

            entranceSeq.AppendInterval(p.delayAfter);
        }

        // ⭐ Khi ghép xong thì bật Animator + script + collider + Idle
        entranceSeq.OnComplete(OnAssembled);
    }

    private void OnAssembled()
    {
        // Bật lại Animator
        if (animator)
        {
            animator.enabled = true;

            // Play Idle nếu có đặt tên
            if (!string.IsNullOrEmpty(idleStateName))
            {
                animator.Play(idleStateName, 0, 0f);
            }
        }

        // Bật lại các script (AI, move, attack, v.v.)
        if (scriptsToEnable != null)
        {
            foreach (var s in scriptsToEnable)
                if (s != null) s.enabled = true;
        }

        // Bật lại collider
        if (collidersToEnable != null)
        {
            foreach (var c in collidersToEnable)
                if (c != null) c.enabled = true;
        }

        // 🔔 Thông báo cho EnemyPullable là entrance đã hoàn thành
        EnemyPullable pullable = GetComponent<EnemyPullable>();
        if (pullable != null)
        {
            pullable.MarkEntranceComplete();
        }
    }
}
