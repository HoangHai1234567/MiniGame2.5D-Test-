using UnityEngine;

public class EnemyIdentity : MonoBehaviour
{
    private static int nextId = 1;          // bộ đếm ID toàn cục

    public int EnemyId { get; private set; }

    private void Awake()
    {
        EnemyId = nextId++;
        Debug.Log($"[EnemyIdentity] {name} được gán ID = {EnemyId}");
    }
}

