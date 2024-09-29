using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCameraMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f; // 카메라 이동 속도
    public float lookSpeed = 2.0f; // 마우스 회전 속도
    public float maxLookAngle = 80f; // 카메라가 볼 수 있는 최대 각도

    private float yaw = 0.0f; // 가로 회전
    private float pitch = 0.0f; // 세로 회전

    void Update()
    {
        // 카메라 이동 처리 (W, A, S, D 키로 이동)
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime; // 좌우 이동 (A, D)
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime; // 앞뒤 이동 (W, S)

        transform.Translate(new Vector3(moveX, 0, moveZ));

        // 마우스 입력을 통해 카메라 회전 처리
        yaw += Input.GetAxis("Mouse X") * lookSpeed; // 가로 회전
        pitch -= Input.GetAxis("Mouse Y") * lookSpeed; // 세로 회전

        // 카메라가 볼 수 있는 각도를 제한
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        // 카메라 회전 적용
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}

