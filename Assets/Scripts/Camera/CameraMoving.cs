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
        float moveX = 0f;
        float moveZ = 0f;

        // TFGH 입력 감지
        if (Input.GetKey(KeyCode.F))  // 왼쪽 이동
        {
            moveX = -moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.H)) // 오른쪽 이동
        {
            moveX = moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.T)) // 전진
        {
            moveZ = moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.G)) // 후진
        {
            moveZ = -moveSpeed * Time.deltaTime;
        }

        // 카메라 이동 적용
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
