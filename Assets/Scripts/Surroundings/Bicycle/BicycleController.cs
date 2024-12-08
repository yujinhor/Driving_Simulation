using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BicycleController : MonoBehaviour
{
    public float moveDistance = 50f;  // 이동 거리
    public float moveSpeed = 5f;     // 이동 속도
    public float turnSpeed = 360f;   // 회전 속도

    private bool isMovingForward = true;

    void Start()
    {
        StartCoroutine(MoveAndTurnRoutine());
    }

    IEnumerator MoveAndTurnRoutine()
    {
        while (true)
        {
            // 앞으로 이동
            yield return StartCoroutine(MoveForward());

            // 180도 회전
            yield return StartCoroutine(TurnAround());
        }
    }

    IEnumerator MoveForward()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + transform.forward * moveDistance;

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
    }

    IEnumerator TurnAround()
    {
        float targetAngle = transform.eulerAngles.y + 180f;
        float currentAngle = transform.eulerAngles.y;

        while (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) > 1f)
        {
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentAngle, transform.eulerAngles.z);
            yield return null;
        }

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetAngle, transform.eulerAngles.z);
    }
}
