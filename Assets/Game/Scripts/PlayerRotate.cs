using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{

    [Header("Player Part")]
    public Transform playerHead;
    public Transform playerBody;
    public Transform armRight;
    public Transform bowArmLeft;

    [Header("Settings")]
    public float rotateSpeed = 20f;
    public float bodyInfluence = 0.5f;
    public float headInfluence = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AimToMouse();
    }

    void AimToMouse()
    {
        if (!playerBody) return;

        // 1) Lấy vị trí chuột trong world
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = playerBody.position.z;

        // 2) Tính vector hướng từ thân → chuột
        Vector3 dir = (mouseWorld - playerBody.position);
        dir.z = 0f;
        dir.Normalize();

        // 3) Lấy góc (theo trục Z, 2D)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 4) Xoay từng phần
        // Thân xoay ít hơn tay
        Quaternion bodyTargetRot = Quaternion.AngleAxis(angle * bodyInfluence, Vector3.forward);
        playerBody.rotation = Quaternion.Lerp(
            playerBody.rotation,
            bodyTargetRot,
            rotateSpeed * Time.deltaTime
        );

        // Tay xoay đúng hướng
        Quaternion armTargetRot = Quaternion.AngleAxis(angle, Vector3.forward);

        if (bowArmLeft)
        {
            bowArmLeft.rotation = Quaternion.Lerp(
                bowArmLeft.rotation,
                armTargetRot,
                rotateSpeed * Time.deltaTime
            );
        }

        if (armRight)
        {
            armRight.rotation = Quaternion.Lerp(
                armRight.rotation,
                armTargetRot,
                rotateSpeed * Time.deltaTime
            );
        }

        // Đầu xoay theo nhưng mềm hơn
        if (playerHead)
        {
            Quaternion headTargetRot = Quaternion.AngleAxis(angle * headInfluence, Vector3.forward);
            playerHead.rotation = Quaternion.Lerp(
                playerHead.rotation,
                headTargetRot,
                rotateSpeed * Time.deltaTime
            );
        }
    }
}
