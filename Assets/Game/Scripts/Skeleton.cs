using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{

    [Header("Flip")]
    public bool faceRight = true;
    public float flippOfffsetX = 1f;

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
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (tower == null) return;
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
        if (!faceRight)
        {   
            scale.x *= -1f;
        }
        transform.localScale = scale;
    }
}
