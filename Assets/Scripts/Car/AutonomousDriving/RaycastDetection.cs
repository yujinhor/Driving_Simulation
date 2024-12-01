using UnityEngine;
using VehiclePhysics;

public class RaycastDetection : MonoBehaviour
{
    private VPStandardInput vpInput;

    public float rayLength = 3f; // 레이의 길이
    public LayerMask detectionLayer; // 감지할 레이어 설정
    public float rayHeightOffset = 0.5f; // Y축 높이
    public float rayBackwardOffset = 3f; // 뒤쪽으로 이동할 거리
    private Vector3 rayOrigin; // 디버그용 레이 시작점
    private Vector3 rayDirection; // 디버그용 레이 방향

    private void Start()
    {
        // VPStandardInput 및 LaneDetection 컴포넌트를 가져옵니다.
        vpInput = FindObjectOfType<VPStandardInput>();

        if (vpInput == null)
        {
            Debug.LogError("VPStandardInput component not found!");
        }

    }
    void Update()
    {
        // 차량 속도 확인
        float vehicleSpeed = vpInput.vehicle.speed;

        rayLength = (float)(3 + (0.3*vehicleSpeed));
        rayBackwardOffset = -(float)(0.3 * vehicleSpeed);

        // 레이 시작 위치와 방향 설정
        rayOrigin = transform.position + Vector3.up * rayHeightOffset - transform.forward * rayBackwardOffset; // 뒤쪽으로 오프셋 추가
        rayDirection = transform.forward;

        // 디버그용 레이 시각화
        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);


        if (vehicleSpeed < 1)
        {
            vpInput.externalHandbrake = 0f;
            vpInput.externalBrake = 0f;
            return;
        }

        // 레이캐스트 히트 정보 저장
        RaycastHit hitInfo;

        // 레이캐스트 수행
        if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo, rayLength, detectionLayer))
        {
            // 감지된 물체 정보 출력
            Debug.Log("Detected Object: " + hitInfo.collider.gameObject.name);
            // 물체 탐지 시 브레이크 적용
            vpInput.externalThrottle = 0f;
            vpInput.externalHandbrake = 1f; // 브레이크 강도 설정
            vpInput.externalBrake = 1f;
            Debug.Log("Obstacle detected! Braking... Speed: "+ vehicleSpeed);

            
            
        }
        else
        {
            // 물체가 없을 때
            Debug.Log("No objects detected in front.");
        }

        
    }

    private void OnDrawGizmos()
    {
        // 레이의 색상을 설정
        Gizmos.color = Color.red;

        // 레이를 시각적으로 표시
        Gizmos.DrawLine(rayOrigin, rayOrigin + rayDirection * rayLength);
    }

}
