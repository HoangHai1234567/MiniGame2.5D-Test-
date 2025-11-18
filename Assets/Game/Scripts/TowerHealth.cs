using UnityEngine;
using System.Collections;

public class TowerHealth : MonoBehaviour
{
    [Header("Tower Stats")]
    public int maxHealth = 20;
    public int currentHealth;

    [Header("Effects")]
    public bool shakeOnHit = true;     // rung khi bị đánh
    public float shakeAmount = 0.1f;   // độ rung nhẹ
    public float shakeTime = 0.1f;     // thời gian rung

    private Vector3 originalPos;

    void Awake()
    {
        currentHealth = maxHealth;
        originalPos = transform.localPosition;
    }

    /// <summary>
    /// Enemy gọi hàm này khi tấn công tower
    /// </summary>
    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        Debug.Log($"[Tower] Bị tấn công {dmg} damage! Máu còn lại: {currentHealth}");

        if (shakeOnHit)
            StartCoroutine(DoShake());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    IEnumerator DoShake()
    {
        float timer = 0f;

        while (timer < shakeTime)
        {
            timer += Time.deltaTime;
            transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * shakeAmount;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

    void Die()
    {
        Debug.Log("[Tower] Tower đã bị phá hủy!");

        // Hiệu ứng phá hủy tower – tuỳ bạn muốn làm gì:
        // - Game Over
        // - Hiệu ứng sụp đổ
        // - Load lại scene
    }
}
